using System.ComponentModel.DataAnnotations;

namespace ShoeStore.Client.Models
{
    public class ProductViewModel
    {
        public int Id { get; set; }
        
        [Required(ErrorMessage = "Vui lòng nhập tên sản phẩm")]
        [StringLength(100, ErrorMessage = "Tên sản phẩm không được quá 100 ký tự")]
        public string Name { get; set; } = string.Empty;
        
        [StringLength(500, ErrorMessage = "Mô tả không được quá 500 ký tự")]
        public string? Description { get; set; }
        
        [Required(ErrorMessage = "Vui lòng nhập giá sản phẩm")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Giá sản phẩm phải lớn hơn 0")]
        public decimal Price { get; set; }
        
        [Required(ErrorMessage = "Vui lòng nhập số lượng tồn kho")]
        [Range(0, int.MaxValue, ErrorMessage = "Số lượng tồn kho phải lớn hơn hoặc bằng 0")]
        public int StockQuantity { get; set; }
        
        public string? ImageUrl { get; set; }
        
        public DateTime CreatedAt { get; set; }
        
        public bool IsActive { get; set; }
        
        [Required(ErrorMessage = "Vui lòng chọn danh mục")]
        public int CategoryId { get; set; }
        
        public string CategoryName { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Vui lòng chọn thương hiệu")]
        public int BrandId { get; set; }
        
        public string BrandName { get; set; } = string.Empty;
        
        public List<ProductVariantViewModel> Variants { get; set; } = new();
        
        public string FormattedPrice => Price.ToString("N0") + " VNĐ";
        // Tổng tồn kho (theo size)
        public int TotalStock { get; set; }
    }

    public class ProductVariantViewModel
    {
        public int Id { get; set; }
        public int Size { get; set; }
        public int Stock { get; set; }
    }

    public class ProductListViewModel
    {
        public List<ProductViewModel> Products { get; set; } = new();
        public int CurrentPage { get; set; } = 1;
        public int TotalPages { get; set; }
        public int PageSize { get; set; } = 12;
        public int TotalItems { get; set; }
        public string? SearchTerm { get; set; }
        public int? CategoryId { get; set; }
        public int? BrandId { get; set; }
        public string? SortBy { get; set; }
    }
} 