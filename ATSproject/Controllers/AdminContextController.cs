using ATS_project.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ATS_project.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminContextController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminContextController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /AdminContext/Select
        public async Task<IActionResult> Select()
        {
            var accounts = await _context.Accounts
                .OrderBy(a => a.Name)
                .ToListAsync();

            return View(accounts);
        }

        // POST: /AdminContext/Select
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Select(Guid accountId)
        {
            HttpContext.Session.SetString("AdminAccountId", accountId.ToString());
            return RedirectToAction("Dashboard", "Admin");
        }

        // GET: /AdminContext/Clear
        public IActionResult Clear()
        {
            HttpContext.Session.Remove("AdminAccountId");
            return RedirectToAction("Dashboard", "Admin");
        }
    }
}
