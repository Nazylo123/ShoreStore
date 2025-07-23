using AutoMapper;
using ShoeStore.API.DTOs;
using ShoeStore.API.Models;
using ShoeStore.API.Repositories;

namespace ShoeStore.API.Services
{
    public class BrandService : IBrandService
    {
        private readonly IBrandRepository _brandRepository;
        private readonly IMapper _mapper;

        public BrandService(IBrandRepository brandRepository, IMapper mapper)
        {
            _brandRepository = brandRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<BrandResponseDTO>> GetAllBrandsAsync()
        {
            var brands = await _brandRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<BrandResponseDTO>>(brands);
        }

        public async Task<IEnumerable<BrandResponseDTO>> GetBrandsWithProductsAsync()
        {
            var brands = await _brandRepository.GetBrandsWithProductsAsync();
            return _mapper.Map<IEnumerable<BrandResponseDTO>>(brands);
        }

        public async Task<BrandResponseDTO?> GetBrandByIdAsync(int id)
        {
            var brand = await _brandRepository.GetByIdAsync(id);
            return _mapper.Map<BrandResponseDTO?>(brand);
        }

        public async Task<BrandResponseDTO?> GetBrandWithProductsAsync(int id)
        {
            var brand = await _brandRepository.GetBrandWithProductsAsync(id);
            return _mapper.Map<BrandResponseDTO?>(brand);
        }

        public async Task<BrandResponseDTO> CreateBrandAsync(CreateBrandDTO brandDto)
        {
            // Business logic validation
            if (string.IsNullOrWhiteSpace(brandDto.Name))
                throw new ArgumentException("Brand name is required");

            // Check if brand name already exists
            var existingBrands = await _brandRepository.FindAsync(b => b.Name.ToLower() == brandDto.Name.ToLower());
            if (existingBrands.Any())
                throw new ArgumentException("Brand name already exists");

            var brand = _mapper.Map<Brand>(brandDto);
            var createdBrand = await _brandRepository.AddAsync(brand);
            
            // Get the brand with products for response
            var brandWithProducts = await _brandRepository.GetBrandWithProductsAsync(createdBrand.Id);
            return _mapper.Map<BrandResponseDTO>(brandWithProducts);
        }

        public async Task UpdateBrandAsync(int id, UpdateBrandDTO brandDto)
        {
            // Business logic validation
            if (string.IsNullOrWhiteSpace(brandDto.Name))
                throw new ArgumentException("Brand name is required");

            // Check if brand name already exists (excluding current brand)
            var existingBrands = await _brandRepository.FindAsync(b => 
                b.Name.ToLower() == brandDto.Name.ToLower() && b.Id != id);
            if (existingBrands.Any())
                throw new ArgumentException("Brand name already exists");

            var existingBrand = await _brandRepository.GetByIdAsync(id);
            if (existingBrand == null)
                throw new ArgumentException("Brand not found");

            // Map DTO to existing brand
            _mapper.Map(brandDto, existingBrand);

            await _brandRepository.UpdateAsync(existingBrand);
        }

        public async Task DeleteBrandAsync(int id)
        {
            var brand = await _brandRepository.GetBrandWithProductsAsync(id);
            if (brand == null)
                throw new ArgumentException("Brand not found");

            // Check if brand has products
            if (brand.Products != null && brand.Products.Any())
                throw new InvalidOperationException("Cannot delete brand that has products");

            await _brandRepository.DeleteAsync(brand);
        }

        public async Task<bool> BrandExistsAsync(int id)
        {
            return await _brandRepository.ExistsAsync(id);
        }
    }
}
