using ShoeStore.Client.Models;

namespace ShoeStore.Client.Services
{
    public interface IBrandService
    {
        Task<List<BrandViewModel>> GetAllBrandsAsync();
        Task<List<BrandViewModel>> GetActiveBrandsAsync();
        Task<BrandViewModel?> GetBrandByIdAsync(int id);
    }
} 