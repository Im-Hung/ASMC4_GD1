using Asm_GD1.Data;
using Asm_GD1.Models;
using Asm_GD1.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Asm_GD1.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class DiscountsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public DiscountsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("active")]
        public async Task<ActionResult<ApiResponse<List<object>>>> GetActive()
        {
            var now = DateTime.Now;
            var discounts = await _context.Discounts
                .Where(d => d.IsActive && d.StartDate <= now && d.EndDate >= now)
                .Select(d => new {
                    code = d.Code.ToUpper(),
                    discountPercent = d.DiscountPercent,
                    maxDiscountAmount = d.MaxDiscountAmount,
                    minOrderAmount = d.MinOrderAmount,
                    isActive = d.IsActive,
                    description = d.Description
                })
                .ToListAsync();

            Console.WriteLine($"?? GetActive API returning {discounts.Count} discounts");
            foreach (var d in discounts)
            {
                Console.WriteLine($"   - {d.code}: {d.discountPercent}%");
            }

            return Ok(new { success = true, data = discounts });
        }
    }
}
