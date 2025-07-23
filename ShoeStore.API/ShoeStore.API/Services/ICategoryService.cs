using ShoeStore.API.DTOs;

namespace ShoeStore.API.Services
{
    public interface ICategoryService
    {
        Task<IEnumerable<CategoryResponseDTO>> GetAllCategoriesAsync();
        Task<IEnumerable<CategoryResponseDTO>> GetCategoriesWithProductsAsync();
        Task<CategoryResponseDTO?> GetCategoryByIdAsync(int id);
        Task<CategoryResponseDTO?> GetCategoryWithProductsAsync(int id);
        Task<CategoryResponseDTO> CreateCategoryAsync(CreateCategoryDTO categoryDto);
        Task UpdateCategoryAsync(int id, UpdateCategoryDTO categoryDto);
        Task DeleteCategoryAsync(int id);
        Task<bool> CategoryExistsAsync(int id);
    }
}
