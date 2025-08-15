using Asm_GD1.Data;
using Asm_GD1.Models;
using Asm_GD1.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Asm_GD1.Controllers
{
    [Authorize(Roles = "adminit, admin1")]
    public class FoodAdminController : Controller
    {
        private readonly AppDbContext _context;
        private readonly SlugGenerator _slugGenerator;
        public FoodAdminController(AppDbContext context, SlugGenerator slugGenerator)
        {
            _context = context;
            _slugGenerator = slugGenerator;
        }
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

        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.ProductToppings = _context.ProductToppings.ToList();
            ViewBag.ProductSizes = _context.ProductSizes.ToList();

            // Lấy danh sách ảnh trong wwwroot/images
            var images = Directory.GetFiles(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images"))
                                  .Select(Path.GetFileName)
                                  .ToList();

            ViewBag.Images = images;

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Product product)
        {
            if (!ModelState.IsValid)
            {
                // Load lại các dữ liệu cần thiết cho view khi trả về lỗi
                ViewBag.ProductToppings = _context.ProductToppings.ToList();
                ViewBag.ProductSizes = _context.ProductSizes.ToList();
                var images = Directory.GetFiles(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images"))
                                      .Select(Path.GetFileName)
                                      .ToList();
                ViewBag.Images = images;
                return View(product);
            }

            var sizeId = _context.ProductSizes.First().SizeID;
            var toppingId = _context.ProductToppings.First().ToppingID;

            if (string.IsNullOrWhiteSpace(product.Slug))
            {
            var slug = _slugGenerator.GenerateSlug(product.Name);
            var baseSlug = slug;
            int counter = 1;

            while (await _context.Products.AnyAsync(p => p.Slug == slug))
            {
                slug = $"{baseSlug}-{counter}";
                counter++;
            }

                product.Slug = slug;
            }

            if (product.ImageFile != null && product.ImageFile.Length > 0)
            {
                var fileName = Path.GetFileName(product.ImageFile.FileName);
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await product.ImageFile.CopyToAsync(stream);
                }

                product.ImageUrl = "~/Images/" + fileName;
            }

            product.IsHot = true;

            product.SizeID = sizeId;
            product.ToppingID = toppingId;

            product.CreatedAt = DateTime.Now;
            product.UpdatedAt = DateTime.Now;

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            TempData["success"] = "Thêm món thành công";
            return RedirectToAction("MenuManagement");
        }

    }
}
