using System.ComponentModel.DataAnnotations;

namespace ShoeStore.API.DTOs
{
    public class UpdateOrderStatusDTO
    {
        [Required]
        [RegularExpression("^(Pending|Processing|Shipped|Delivered|Cancelled)$", 
            ErrorMessage = "Status must be one of: Pending, Processing, Shipped, Delivered, Cancelled")]
        public string Status { get; set; } = string.Empty;
    }
} 