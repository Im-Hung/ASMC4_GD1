using Asm_GD1.Data;
using Asm_GD1.Models;
using Asm_GD1.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Asm_GD1.Controllers
{
    [Authorize(Roles = "admin, adminit")]
    public class UserAdminController : Controller
    {
        private readonly AppDbContext _context;
        public UserAdminController(AppDbContext context)
        {
            _context = context;
        }
        public IActionResult Dashboard()
        {
            return View();
        }

        public IActionResult UserManagement()
        {
            return View();
        }

        public IActionResult OrderManagement()
        {
            return View();
        }

        public IActionResult CustomerReviews()
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

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Account account)
        {
            if (!ModelState.IsValid)
            {
                return View(account);
            }

            var hasher = new PasswordHasher<Account>();
            var acc = new Account
            {
                Username = account.Username,
                Email = account.Email,
                PhoneNumber = account.PhoneNumber,
                Role = account.Role,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            acc.Password = hasher.HashPassword(acc, account.Password);

            _context.Accounts.Add(acc);
            await _context.SaveChangesAsync();

            TempData["success"] = "Thêm tài khoản thành công";
            return RedirectToAction("UserManagement");
        }
    }
}
