using System.ComponentModel.DataAnnotations;

namespace ShoeStore.API.DTOs
{
    public class CartResponseDTO
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public int TotalItems { get; set; }
        public decimal TotalAmount { get; set; }
        public List<CartItemResponseDTO> Items { get; set; } = new List<CartItemResponseDTO>();
    }

    public class CartItemResponseDTO
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string ProductImageUrl { get; set; } = string.Empty;
        public string BrandName { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Size { get; set; }
        public int Quantity { get; set; }
        public decimal Subtotal { get; set; }
        public DateTime AddedAt { get; set; }
    }

    public class AddToCartDTO
    {
        [Required]
        public int ProductId { get; set; }

        [Required]
        [Range(35, 45, ErrorMessage = "Size must be between 35 and 45")]
        public int Size { get; set; }

        [Required]
        [Range(1, 10, ErrorMessage = "Quantity must be between 1 and 10")]
        public int Quantity { get; set; }
    }

    public class UpdateCartItemDTO
    {
        [Required]
        [Range(1, 10, ErrorMessage = "Quantity must be between 1 and 10")]
        public int Quantity { get; set; }
    }

    public class CheckoutDTO
    {
        [Required]
        [StringLength(200, ErrorMessage = "Shipping address cannot be longer than 200 characters")]
        public string ShippingAddress { get; set; } = string.Empty;

        [Required]
        [Phone(ErrorMessage = "Invalid phone number format")]
        public string PhoneNumber { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Notes cannot be longer than 500 characters")]
        public string? Notes { get; set; }
    }

    public class CartSummaryDTO
    {
        public int TotalItems { get; set; }
        public decimal TotalAmount { get; set; }
        public int ItemCount { get; set; }
    }
} 