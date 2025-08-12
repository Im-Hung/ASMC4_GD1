using System.ComponentModel.DataAnnotations;

namespace Asm_GD1.Models
{
    public class ProductTopping
    {
        [Key]
        public int ToppingID { get; set; }
        public string ToppingName { get; set; } = string.Empty;
        public decimal ExtraPrice { get; set; }
    }
}
