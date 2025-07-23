using System.ComponentModel.DataAnnotations;

namespace ShoeStore.Client.Models
{
    public class CartViewModel
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<CartItemViewModel> Items { get; set; } = new();
        public decimal TotalAmount => Items.Sum(item => item.TotalPrice);
        public int TotalItems => Items.Sum(item => item.Quantity);
        public string FormattedTotalAmount => TotalAmount.ToString("N0") + " VNĐ";
    }

    public class CartItemViewModel
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string ProductImage { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Size { get; set; }
        public int Quantity { get; set; }
        public int Stock { get; set; }
        public decimal TotalPrice => Price * Quantity;
        public string FormattedPrice => Price.ToString("N0") + " VNĐ";
        public string FormattedTotalPrice => TotalPrice.ToString("N0") + " VNĐ";
    }

    public class AddToCartViewModel
    {
        [Required]
        public int ProductId { get; set; }
        
        [Required]
        [Range(1, 50)]
        public int Size { get; set; }
        
        [Required]
        [Range(1, 10)]
        public int Quantity { get; set; } = 1;
    }
} 