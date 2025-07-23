using System.ComponentModel.DataAnnotations;

namespace ShoeStore.API.DTOs
{
    public class BrandResponseDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int ProductCount { get; set; }
        public List<ProductResponseDTO> Products { get; set; } = new();
    }

    public class CreateBrandDTO
    {
        [Required]
        [StringLength(50, ErrorMessage = "Name cannot be longer than 50 characters")]
        public string Name { get; set; } = string.Empty;
    }

    public class UpdateBrandDTO
    {
        [Required]
        [StringLength(50, ErrorMessage = "Name cannot be longer than 50 characters")]
        public string Name { get; set; } = string.Empty;
    }
} 