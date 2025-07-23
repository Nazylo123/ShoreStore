using ShoeStore.Client.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ShoeStore.Client.Services
{
    public interface IProductService
    {
        Task<List<ProductViewModel>> GetAllProductsAsync();
        Task<List<ProductViewModel>> GetActiveProductsAsync();
        Task<ProductViewModel?> GetProductByIdAsync(int id);
        Task<List<ProductViewModel>> GetProductsByCategoryAsync(int categoryId);
        Task<List<ProductViewModel>> GetProductsByBrandAsync(int brandId);
        Task<ProductListViewModel> GetProductsWithPaginationAsync(int page = 1, int pageSize = 12, string? searchTerm = null, int? categoryId = null, int? brandId = null, string? sortBy = null);
        Task<IEnumerable<ProductViewModel>> GetAllAsync();
        Task<ProductViewModel> GetByIdAsync(int id);
        Task CreateAsync(ProductViewModel model);
        Task UpdateAsync(ProductViewModel model);
        Task DeleteAsync(int id);
        Task<List<BrandViewModel>> GetAllBrandsAsync();
        Task<List<CategoryViewModel>> GetAllCategoriesAsync();
    }
} 