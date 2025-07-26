using ShoeStore.API.Models;

namespace ShoeStore.API.Repositories
{
    public interface IProductRepository : IGenericRepository<Product>
    {
        Task<IEnumerable<Product>> GetProductsWithCategoryAndBrandAsync();
        Task<IEnumerable<Product>> GetAllProductsWithCategoryAndBrandAsync();
        Task<Product?> GetProductWithDetailsAsync(int id);
        Task<IEnumerable<Product>> GetProductsByCategoryAsync(int categoryId);
        Task<IEnumerable<Product>> GetProductsByBrandAsync(int brandId);
        Task<IEnumerable<Product>> GetActiveProductsAsync();
    }
}
