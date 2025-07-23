using System.ComponentModel.DataAnnotations;

namespace ShoeStore.API.DTOs
{
    public class ProductResponseDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public string? ImageUrl { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsActive { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public int BrandId { get; set; }
        public string BrandName { get; set; } = string.Empty;
        public List<ProductVariantResponseDTO> Variants { get; set; } = new();
        // Thêm trường tổng stock
        public int TotalStock { get; set; }
    }

    public class CreateProductDTO
    {
        [Required]
        [StringLength(100, ErrorMessage = "Name cannot be longer than 100 characters")]
        public string Name { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Description cannot be longer than 500 characters")]
        public string? Description { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than zero")]
        public decimal Price { get; set; }

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Stock quantity must be non-negative")]
        public int StockQuantity { get; set; }

        public string? ImageUrl { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Category is required")]
        public int CategoryId { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Brand is required")]
        public int BrandId { get; set; }

        public bool IsActive { get; set; } = true;

        // Thêm property nhận danh sách variant khi tạo sản phẩm
        public List<CreateProductVariantDTO> Variants { get; set; } = new();
    }

    public class CreateProductVariantDTO
    {
        [Required]
        public int Size { get; set; }
        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Stock must be non-negative")]
        public int Stock { get; set; }
    }

    public class UpdateProductDTO
    {
        [Required]
        [StringLength(100, ErrorMessage = "Name cannot be longer than 100 characters")]
        public string Name { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Description cannot be longer than 500 characters")]
        public string? Description { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than zero")]
        public decimal Price { get; set; }

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Stock quantity must be non-negative")]
        public int StockQuantity { get; set; }

        public string? ImageUrl { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Category is required")]
        public int CategoryId { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Brand is required")]
        public int BrandId { get; set; }

        public bool IsActive { get; set; }

        // Thêm property nhận danh sách variant khi cập nhật sản phẩm
        public List<CreateProductVariantDTO> Variants { get; set; } = new();
    }

    public class ProductVariantResponseDTO
    {
        public int Id { get; set; }
        public int Size { get; set; }
        public int Stock { get; set; }
    }
} 