using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Asm_GD1.Controllers
{
    [Authorize(Roles = "admin, adminit")]
    public class UserAdminController : Controller
    {
        public IActionResult Dashboard()
        {
            return View();
        }

        public IActionResult UserManagement()
        {
            return View();
        }

        public IActionResult OrderManagement()
        {
            return View();
        }

        public IActionResult CustomerReviews()
        {
            return View();
        }

        public IActionResult Reports()
        {
            return View();
        }

        public IActionResult Settings()
        {
            return View();
        }
    }
}
