using Asm_GD1.Data;
using Asm_GD1.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Asm_GD1.Controllers
{
    public class CartController : Controller
    {
        private readonly AppDbContext _context;
        private const string CART_KEY = "cart";
        public CartController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var cart = GetCart();
            return View(cart);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddToCart(
        int id,
        int? sizeId,
        [FromForm] int[]? toppingIds,
        int quantity,
        string? note = null)
        {
            quantity = Math.Clamp(quantity, 1, 10);

            var product = _context.Products
                .AsNoTracking()
                .FirstOrDefault(p => p.ProductID == id);
            if (product == null) return NotFound();

            var size = (sizeId.HasValue && sizeId.Value > 0)
                ? _context.ProductSizes.AsNoTracking()
                    .FirstOrDefault(s => s.SizeID == sizeId.Value)
                : null;

            var toppings = (toppingIds != null && toppingIds.Length > 0)
                ? _context.ProductToppings.AsNoTracking()
                    .Where(t => toppingIds.Contains(t.ToppingID))
                    .ToList()
                : new List<ProductTopping>();

            decimal basePrice = (product.DiscountPrice > 0 ? product.DiscountPrice : product.BasePrice);

            decimal sizeExtra = size?.ExtraPrice ?? 0;
            decimal toppingExtra = toppings.Sum(t => t.ExtraPrice);
            decimal unitPrice = basePrice + sizeExtra + toppingExtra;

            string toppingIdsCsv = toppings.Count > 0 ? string.Join(",", toppings.Select(t => t.ToppingID)) : "";
            string toppingNames = toppings.Count > 0 ? string.Join(", ", toppings.Select(t => t.ToppingName)) : "";

            var cart = GetCart();

            var sameItem = cart.FirstOrDefault(i =>
                i.ProductID == id
                && i.SizeID == (size?.SizeID ?? 0)
                && (i.ToppingID ?? 0) == toppingIdsCsv.FirstOrDefault()
                && string.Equals((i.Note ?? "").Trim(), (note ?? "").Trim(), StringComparison.OrdinalIgnoreCase));

            if (sameItem == null)
            {
                var newItem = new CartItem
                {
                    ProductID = product.ProductID,
                    ProductImage = product.ImageUrl,
                    ProductName = product.Name,

                    SizeID = size?.SizeID ?? 0,
                    SizeName = size?.SizeName ?? string.Empty,

                    ToppingID = toppingIdsCsv.FirstOrDefault(),
                    ToppingName = toppingNames,

                    Note = note?.Trim() ?? string.Empty,
                    UnitPrice = unitPrice,
                    Quantity = quantity,
                    TotalPrice = unitPrice * quantity
                };
                cart.Add(newItem);
            }
            else
            {
                sameItem.Quantity += quantity;
                sameItem.TotalPrice = sameItem.UnitPrice * sameItem.Quantity;
            }

            SaveCart(cart);
            return RedirectToAction("Index", "Home");
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult RemoveFromCart(int productId, int? sizeId, int? toppingId)
        {
            var cart = GetCart();
            var item = cart.FirstOrDefault(i => i.ProductID == productId
                                             && i.SizeID == sizeId
                                             && i.ToppingID == toppingId);
            if (item != null)
            {
                cart.Remove(item);
                SaveCart(cart);
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ClearCart()
        {
            // Cách 1: xóa hẳn key trong session
            HttpContext.Session.Remove(CART_KEY);

            // Hoặc cách 2:
            // SaveCart(new List<CartItem>());

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ChangeQuantity(int productId, int? sizeId, int? toppingId, int delta)
        {
            var cart = GetCart();
            var item = cart.FirstOrDefault(i => i.ProductID == productId
                                             && i.SizeID == sizeId
                                             && i.ToppingID == toppingId);
            if (item == null) return NotFound();

            item.Quantity = Math.Clamp(item.Quantity + delta, 1, 10);
            item.TotalPrice = item.UnitPrice * item.Quantity;

            SaveCart(cart);
            return RedirectToAction(nameof(Index));
        }

        private List<CartItem> GetCart()
        {
            var sessionData = HttpContext.Session.GetString(CART_KEY);
            if (string.IsNullOrEmpty(sessionData))
                return new List<CartItem>();
            return JsonConvert.DeserializeObject<List<CartItem>>(sessionData) ?? new List<CartItem>();
        }

        private void SaveCart(List<CartItem> cart)
        {
            var jsonData = JsonConvert.SerializeObject(cart);
            HttpContext.Session.SetString(CART_KEY, jsonData);
        }
    }
}

