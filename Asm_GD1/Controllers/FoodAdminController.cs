using Microsoft.AspNetCore.Mvc;

namespace Asm_GD1.Controllers
{
    public class FoodAdminController : Controller
    {
        public IActionResult Dashboard() => View();
        public IActionResult MenuManagement() => View();
        public IActionResult CategoryManagement() => View();
        public IActionResult PromotionManagement() => View();
        public IActionResult Reports() => View();
        public IActionResult Settings() => View();
    }
}
