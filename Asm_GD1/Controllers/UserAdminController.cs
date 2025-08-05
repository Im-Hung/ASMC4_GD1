using Microsoft.AspNetCore.Mvc;

namespace Asm_GD1.Controllers
{
    public class UserAdminController : Controller
    {
        // Phương thức kiểm tra quyền truy cập
        private bool HasUserAdminAccess()
        {
            var role = HttpContext.Session.GetString("UserRole");
            return role == "Admin User" || role == "Admin IT" || role == "Admin All";
        }

        public IActionResult Dashboard()
        {
            if (!HasUserAdminAccess())
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        public IActionResult UserManagement()
        {
            if (!HasUserAdminAccess())
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        public IActionResult OrderManagement()
        {
            if (!HasUserAdminAccess())
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        public IActionResult CustomerReviews()
        {
            if (!HasUserAdminAccess())
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        public IActionResult Reports()
        {
            if (!HasUserAdminAccess())
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        public IActionResult Settings()
        {
            if (!HasUserAdminAccess())
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }
    }
}
