using AutoMapper;
using ShoeStore.API.DTOs;
using ShoeStore.API.Models;
using ShoeStore.API.Repositories;

namespace ShoeStore.API.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;

        public ProductService(IProductRepository productRepository, IMapper mapper)
        {
            _productRepository = productRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ProductResponseDTO>> GetAllProductsAsync()
        {
            var products = await _productRepository.GetProductsWithCategoryAndBrandAsync();
            var dtos = _mapper.Map<IEnumerable<ProductResponseDTO>>(products).ToList();
            // Gán tổng stock cho từng sản phẩm
            foreach (var dto in dtos)
            {
                var product = products.FirstOrDefault(p => p.Id == dto.Id);
                dto.TotalStock = product?.Variants?.Sum(v => v.Stock) ?? 0;
            }
            return dtos;
        }

        public async Task<IEnumerable<ProductResponseDTO>> GetActiveProductsAsync()
        {
            var products = await _productRepository.GetActiveProductsAsync();
            var dtos = _mapper.Map<IEnumerable<ProductResponseDTO>>(products).ToList();
            foreach (var dto in dtos)
            {
                var product = products.FirstOrDefault(p => p.Id == dto.Id);
                dto.TotalStock = product?.Variants?.Sum(v => v.Stock) ?? 0;
            }
            return dtos;
        }

        public async Task<ProductResponseDTO?> GetProductByIdAsync(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            var dto = _mapper.Map<ProductResponseDTO?>(product);
            if (dto != null && product != null)
            {
                dto.TotalStock = product.Variants?.Sum(v => v.Stock) ?? 0;
            }
            return dto;
        }

        public async Task<ProductResponseDTO?> GetProductWithDetailsAsync(int id)
        {
            var product = await _productRepository.GetProductWithDetailsAsync(id);
            var dto = _mapper.Map<ProductResponseDTO?>(product);
            if (dto != null && product != null)
            {
                dto.TotalStock = product.Variants?.Sum(v => v.Stock) ?? 0;
            }
            return dto;
        }

        public async Task<IEnumerable<ProductResponseDTO>> GetProductsByCategoryAsync(int categoryId)
        {
            var products = await _productRepository.GetProductsByCategoryAsync(categoryId);
            var dtos = _mapper.Map<IEnumerable<ProductResponseDTO>>(products).ToList();
            foreach (var dto in dtos)
            {
                var product = products.FirstOrDefault(p => p.Id == dto.Id);
                dto.TotalStock = product?.Variants?.Sum(v => v.Stock) ?? 0;
            }
            return dtos;
        }

        public async Task<IEnumerable<ProductResponseDTO>> GetProductsByBrandAsync(int brandId)
        {
            var products = await _productRepository.GetProductsByBrandAsync(brandId);
            var dtos = _mapper.Map<IEnumerable<ProductResponseDTO>>(products).ToList();
            foreach (var dto in dtos)
            {
                var product = products.FirstOrDefault(p => p.Id == dto.Id);
                dto.TotalStock = product?.Variants?.Sum(v => v.Stock) ?? 0;
            }
            return dtos;
        }

        public async Task<ProductResponseDTO> CreateProductAsync(CreateProductDTO productDto)
        {
            // Business logic validation
            if (string.IsNullOrWhiteSpace(productDto.Name))
                throw new ArgumentException("Product name is required");

            if (productDto.Price <= 0)
                throw new ArgumentException("Product price must be greater than zero");

            if (productDto.StockQuantity < 0)
                throw new ArgumentException("Stock quantity must be non-negative");

            if (productDto.CategoryId <= 0)
                throw new ArgumentException("Category is required");

            if (productDto.BrandId <= 0)
                throw new ArgumentException("Brand is required");

            // Validate variants
            if (productDto.Variants == null || !productDto.Variants.Any())
                throw new ArgumentException("Phải nhập ít nhất 1 size và số lượng tồn kho!");
            foreach (var variant in productDto.Variants)
            {
                if (variant.Size <= 0) throw new ArgumentException("Size phải lớn hơn 0");
                if (variant.Stock < 0) throw new ArgumentException("Số lượng tồn kho phải >= 0");
            }

            // Map DTO to domain model
            var product = _mapper.Map<Product>(productDto);

            // Save to database
            var createdProduct = await _productRepository.AddAsync(product);
            
            // Get the product with details for response
            var productWithDetails = await _productRepository.GetProductWithDetailsAsync(createdProduct.Id);
            return _mapper.Map<ProductResponseDTO>(productWithDetails);
        }

        public async Task UpdateProductAsync(int id, UpdateProductDTO productDto)
        {
            // Business logic validation
            if (string.IsNullOrWhiteSpace(productDto.Name))
                throw new ArgumentException("Product name is required");

            if (productDto.Price <= 0)
                throw new ArgumentException("Product price must be greater than zero");

            if (productDto.StockQuantity < 0)
                throw new ArgumentException("Stock quantity must be non-negative");

            if (productDto.CategoryId <= 0)
                throw new ArgumentException("Category is required");

            if (productDto.BrandId <= 0)
                throw new ArgumentException("Brand is required");

            // Validate variants
            if (productDto.Variants == null || !productDto.Variants.Any())
                throw new ArgumentException("Phải nhập ít nhất 1 size và số lượng tồn kho!");
            foreach (var variant in productDto.Variants)
            {
                if (variant.Size <= 0) throw new ArgumentException("Size phải lớn hơn 0");
                if (variant.Stock < 0) throw new ArgumentException("Số lượng tồn kho phải >= 0");
            }

            // Get existing product
            var existingProduct = await _productRepository.GetProductWithDetailsAsync(id);
            if (existingProduct == null)
                throw new ArgumentException("Product not found");

            // Map DTO to existing product (cập nhật thông tin chung)
            _mapper.Map(productDto, existingProduct);

            // Cập nhật variants: xóa hết variants cũ, thêm lại variants mới
            existingProduct.Variants.Clear();
            foreach (var variantDto in productDto.Variants)
            {
                existingProduct.Variants.Add(new ProductVariant
                {
                    Size = variantDto.Size,
                    Stock = variantDto.Stock
                });
            }

            await _productRepository.UpdateAsync(existingProduct);
        }

        public async Task DeleteProductAsync(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
                throw new ArgumentException("Product not found");

            // Soft delete - set IsActive to false instead of hard delete
            product.IsActive = false;
            await _productRepository.UpdateAsync(product);
        }

        public async Task<bool> ProductExistsAsync(int id)
        {
            return await _productRepository.ExistsAsync(id);
        }
    }
}
