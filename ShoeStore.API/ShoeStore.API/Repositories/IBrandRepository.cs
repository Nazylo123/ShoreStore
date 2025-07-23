using ShoeStore.API.Models;

namespace ShoeStore.API.Repositories
{
    public interface IBrandRepository : IGenericRepository<Brand>
    {
        Task<IEnumerable<Brand>> GetBrandsWithProductsAsync();
        Task<Brand?> GetBrandWithProductsAsync(int id);
    }
}
