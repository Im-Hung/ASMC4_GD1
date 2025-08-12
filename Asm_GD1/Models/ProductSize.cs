using System.ComponentModel.DataAnnotations;

namespace Asm_GD1.Models
{
    public class ProductSize
    {
        [Key]
        public int SizeID { get; set; }
        public string SizeName { get; set; } = string.Empty;
        public decimal ExtraPrice { get; set; }
    }
}
