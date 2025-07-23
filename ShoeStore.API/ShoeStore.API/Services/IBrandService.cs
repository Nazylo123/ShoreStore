using ShoeStore.API.DTOs;

namespace ShoeStore.API.Services
{
    public interface IBrandService
    {
        Task<IEnumerable<BrandResponseDTO>> GetAllBrandsAsync();
        Task<IEnumerable<BrandResponseDTO>> GetBrandsWithProductsAsync();
        Task<BrandResponseDTO?> GetBrandByIdAsync(int id);
        Task<BrandResponseDTO?> GetBrandWithProductsAsync(int id);
        Task<BrandResponseDTO> CreateBrandAsync(CreateBrandDTO brandDto);
        Task UpdateBrandAsync(int id, UpdateBrandDTO brandDto);
        Task DeleteBrandAsync(int id);
        Task<bool> BrandExistsAsync(int id);
    }
}
