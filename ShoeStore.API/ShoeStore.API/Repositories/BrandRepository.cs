using Microsoft.EntityFrameworkCore;
using ShoeStore.API.Data;
using ShoeStore.API.Models;

namespace ShoeStore.API.Repositories
{
    public class BrandRepository : GenericRepository<Brand>, IBrandRepository
    {
        public BrandRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Brand>> GetBrandsWithProductsAsync()
        {
            return await _context.Brands
                .Include(b => b.Products.Where(p => p.IsActive))
                .ToListAsync();
        }

        public async Task<Brand?> GetBrandWithProductsAsync(int id)
        {
            return await _context.Brands
                .Include(b => b.Products.Where(p => p.IsActive))
                .FirstOrDefaultAsync(b => b.Id == id);
        }
    }
}
