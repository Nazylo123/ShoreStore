using System.Text.Json;
using ShoeStore.Client.Models;

namespace ShoeStore.Client.Services
{
    public class BrandService : IBrandService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public BrandService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _httpClient.BaseAddress = new Uri(_configuration["ApiBaseUrl"] ?? "https://localhost:7001/api/");
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

        public async Task<List<BrandViewModel>> GetActiveBrandsAsync()
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

        public async Task<BrandViewModel?> GetBrandByIdAsync(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"Brands/{id}");
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var brand = JsonSerializer.Deserialize<BrandViewModel>(content, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                    return brand;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting brand {id}: {ex.Message}");
            }
            return null;
        }
    }
} 