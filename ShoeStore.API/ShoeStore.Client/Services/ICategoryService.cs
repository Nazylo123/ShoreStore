using ShoeStore.Client.Models;

namespace ShoeStore.Client.Services
{
    public interface ICategoryService
    {
        Task<List<CategoryViewModel>> GetAllCategoriesAsync();
        Task<List<CategoryViewModel>> GetActiveCategoriesAsync();
        Task<CategoryViewModel?> GetCategoryByIdAsync(int id);
    }
} 