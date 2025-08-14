using Asm_GD1.Data;
using Asm_GD1.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Asm_GD1.Controllers
{
    public class CartController : BaseController
    {

        public CartController(AppDbContext context) : base(context)
        {

        }

        public async Task<IActionResult> Index()
        {
            if (!User.Identity?.IsAuthenticated ?? true)
            {
                TempData["ErrorMessage"] = "Bạn cần đăng nhập để xem giỏ hàng.";
                return RedirectToAction("Login", "Account");
            }

            int userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);
            var cart = await GetCartAsync(userId);
            return View(cart.CartItems);
        }

        //THÊM MỚI: Method GetCartAsync
        private async Task<Cart> GetCartAsync(int userId)
        {
            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .FirstOrDefaultAsync(c => c.UserID == userId);

            if (cart == null)
            {
                cart = new Cart
                {
                    UserID = userId,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                _context.Carts.Add(cart);
                await _context.SaveChangesAsync();
            }

            return cart;
        }

        // THÊM MỚI: Action Checkout
        public async Task<IActionResult> Checkout()
        {
            if (!User.Identity?.IsAuthenticated ?? true)
            {
                TempData["ErrorMessage"] = "Bạn cần đăng nhập để thanh toán.";
                return RedirectToAction("Login", "Account");
            }

            int userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);
            var cart = await GetCartAsync(userId);

            if (!cart.CartItems.Any())
            {
                TempData["ErrorMessage"] = "Giỏ hàng của bạn đang trống.";
                return RedirectToAction("Index");
            }

            return View(cart.CartItems);
        }

        //THÊM MỚI: Thanh toán thành công
        public IActionResult Success()
        {
            if (TempData["OrderSuccess"] == null)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        // THÊM MỚI: Xử lý đặt hàng
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PlaceOrder(string fullName, string phone, string email,
    string address, string city, string district, string ward, string note,
    string deliveryTime, string paymentMethod)
        {
            // THÊM DEBUG
            Console.WriteLine("=== PlaceOrder method called ===");
            Console.WriteLine($"FullName: {fullName}");
            Console.WriteLine($"Phone: {phone}");
            Console.WriteLine($"DeliveryTime: {deliveryTime}");

            if (!User.Identity?.IsAuthenticated ?? true)
            {
                Console.WriteLine("User not authenticated");
                return RedirectToAction("Login", "Account");
            }

            int userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);
            Console.WriteLine($"UserId: {userId}");

            var cart = await GetCartAsync(userId);

            if (!cart.CartItems.Any())
            {
                Console.WriteLine("Cart is empty");
                TempData["ErrorMessage"] = "Giỏ hàng của bạn đang trống.";
                return RedirectToAction("Index");
            }

            Console.WriteLine($"Cart has {cart.CartItems.Count} items");

            // Xóa giỏ hàng sau khi đặt thành công
            _context.CartItems.RemoveRange(cart.CartItems);
            await _context.SaveChangesAsync();

            Console.WriteLine("Cart cleared successfully");

            // Truyền thông tin qua TempData
            TempData["OrderSuccess"] = true;
            TempData["CustomerName"] = fullName;
            TempData["CustomerPhone"] = phone;
            TempData["CustomerAddress"] = address + ", " + ward + ", " + district + ", " + city;
            TempData["DeliveryType"] = deliveryTime == "now" ? "Tại chỗ" : "Giao hàng";
            TempData["PaymentMethod"] = paymentMethod;

            Console.WriteLine("TempData set, redirecting to Success");

            return RedirectToAction("Success");
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddToCart(
        int id,
        int? sizeId,
        [FromForm] int[]? toppingIds,
        int quantity,
        string? note = null)
        {
            if (!User.Identity?.IsAuthenticated ?? true)
            {
                TempData["ErrorMessage"] = "Bạn cần đăng nhập để thêm sản phẩm vào giỏ hàng.";
                return RedirectToAction("Login", "Account");
            }

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

            int userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);
            var cart = await GetCartAsync(userId);

            var sameItem = cart.CartItems.FirstOrDefault(i =>
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
                cart.CartItems.Add(newItem);
            }
            else
            {
                sameItem.Quantity += quantity;
                sameItem.TotalPrice = sameItem.UnitPrice * sameItem.Quantity;
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Home");
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveFromCart(int productId, int? sizeId, int? toppingId)
        {
            int userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);
            var cart = await GetCartAsync(userId);
            var item = cart.CartItems.FirstOrDefault(i => i.ProductID == productId
                                             && i.SizeID == sizeId
                                             && i.ToppingID == toppingId);
            if (item != null)
            {
                cart.CartItems.Remove(item);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ClearCart()
        {
            int userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);
            var cart = await GetCartAsync(userId);

            if (cart.CartItems.Any())
            {
                _context.CartItems.RemoveRange(cart.CartItems);
                cart.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeQuantity(int productId, int? sizeId, int? toppingId, int delta)
        {
            int userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);
            var cart = await GetCartAsync(userId);
            var item = cart.CartItems.FirstOrDefault(i => i.ProductID == productId
                                             && i.SizeID == sizeId
                                             && i.ToppingID == toppingId);
            if (item == null) return NotFound();

            item.Quantity = Math.Clamp(item.Quantity + delta, 1, 10);
            item.TotalPrice = item.UnitPrice * item.Quantity;

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}

