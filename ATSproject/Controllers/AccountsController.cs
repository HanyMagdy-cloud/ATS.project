using ATS_project.Data;
using ATS_project.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ATS_project.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AccountsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AccountsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Accounts
        public async Task<IActionResult> Index()
        {
            var accounts = await _context.Accounts
                .OrderBy(a => a.CreatedAt)
                .ToListAsync();

            return View(accounts);
        }

        // GET: Accounts/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Accounts/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Account account)
        {
            if (ModelState.IsValid)
            {
                account.AccountId = Guid.NewGuid();
                account.CreatedAt = DateTime.UtcNow;

                _context.Add(account);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            return View(account);
        }
    }
}
