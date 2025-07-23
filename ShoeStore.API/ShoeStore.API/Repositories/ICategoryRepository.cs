using ShoeStore.API.Models;

namespace ShoeStore.API.Repositories
{
    public interface ICategoryRepository : IGenericRepository<Category>
    {
        Task<IEnumerable<Category>> GetCategoriesWithProductsAsync();
        Task<Category?> GetCategoryWithProductsAsync(int id);
    }
}
