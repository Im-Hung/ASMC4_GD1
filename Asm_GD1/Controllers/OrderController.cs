using Microsoft.AspNetCore.Mvc;

namespace Asm_GD1.Controllers
{
    public class OrderController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Detail(int id)
        {
            return View();
        }

        public ActionResult Checkout()
        {
            return View();
        }
    }
}
