using Microsoft.AspNetCore.Mvc;

namespace Asm_GD1.Controllers
{
    public class AccountController : Controller
    {
        public IActionResult Login() => View();
        public IActionResult Register() => View();
        public IActionResult Profile() => View();
        public IActionResult Orders() => View();
        public IActionResult Reviews() => View();
        public IActionResult Settings() => View();

        // Dữ liệu mâu cho đăng nhập
        [HttpPost]
        public IActionResult Login(string username, string password, bool rememberMe = false)
        {
            // Validate input
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                ViewBag.Error = "Vui lòng nhập đầy đủ thông tin đăng nhập";
                return View();
            }

            // Phân quyền dựa trên username
            switch (username.ToLower())
            {
                case "admin":
                    if (password == "admin")
                    {
                        // Set session cho user đã đăng nhập
                        HttpContext.Session.SetString("IsLoggedIn", "true");
                        HttpContext.Session.SetString("Username", username);
                        HttpContext.Session.SetString("UserRole", "Admin User");
                        HttpContext.Session.SetString("UserAvatar", "/Images/admin.jpg");

                        ViewBag.SuccessMessage = "Đăng nhập thành công với quyền Admin User";
                        ViewBag.RedirectUrl = Url.Action("Dashboard", "UserAdmin");
                        return View();
                    }
                    break;

                case "admin1":
                    if (password == "admin1")
                    {
                        HttpContext.Session.SetString("IsLoggedIn", "true");
                        HttpContext.Session.SetString("Username", username);
                        HttpContext.Session.SetString("UserRole", "Admin Food");
                        HttpContext.Session.SetString("UserAvatar", "/Images/admin1.jpg");

                        ViewBag.SuccessMessage = "Đăng nhập thành công với quyền Admin Food";
                        ViewBag.RedirectUrl = Url.Action("Dashboard", "FoodAdmin");
                        return View();
                    }
                    break;

                case "user":
                    if (password == "user")
                    {
                        HttpContext.Session.SetString("IsLoggedIn", "true");
                        HttpContext.Session.SetString("Username", username);
                        HttpContext.Session.SetString("UserRole", "Khách hàng");
                        HttpContext.Session.SetString("UserAvatar", "/Images/user.jpg");

                        ViewBag.SuccessMessage = $"Chào mừng {username} đến với FoodOrder!";
                        ViewBag.RedirectUrl = Url.Action("Profile", "Account");
                        return View();
                    }
                    break;

                default:
                    if (!string.IsNullOrEmpty(password))
                    {
                        HttpContext.Session.SetString("IsLoggedIn", "true");
                        HttpContext.Session.SetString("Username", username);
                        HttpContext.Session.SetString("UserRole", "Khách hàng");
                        HttpContext.Session.SetString("UserAvatar", "/Images/default-avatar.png");

                        ViewBag.SuccessMessage = $"Chào mừng {username} đến với FoodOrder!";
                        ViewBag.RedirectUrl = Url.Action("Profile", "Account");
                        return View();
                    }
                    break;
            }

            ViewBag.Error = "Tên đăng nhập hoặc mật khẩu không chính xác";
            return View();
        }

        // Thêm action Logout
        public IActionResult Logout()
        {
            // Clear session
            HttpContext.Session.Clear();

            TempData["LogoutSuccess"] = "Đã đăng xuất thành công";
            return RedirectToAction("Index", "Home");
        }

    }
}
