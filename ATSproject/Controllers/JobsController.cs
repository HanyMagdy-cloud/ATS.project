using ATS_project.Data;
using ATS_project.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ATS_project.Controllers
{
    [Authorize(Roles = "Admin,Customer")]
    public class JobsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public JobsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
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


        // GET: /Jobs
        public async Task<IActionResult> Index()
        {
            var accountId = await GetCurrentAccountIdAsync();

            // Admin must have AccountId null, so for now we prevent listing jobs until we add company context
            if (User.IsInRole("Admin") && accountId == null)
            {
                ViewBag.Message = "Admin: Please select a company context (we will add this next).";
                return View(new List<Job>());
            }

            if (accountId == null)
                return Forbid();

            var jobs = await _context.Jobs
                .Where(j => j.AccountId == accountId)
                .OrderByDescending(j => j.CreatedAt)
                .ToListAsync();

            return View(jobs);
        }

        // GET: /Jobs/Create
        public async Task<IActionResult> Create()
        {
            var accountId = await GetCurrentAccountIdAsync();
            if (accountId == null) return Forbid();

            return View();
        }

        // POST: /Jobs/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Job job)
        {
            var accountId = await GetCurrentAccountIdAsync();
            if (accountId == null) return Forbid();

            if (!ModelState.IsValid)
                return View(job);

            job.JobId = Guid.NewGuid();
            job.AccountId = accountId.Value;
            job.CreatedAt = DateTime.UtcNow;

            _context.Jobs.Add(job);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Job created successfully!";
            return RedirectToAction(nameof(Index));
        }
    }
}
