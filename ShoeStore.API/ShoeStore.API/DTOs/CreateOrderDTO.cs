using System.ComponentModel.DataAnnotations;

namespace ShoeStore.API.DTOs
{
    public class CreateOrderDTO
    {
        [Required]
        public int CustomerId { get; set; }

        [Required]
        [StringLength(200, ErrorMessage = "Shipping address cannot be longer than 200 characters")]
        public string ShippingAddress { get; set; } = string.Empty;

        [Required]
        [Phone(ErrorMessage = "Invalid phone number format")]
        public string PhoneNumber { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Notes cannot be longer than 500 characters")]
        public string? Notes { get; set; }

        [Required]
        [MinLength(1, ErrorMessage = "Order must have at least one item")]
        public List<CreateOrderDetailDTO> OrderDetails { get; set; } = new();
    }

    public class CreateOrderDetailDTO
    {
        [Required]
        public int ProductId { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than zero")]
        public int Quantity { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than zero")]
        public decimal Price { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Size must be greater than zero")]
        public int Size { get; set; }
    }
} 