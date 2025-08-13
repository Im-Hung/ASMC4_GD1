using Asm_GD1.Data;
using Asm_GD1.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Asm_GD1.Controllers
{
    public abstract class BaseController : Controller
    {
        protected readonly AppDbContext _context;
        protected const string SessionCartIdKey = "CartId";

        public BaseController(AppDbContext context)
        {
            _context = context;
        }

        protected int? GetCartIdFromSession()
        {
            return HttpContext.Session.GetInt32(SessionCartIdKey);
        }

        protected void SetCartIdToSession(int cartId)
        {
            HttpContext.Session.SetInt32(SessionCartIdKey, cartId);
        }

        protected async Task<Cart> GetOrCreateActiveCartAsync(int userId)
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

        protected async Task<Cart> GetCartAsync(int userId)
        {
            var cartId = GetCartIdFromSession();
            if (cartId.HasValue)
            {
                var cart = await _context.Carts
                    .Include(c => c.CartItems)
                    .FirstOrDefaultAsync(c => c.CartID == cartId && c.UserID == userId);

                if (cart != null) return cart;
            }

            var newCart = await GetOrCreateActiveCartAsync(userId);
            SetCartIdToSession(newCart.CartID);
            return newCart;
        }
    }
}
