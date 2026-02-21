using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ATS_project.Controllers
{
    [Authorize(Roles = "Admin,Customer")]
    public class CustomerController : Controller
    {
        public IActionResult Dashboard()
        {
            return View();
        }
    }
}
