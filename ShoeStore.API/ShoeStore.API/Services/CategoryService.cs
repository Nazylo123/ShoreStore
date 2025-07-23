using AutoMapper;
using ShoeStore.API.DTOs;
using ShoeStore.API.Models;
using ShoeStore.API.Repositories;

namespace ShoeStore.API.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMapper _mapper;

        public CategoryService(ICategoryRepository categoryRepository, IMapper mapper)
        {
            _categoryRepository = categoryRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<CategoryResponseDTO>> GetAllCategoriesAsync()
        {
            var categories = await _categoryRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<CategoryResponseDTO>>(categories);
        }

        public async Task<IEnumerable<CategoryResponseDTO>> GetCategoriesWithProductsAsync()
        {
            var categories = await _categoryRepository.GetCategoriesWithProductsAsync();
            return _mapper.Map<IEnumerable<CategoryResponseDTO>>(categories);
        }

        public async Task<CategoryResponseDTO?> GetCategoryByIdAsync(int id)
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            return _mapper.Map<CategoryResponseDTO?>(category);
        }

        public async Task<CategoryResponseDTO?> GetCategoryWithProductsAsync(int id)
        {
            var category = await _categoryRepository.GetCategoryWithProductsAsync(id);
            return _mapper.Map<CategoryResponseDTO?>(category);
        }

        public async Task<CategoryResponseDTO> CreateCategoryAsync(CreateCategoryDTO categoryDto)
        {
            // Business logic validation
            if (string.IsNullOrWhiteSpace(categoryDto.Name))
                throw new ArgumentException("Category name is required");

            // Check if category name already exists
            var existingCategories = await _categoryRepository.FindAsync(c => c.Name.ToLower() == categoryDto.Name.ToLower());
            if (existingCategories.Any())
                throw new ArgumentException("Category name already exists");

            var category = _mapper.Map<Category>(categoryDto);
            var createdCategory = await _categoryRepository.AddAsync(category);
            
            // Get the category with products for response
            var categoryWithProducts = await _categoryRepository.GetCategoryWithProductsAsync(createdCategory.Id);
            return _mapper.Map<CategoryResponseDTO>(categoryWithProducts);
        }

        public async Task UpdateCategoryAsync(int id, UpdateCategoryDTO categoryDto)
        {
            // Business logic validation
            if (string.IsNullOrWhiteSpace(categoryDto.Name))
                throw new ArgumentException("Category name is required");

            // Check if category name already exists (excluding current category)
            var existingCategories = await _categoryRepository.FindAsync(c => 
                c.Name.ToLower() == categoryDto.Name.ToLower() && c.Id != id);
            if (existingCategories.Any())
                throw new ArgumentException("Category name already exists");

            var existingCategory = await _categoryRepository.GetByIdAsync(id);
            if (existingCategory == null)
                throw new ArgumentException("Category not found");

            // Map DTO to existing category
            _mapper.Map(categoryDto, existingCategory);

            await _categoryRepository.UpdateAsync(existingCategory);
        }

        public async Task DeleteCategoryAsync(int id)
        {
            var category = await _categoryRepository.GetCategoryWithProductsAsync(id);
            if (category == null)
                throw new ArgumentException("Category not found");

            // Check if category has products
            if (category.Products != null && category.Products.Any())
                throw new InvalidOperationException("Cannot delete category that has products");

            await _categoryRepository.DeleteAsync(category);
        }

        public async Task<bool> CategoryExistsAsync(int id)
        {
            return await _categoryRepository.ExistsAsync(id);
        }
    }
}
