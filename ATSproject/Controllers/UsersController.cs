using ATS_project.Data;
using ATS_project.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ATS_project.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UsersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public UsersController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: /Users
        public async Task<IActionResult> Index()
        {
            // Get only customer users (AccountId != null)
            var users = await _userManager.Users
                .Where(u => u.AccountId != null)
                .ToListAsync();

            // Load accounts to map AccountId -> Company Name
            var accounts = await _context.Accounts.ToListAsync();
            var accountNameById = accounts.ToDictionary(a => a.AccountId, a => a.Name);

            ViewBag.AccountNameById = accountNameById;

            return View(users);
        }

        // GET: /Users/CreateAdmin
        public IActionResult CreateAdmin()
        {
            return View(new CreateAdminUser());
        }

        // POST: /Users/CreateAdmin
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateAdmin(CreateAdminUser model)
        {
            if (!ModelState.IsValid)
                return View(model);

            // Create admin user (AccountId must be null)
            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                FullName = model.FullName,
                AccountId = null,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(user, model.TempPassword);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                    ModelState.AddModelError("", error.Description);

                return View(model);
            }

            // Assign Admin role
            await _userManager.AddToRoleAsync(user, "Admin");

            TempData["Success"] = "Admin user created successfully!";
            return RedirectToAction(nameof(Index));
        }


        // GET: /Users/CreateCustomer
        public async Task<IActionResult> CreateCustomer()
        {
            // We will use this later in the view as a dropdown.
            ViewBag.Accounts = await _context.Accounts
                .OrderBy(a => a.Name)
                .ToListAsync();

            return View(new CreateCustomerUser());
        }


        // POST: /Users/CreateCustomer
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateCustomer(CreateCustomerUser model)
        {
            // Reload accounts list if validation fails and we need to show the view again
            ViewBag.Accounts = await _context.Accounts
                .OrderBy(a => a.Name)
                .ToListAsync();

            if (!ModelState.IsValid)
                return View(model);

            // Rule: only 1 customer user per company
            var existingCustomerForAccount = await _userManager.Users
                .AnyAsync(u => u.AccountId == model.AccountId);

            if (existingCustomerForAccount)
            {
                ModelState.AddModelError("", "This company already has a customer user.");
                return View(model);
            }

            // Create the user
            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                FullName = model.FullName,
                AccountId = model.AccountId,
                EmailConfirmed = true // MVP: skip email confirmation
            };

            var result = await _userManager.CreateAsync(user, model.TempPassword);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                    ModelState.AddModelError("", error.Description);

                return View(model);
            }

            // Assign role Customer
            await _userManager.AddToRoleAsync(user, "Customer");
            TempData["Success"] = "Customer created successfully!";


            return RedirectToAction("Index", "Accounts");
        }

        // GET: /Users/EditCustomer/{id}
        public async Task<IActionResult> EditCustomer(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return NotFound();

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound();

            // Only allow editing customer users (must have AccountId)
            if (user.AccountId == null)
                return BadRequest("This is not a customer user.");

            ViewBag.Accounts = await _context.Accounts
                .OrderBy(a => a.Name)
                .ToListAsync();

            var model = new CreateCustomerUser
            {
                FullName = user.FullName,
                Email = user.Email ?? "",
                AccountId = user.AccountId.Value,
                TempPassword = "********" // not used here (we won't update password here)
            };

            ViewBag.UserId = user.Id;
            return View(model);
        }

        // POST: /Users/EditCustomer/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditCustomer(string id, CreateCustomerUser model)
        {
            if (string.IsNullOrWhiteSpace(id))
                return NotFound();

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound();

            if (user.AccountId == null)
                return BadRequest("This is not a customer user.");

            ViewBag.Accounts = await _context.Accounts
                .OrderBy(a => a.Name)
                .ToListAsync();

            // For edit, we don't require TempPassword (ignore validation if it exists)
            ModelState.Remove(nameof(CreateCustomerUser.TempPassword));

            if (!ModelState.IsValid)
            {
                ViewBag.UserId = user.Id;
                return View(model);
            }

            // Rule: only 1 customer user per company
            var alreadyUsed = await _userManager.Users.AnyAsync(u =>
                u.Id != user.Id &&
                u.AccountId == model.AccountId);

            if (alreadyUsed)
            {
                ModelState.AddModelError("", "This company already has a customer user.");
                ViewBag.UserId = user.Id;
                return View(model);
            }

            // Update fields
            user.FullName = model.FullName.Trim();
            user.AccountId = model.AccountId;

            // Email update should use Identity helpers (updates normalized fields too)
            var newEmail = model.Email.Trim();
            if (!string.Equals(user.Email, newEmail, StringComparison.OrdinalIgnoreCase))
            {
                var setEmail = await _userManager.SetEmailAsync(user, newEmail);
                if (!setEmail.Succeeded)
                {
                    foreach (var e in setEmail.Errors) ModelState.AddModelError("", e.Description);
                    ViewBag.UserId = user.Id;
                    return View(model);
                }

                var setUserName = await _userManager.SetUserNameAsync(user, newEmail);
                if (!setUserName.Succeeded)
                {
                    foreach (var e in setUserName.Errors) ModelState.AddModelError("", e.Description);
                    ViewBag.UserId = user.Id;
                    return View(model);
                }
            }

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                foreach (var e in result.Errors) ModelState.AddModelError("", e.Description);
                ViewBag.UserId = user.Id;
                return View(model);
            }

            TempData["Success"] = "Customer user updated successfully!";
            return RedirectToAction(nameof(Index));
        }


        // GET: /Users/DeleteCustomer/{id}
        public async Task<IActionResult> DeleteCustomer(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return NotFound();

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound();

            if (user.AccountId == null)
                return BadRequest("This is not a customer user.");

            // show company name
            var account = await _context.Accounts.FirstOrDefaultAsync(a => a.AccountId == user.AccountId);
            ViewBag.CompanyName = account?.Name ?? "-";

            return View(user);
        }

        // POST: /Users/DeleteCustomer/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteCustomerConfirmed(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return NotFound();

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound();

            if (user.AccountId == null)
                return BadRequest("This is not a customer user.");

            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
            {
                foreach (var e in result.Errors) ModelState.AddModelError("", e.Description);
                return View("DeleteCustomer", user);
            }

            TempData["Success"] = "Customer user deleted successfully!";
            return RedirectToAction(nameof(Index));
        }




    }
}
