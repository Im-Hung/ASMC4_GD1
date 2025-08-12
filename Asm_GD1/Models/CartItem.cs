using System.Drawing;

namespace Asm_GD1.Models
{
    public class CartItem
    {
        public int CartItemID { get; set; }
        public int CartID { get; set; }    
        public int ProductID { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string ProductImage { get; set; } = string.Empty;
        public int? SizeID { get; set; }
        public string SizeName { get; set; } = string.Empty;
        public int? ToppingID { get; set; }
        public string ToppingName { get; set; } = string.Empty;
        public string Note { get; set; } = string.Empty;
        public int Quantity { get; set; }         
        public decimal UnitPrice { get; set; }    
        public decimal TotalPrice { get; set; } 

        public Cart? Cart { get; set; }
    }
}
