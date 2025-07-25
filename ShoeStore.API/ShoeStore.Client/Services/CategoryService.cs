using System.Text.Json;
using ShoeStore.Client.Models;

namespace ShoeStore.Client.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public CategoryService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            // _httpClient.BaseAddress = new Uri(_configuration["ApiBaseUrl"] ?? "https://localhost:7001/api/");
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

        public async Task<List<CategoryViewModel>> GetActiveCategoriesAsync()
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

        public async Task<CategoryViewModel?> GetCategoryByIdAsync(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"Categories/{id}");
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var category = JsonSerializer.Deserialize<CategoryViewModel>(content, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                    return category;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting category {id}: {ex.Message}");
            }
            return null;
        }
    }
} 