using System.ComponentModel.DataAnnotations;

namespace ShoeStore.API.DTOs
{
    public class CategoryResponseDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int ProductCount { get; set; }
        public List<ProductResponseDTO> Products { get; set; } = new();
    }

    public class CreateCategoryDTO
    {
        [Required]
        [StringLength(50, ErrorMessage = "Name cannot be longer than 50 characters")]
        public string Name { get; set; } = string.Empty;
    }

    public class UpdateCategoryDTO
    {
        [Required]
        [StringLength(50, ErrorMessage = "Name cannot be longer than 50 characters")]
        public string Name { get; set; } = string.Empty;
    }
} 