using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Asm_GD1.Models
{
    public class Product
    {
        public int ProductID { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string? Slug { get; set; } = string.Empty;
        [NotMapped]
        public IFormFile? ImageFile { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public decimal BasePrice { get; set; }
        public decimal DiscountPrice { get; set; }
        public int DiscountPercent { get; set; }
        public decimal Rating { get; set; }
        public int RatingCount { get; set; }
        public int SoldCount { get; set; }
        public string NoteText { get; set; } = string.Empty;
        public bool IsHot { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public int? SizeID { get; set; }
        public ProductSize? Size { get; set; }
        public int? ToppingID { get; set; }
        public ProductTopping? Topping { get; set; }

    }
}
