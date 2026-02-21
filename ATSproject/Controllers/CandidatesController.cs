using ATS_project.Data;
using ATS_project.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ATS_project.Controllers
{
    [Authorize(Roles = "Admin,Customer")]
    public class CandidatesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public CandidatesController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        private async Task<Guid?> GetCurrentAccountIdAsync()
        {
            // Admin uses selected company from Session
            if (User.IsInRole("Admin"))
            {
                var s = HttpContext.Session.GetString("AdminAccountId");
                if (Guid.TryParse(s, out var adminAccountId))
                    return adminAccountId;

                return null; // Admin hasn't selected a company yet
            }

            // Customer uses their own AccountId from AspNetUsers
            var user = await _userManager.GetUserAsync(User);
            return user?.AccountId;
        }


        // GET: /Candidates  (Applications list)
        public async Task<IActionResult> Index()
        {
            var accountId = await GetCurrentAccountIdAsync();

            // For now: Admin needs company context (later). Customer must have AccountId.
            if (User.IsInRole("Admin") && accountId == null)
            {
                ViewBag.Message = "Admin: Please select a company context.";
                return View(new List<Application>());
            }

            if (accountId == null)
                return Forbid();

            var applications = await _context.Applications
                .Where(a => a.AccountId == accountId)
                .Include(a => a.Candidate)
                .Include(a => a.Job)
                .OrderByDescending(a => a.CreatedAt)
                .ToListAsync();

            return View(applications);
        }

        // GET: /Candidates/Create
        public async Task<IActionResult> Create()
        {
            var accountId = await GetCurrentAccountIdAsync();

            // Admin needs company context (later)
            if (User.IsInRole("Admin") && accountId == null)
                return Forbid();

            if (accountId == null)
                return Forbid();

            ViewBag.Jobs = await _context.Jobs
                .Where(j => j.AccountId == accountId)
                .OrderBy(j => j.Title)
                .ToListAsync();

            return View(new CreateApplication());
        }

        // POST: /Candidates/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateApplication model)
        {
            var accountId = await GetCurrentAccountIdAsync();

            if (User.IsInRole("Admin") && accountId == null)
                return Forbid();

            if (accountId == null)
                return Forbid();

            // Reload jobs list if we need to show the view again
            ViewBag.Jobs = await _context.Jobs
                .Where(j => j.AccountId == accountId)
                .OrderBy(j => j.Title)
                .ToListAsync();

            if (!ModelState.IsValid)
                return View(model);

            // Ensure job belongs to this company
            var jobExists = await _context.Jobs.AnyAsync(j => j.JobId == model.JobId && j.AccountId == accountId);
            if (!jobExists)
            {
                ModelState.AddModelError("", "Invalid job selection.");
                return View(model);
            }

            // 1) Find or create candidate (unique by email per company)
            var emailNormalized = model.Email.Trim().ToLower();

            var candidate = await _context.Candidates
                .FirstOrDefaultAsync(c => c.AccountId == accountId && c.Email.ToLower() == emailNormalized);

            if (candidate == null)
            {
                candidate = new Candidate
                {
                    CandidateId = Guid.NewGuid(),
                    AccountId = accountId.Value,
                    FirstName = model.FirstName.Trim(),
                    LastName = model.LastName.Trim(),
                    Email = model.Email.Trim(),
                    LinkedInUrl = model.LinkedInUrl?.Trim(),
                    Phone = model.Phone?.Trim(),
                    Notes = model.Notes,
                    CreatedAt = DateTime.UtcNow
                };

                _context.Candidates.Add(candidate);
                await _context.SaveChangesAsync();
            }
            else
            {
                // Optional: update candidate info with latest input
                candidate.FirstName = model.FirstName.Trim();
                candidate.LastName = model.LastName.Trim();
                candidate.LinkedInUrl = model.LinkedInUrl?.Trim();
                candidate.Phone = model.Phone?.Trim();
                candidate.Notes = model.Notes;

                _context.Candidates.Update(candidate);
                await _context.SaveChangesAsync();
            }

            // 2) Prevent duplicate application (same candidate + same job)
            var alreadyApplied = await _context.Applications.AnyAsync(a =>
                a.AccountId == accountId &&
                a.CandidateId == candidate.CandidateId &&
                a.JobId == model.JobId);

            if (alreadyApplied)
            {
                ModelState.AddModelError("", "This candidate already has an application for this job.");
                return View(model);
            }

            // 3) Create application
            var application = new Application
            {
                ApplicationId = Guid.NewGuid(),
                AccountId = accountId.Value,
                CandidateId = candidate.CandidateId,
                JobId = model.JobId,
                Stage = "Applied",
                CreatedAt = DateTime.UtcNow
            };

            _context.Applications.Add(application);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Candidate application added successfully!";
            return RedirectToAction(nameof(Index));
        }


    }
}
