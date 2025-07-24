using Microsoft.AspNetCore.Mvc;

namespace Asm_GD1.Controllers
{
    public class UserAdminController : Controller
    {
        public IActionResult Dashboard() => View();
        public IActionResult UserManagement() => View();
        public IActionResult OrderManagement() => View();
        public IActionResult CustomerReviews() => View();
        public IActionResult Reports() => View();
        public IActionResult Settings() => View();
    }
}
