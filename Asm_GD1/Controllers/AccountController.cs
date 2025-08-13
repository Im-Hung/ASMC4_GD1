using Asm_GD1.Data;
using Asm_GD1.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;

namespace Asm_GD1.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDbContext _context;

        public AccountController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Login() => View();
        public IActionResult Register() => View();
        public IActionResult Profile() => View();
        public IActionResult Orders() => View();
        public IActionResult Reviews() => View();
        public IActionResult Settings() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(Account model)
        {
            if (!ModelState.IsValid) return View(model);

            if (await _context.Accounts.AnyAsync(a => a.Email == model.Email))
            {
                ModelState.AddModelError(nameof(model.Email), "Email đã tồn tại");
                return View(model);
            }

            var hasher = new PasswordHasher<Account>();
            var account = new Account
            {
                Username = model.Username,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber
            };

            account.Password = hasher.HashPassword(account, model.Password);

            _context.Accounts.Add(account);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Đăng ký thành công. Vui lòng đăng nhập.";
            return RedirectToAction("Login");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(Account model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var account = await _context.Accounts.FirstOrDefaultAsync(a => a.Email == model.Email);

            if (account != null)
            {
                var hasher = new PasswordHasher<Account>();
                var verifyResult = hasher.VerifyHashedPassword(account, account.Password, model.Password);

                if (verifyResult == PasswordVerificationResult.Success)
                {
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.NameIdentifier, account.Id.ToString()),
                        new Claim(ClaimTypes.Name, string.IsNullOrWhiteSpace(account.Username) ? account.Email : account.Username),
                        new Claim(ClaimTypes.Email, account.Email),
                        new Claim(ClaimTypes.Role, account.Role ?? string.Empty)
                    };
                    var avatarValue = !string.IsNullOrWhiteSpace(account.Avatar) ? account.Avatar : Url.Content("~/images/user.jpg");
                    claims.Add(new Claim("Avatar", avatarValue));

                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                    var authProperties = new AuthenticationProperties
                    {
                        IsPersistent = model.RememberMe,
                    };

                    await HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(claimsIdentity),
                        authProperties);

                    TempData["SuccessMessage"] = $"Đăng nhập thành công với quyền: {account.Role}";

                    switch ((account.Role?.ToLowerInvariant()) ?? "")
                    {
                        case "adminit":
                        case "admin":
                        case "admin1":
                        case "staff":
                        case "employee1":
                        case "employee2":
                            return RedirectToAction("Dashboard", "FoodAdmin");
                        default:
                            return RedirectToAction("Index", "Home");
                    }
                }
            }

            ViewBag.Error = "Email hoặc mật khẩu không đúng.";
            return View(model);
        }

        public async Task<IActionResult> Logout()
        {
            // Xóa session giỏ hàng
            HttpContext.Session.Remove("cart"); // "cart" là CART_KEY bên CartController

            // Xóa toàn bộ session (nếu muốn)
            // HttpContext.Session.Clear();

            // Đăng xuất người dùng
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToAction("Login");
        }
    }
}
