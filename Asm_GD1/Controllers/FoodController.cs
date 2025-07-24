using Microsoft.AspNetCore.Mvc;

namespace Asm_GD1.Controllers
{
    public class FoodController : Controller
    {
        public IActionResult Order() => View();
        public IActionResult Detail(int id)
        {
            // Tạm thời trả về View với id, sau này sẽ lấy data từ database
            ViewBag.FoodId = id;
            return View();
        }
        public IActionResult Category(string type) => View();
    }
}
