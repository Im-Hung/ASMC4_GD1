using Microsoft.AspNetCore.Mvc;

namespace Asm_GD1.Controllers
{
    public class CartController : Controller
    {
        public IActionResult Index() => View();
        public IActionResult Success() => View();
        public IActionResult Failed() => View();
    }
}

