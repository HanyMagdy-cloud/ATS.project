using ATS_project.Data;
using ATS_project.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ATS_project.Controllers
{
    [Authorize(Roles = "Admin,Customer")]
    public class KanbanController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public KanbanController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        private async Task<Guid?> GetCurrentAccountIdAsync()
        {
            // Admin uses selected company context from Session
            if (User.IsInRole("Admin"))
            {
                var s = HttpContext.Session.GetString("AdminAccountId");
                if (Guid.TryParse(s, out var adminAccountId))
                    return adminAccountId;
                return null;
            }

            // Customer uses their own AccountId
            var user = await _userManager.GetUserAsync(User);
            return user?.AccountId;
        }

        // GET: /Kanban?jobId=...&q=...
        public async Task<IActionResult> Index(Guid? jobId, string? q)
        {
            var accountId = await GetCurrentAccountIdAsync();
            if (accountId == null)
            {
                ViewBag.Message = User.IsInRole("Admin")
                    ? "Admin: Please select a company context first."
                    : "No company assigned to this user.";
                return View(new List<Application>());
            }

            // Jobs list for dropdown
            ViewBag.Jobs = await _context.Jobs
                .Where(j => j.AccountId == accountId)
                .OrderBy(j => j.Title)
                .ToListAsync();

            ViewBag.SelectedJobId = jobId;
            ViewBag.Query = q ?? "";

            // Base query
            var appsQuery = _context.Applications
                .Where(a => a.AccountId == accountId)
                .Include(a => a.Candidate)
                .Include(a => a.Job)
                .AsQueryable();

            // Filter: job
            if (jobId.HasValue)
                appsQuery = appsQuery.Where(a => a.JobId == jobId.Value);

            // Filter: candidate name/email contains
            if (!string.IsNullOrWhiteSpace(q))
            {
                var term = q.Trim().ToLower();
                appsQuery = appsQuery.Where(a =>
                    (a.Candidate != null) &&
                    (
                        (a.Candidate.FirstName + " " + a.Candidate.LastName).ToLower().Contains(term) ||
                        a.Candidate.Email.ToLower().Contains(term)
                    )
                );
            }

            var apps = await appsQuery
                .OrderByDescending(a => a.CreatedAt)
                .ToListAsync();

            return View(apps);
        }

        // POST: /Kanban/UpdateStage
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStage(Guid applicationId, string stage, Guid? jobId, string? q)
        {
            var accountId = await GetCurrentAccountIdAsync();
            if (accountId == null) return Forbid();

            // Validate stage
            var allowed = new[] { "Applied", "Interview", "Hired", "Rejected" };
            if (!allowed.Contains(stage))
                return BadRequest("Invalid stage.");

            // Ensure application belongs to current account (multi-tenant safe)
            var app = await _context.Applications
                .FirstOrDefaultAsync(a => a.ApplicationId == applicationId && a.AccountId == accountId);

            if (app == null) return NotFound();

            app.Stage = stage;
            await _context.SaveChangesAsync();

            // Redirect back keeping filters
            return RedirectToAction(nameof(Index), new { jobId, q });
        }
    }
}
