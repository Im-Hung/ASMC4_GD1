using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Asm_GD1.Controllers
{
    [Authorize(Roles = "admin, adminit, admin1")]
    public class FoodAdminController : Controller
    {
        public IActionResult Dashboard()
        {
            return View();
        }

        public IActionResult MenuManagement()
        {
            return View();
        }

        public IActionResult CategoryManagement()
        {
            return View();
        }

        public IActionResult PromotionManagement()
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
