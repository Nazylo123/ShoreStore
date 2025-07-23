using Microsoft.EntityFrameworkCore;
using ShoeStore.API.Data;
using ShoeStore.API.Models;

namespace ShoeStore.API.Repositories
{
    public class CategoryRepository : GenericRepository<Category>, ICategoryRepository
    {
        public CategoryRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Category>> GetCategoriesWithProductsAsync()
        {
            return await _context.Categories
                .Include(c => c.Products.Where(p => p.IsActive))
                .ToListAsync();
        }

        public async Task<Category?> GetCategoryWithProductsAsync(int id)
        {
            return await _context.Categories
                .Include(c => c.Products.Where(p => p.IsActive))
                .FirstOrDefaultAsync(c => c.Id == id);
        }
    }
}
