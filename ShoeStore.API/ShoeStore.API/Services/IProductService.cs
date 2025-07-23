using ShoeStore.API.DTOs;

namespace ShoeStore.API.Services
{
    public interface IProductService
    {
        Task<IEnumerable<ProductResponseDTO>> GetAllProductsAsync();
        Task<IEnumerable<ProductResponseDTO>> GetActiveProductsAsync();
        Task<ProductResponseDTO?> GetProductByIdAsync(int id);
        Task<ProductResponseDTO?> GetProductWithDetailsAsync(int id);
        Task<IEnumerable<ProductResponseDTO>> GetProductsByCategoryAsync(int categoryId);
        Task<IEnumerable<ProductResponseDTO>> GetProductsByBrandAsync(int brandId);
        Task<ProductResponseDTO> CreateProductAsync(CreateProductDTO productDto);
        Task UpdateProductAsync(int id, UpdateProductDTO productDto);
        Task DeleteProductAsync(int id);
        Task<bool> ProductExistsAsync(int id);
    }
}
