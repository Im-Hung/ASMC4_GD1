using System.Diagnostics;
using Asm_GD1.Models;
using Microsoft.AspNetCore.Mvc;

namespace Asm_GD1.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
