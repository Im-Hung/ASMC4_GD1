using Microsoft.AspNetCore.Mvc;

namespace Asm_GD1.Controllers
{
    public class FoodAdminController : Controller
    {
        // Phương thức kiểm tra quyền truy cập Food Admin
        private bool HasFoodAdminAccess()
        {
            var role = HttpContext.Session.GetString("UserRole");
            return role == "Admin Food" || role == "Admin IT" || role == "Admin All";
        }

        public IActionResult Dashboard()
        {
            if (!HasFoodAdminAccess())
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        public IActionResult MenuManagement()
        {
            if (!HasFoodAdminAccess())
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        public IActionResult CategoryManagement()
        {
            if (!HasFoodAdminAccess())
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        public IActionResult PromotionManagement()
        {
            if (!HasFoodAdminAccess())
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        public IActionResult Reports()
        {
            if (!HasFoodAdminAccess())
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        public IActionResult Settings()
        {
            if (!HasFoodAdminAccess())
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }
    }
}
