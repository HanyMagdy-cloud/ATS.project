using ATS_project.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ATS_project.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Dashboard()
        {
            // ✅ ALWAYS load accounts for the dropdown first
            ViewBag.Accounts = await _context.Accounts
                .OrderBy(a => a.Name)
                .ToListAsync();

            // Get selected company from Session
            var s = HttpContext.Session.GetString("AdminAccountId");
            if (!Guid.TryParse(s, out var accountId))
            {
                ViewBag.Message = "No company selected. Please choose a company context.";
                return View();
            }

            // Load company
            var account = await _context.Accounts.FirstOrDefaultAsync(a => a.AccountId == accountId);
            if (account == null)
            {
                ViewBag.Message = "Selected company was not found.";
                return View();
            }

            // Counts for this company
            var jobsCount = await _context.Jobs.CountAsync(j => j.AccountId == accountId);
            var applicationsCount = await _context.Applications.CountAsync(a => a.AccountId == accountId);

            var appliedCount = await _context.Applications.CountAsync(a => a.AccountId == accountId && a.Stage == "Applied");
            var interviewCount = await _context.Applications.CountAsync(a => a.AccountId == accountId && a.Stage == "Interview");
            var hiredCount = await _context.Applications.CountAsync(a => a.AccountId == accountId && a.Stage == "Hired");
            var rejectedCount = await _context.Applications.CountAsync(a => a.AccountId == accountId && a.Stage == "Rejected");

            // Send to view
            ViewBag.CompanyName = account.Name;
            ViewBag.JobsCount = jobsCount;
            ViewBag.ApplicationsCount = applicationsCount;
            ViewBag.AppliedCount = appliedCount;
            ViewBag.InterviewCount = interviewCount;
            ViewBag.HiredCount = hiredCount;
            ViewBag.RejectedCount = rejectedCount;

            return View();
        }

    }
}
