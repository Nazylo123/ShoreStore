using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using ShoeStore.Client.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http;
using System.Net.Http.Headers;
using System.Text;

namespace ShoeStore.Client.Services
{
    public class ProductService : IProductService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiBaseUrl;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ProductService(HttpClient httpClient, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = httpClient;
            _apiBaseUrl = configuration["ApiBaseUrl"] ?? "";
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<List<ProductViewModel>> GetAllProductsAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_apiBaseUrl}products");
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var products = JsonSerializer.Deserialize<List<ProductViewModel>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    return products ?? new List<ProductViewModel>();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting products: {ex.Message}");
            }
            return new List<ProductViewModel>();
        }

        public async Task<List<ProductViewModel>> GetActiveProductsAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_apiBaseUrl}products/active");
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var products = JsonSerializer.Deserialize<List<ProductViewModel>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    return products ?? new List<ProductViewModel>();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting active products: {ex.Message}");
            }
            return new List<ProductViewModel>();
        }

        public async Task<ProductViewModel?> GetProductByIdAsync(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_apiBaseUrl}products/{id}");
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var product = JsonSerializer.Deserialize<ProductViewModel>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    return product;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting product {id}: {ex.Message}");
            }
            return null;
        }

        public async Task<List<ProductViewModel>> GetProductsByCategoryAsync(int categoryId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_apiBaseUrl}products/category/{categoryId}");
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var products = JsonSerializer.Deserialize<List<ProductViewModel>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    return products ?? new List<ProductViewModel>();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting products by category {categoryId}: {ex.Message}");
            }
            return new List<ProductViewModel>();
        }

        public async Task<List<ProductViewModel>> GetProductsByBrandAsync(int brandId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_apiBaseUrl}products/brand/{brandId}");
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var products = JsonSerializer.Deserialize<List<ProductViewModel>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    return products ?? new List<ProductViewModel>();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting products by brand {brandId}: {ex.Message}");
            }
            return new List<ProductViewModel>();
        }

        public async Task<ProductListViewModel> GetProductsWithPaginationAsync(int page = 1, int pageSize = 12, string? searchTerm = null, int? categoryId = null, int? brandId = null, string? sortBy = null)
        {
            var products = await GetActiveProductsAsync();
            if (!string.IsNullOrEmpty(searchTerm))
            {
                products = products.Where(p => p.Name.Contains(searchTerm, System.StringComparison.OrdinalIgnoreCase) || (p.Description?.Contains(searchTerm, System.StringComparison.OrdinalIgnoreCase) == true)).ToList();
            }
            if (categoryId.HasValue)
            {
                products = products.Where(p => p.CategoryId == categoryId.Value).ToList();
            }
            if (brandId.HasValue)
            {
                products = products.Where(p => p.BrandId == brandId.Value).ToList();
            }
            if (!string.IsNullOrEmpty(sortBy))
            {
                products = sortBy.ToLower() switch
                {
                    "price_asc" => products.OrderBy(p => p.Price).ToList(),
                    "price_desc" => products.OrderByDescending(p => p.Price).ToList(),
                    "name_asc" => products.OrderBy(p => p.Name).ToList(),
                    "name_desc" => products.OrderByDescending(p => p.Name).ToList(),
                    "newest" => products.OrderByDescending(p => p.CreatedAt).ToList(),
                    _ => products
                };
            }
            var totalItems = products.Count;
            var totalPages = (int)System.Math.Ceiling((double)totalItems / pageSize);
            var pagedProducts = products.Skip((page - 1) * pageSize).Take(pageSize).ToList();
            return new ProductListViewModel
            {
                Products = pagedProducts,
                CurrentPage = page,
                TotalPages = totalPages,
                TotalItems = totalItems,
                CategoryId = categoryId,
                BrandId = brandId
            };
        }

        public async Task<IEnumerable<ProductViewModel>> GetAllAsync()
        {
            var response = await _httpClient.GetAsync($"{_configuration["ApiBaseUrl"]}products");
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<IEnumerable<ProductViewModel>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new List<ProductViewModel>();
        }

        public async Task<ProductViewModel> GetByIdAsync(int id)
        {
            var response = await _httpClient.GetAsync($"{_configuration["ApiBaseUrl"]}products/{id}");
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<ProductViewModel>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        public async Task CreateAsync(ProductViewModel model)
        {
            var token = _httpContextAccessor.HttpContext?.Session.GetString("JwtToken");
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
            var json = JsonSerializer.Serialize(model);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync($"{_configuration["ApiBaseUrl"]}products", content);
            response.EnsureSuccessStatusCode();
        }

        public async Task UpdateAsync(ProductViewModel model)
        {
            var token = _httpContextAccessor.HttpContext?.Session.GetString("JwtToken");
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
            var json = JsonSerializer.Serialize(model);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync($"{_configuration["ApiBaseUrl"]}products/{model.Id}", content);
            response.EnsureSuccessStatusCode();
        }

        public async Task DeleteAsync(int id)
        {
            var token = _httpContextAccessor.HttpContext?.Session.GetString("JwtToken");
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
            var response = await _httpClient.DeleteAsync($"{_configuration["ApiBaseUrl"]}products/{id}");
            response.EnsureSuccessStatusCode();
        }

        public async Task<List<BrandViewModel>> GetAllBrandsAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("Brands");
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var brands = JsonSerializer.Deserialize<List<BrandViewModel>>(content, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                    return brands ?? new List<BrandViewModel>();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting brands: {ex.Message}");
            }
            return new List<BrandViewModel>();
        }

        public async Task<List<CategoryViewModel>> GetAllCategoriesAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("Categories");
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var categories = JsonSerializer.Deserialize<List<CategoryViewModel>>(content, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                    return categories ?? new List<CategoryViewModel>();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting categories: {ex.Message}");
            }
            return new List<CategoryViewModel>();
        }
    }
} 