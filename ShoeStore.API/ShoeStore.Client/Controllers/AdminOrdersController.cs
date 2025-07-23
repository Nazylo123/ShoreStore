using Microsoft.AspNetCore.Mvc;
using ShoeStore.Client.Models;
using System.Net.Http.Headers;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using System.Text;

namespace ShoeStore.Client.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminOrdersController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public AdminOrdersController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        // GET: /AdminOrders
        public async Task<IActionResult> Index(string? status = null)
        {
            var token = HttpContext.Session.GetString("JwtToken");
            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            string apiUrl = string.IsNullOrEmpty(status)
                ? $"{_configuration["ApiBaseUrl"]}Orders"
                : $"{_configuration["ApiBaseUrl"]}Orders/status/{status}";
            var response = await client.GetAsync(apiUrl);
            if (!response.IsSuccessStatusCode)
            {
                TempData["ErrorMessage"] = "Không thể lấy danh sách đơn hàng.";
                return View(new List<OrderViewModel>());
            }
            var json = await response.Content.ReadAsStringAsync();
            var orders = JsonSerializer.Deserialize<List<OrderApiResponse>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new List<OrderApiResponse>();
            var viewModels = orders.Select(o => new OrderViewModel
            {
                Id = o.Id,
                CustomerId = o.CustomerId,
                CustomerName = o.CustomerName,
                Phone = o.CustomerPhone,
                Address = o.ShippingAddress,
                TotalAmount = o.TotalAmount,
                Status = o.Status,
                OrderDate = o.OrderDate,
                Items = o.OrderDetails.Select(od => new CartItemViewModel
                {
                    ProductId = od.ProductId,
                    ProductName = od.ProductName,
                    ProductImage = od.ProductImageUrl,
                    Price = od.Price,
                    Size = od.Size,
                    Quantity = od.Quantity
                }).ToList()
            }).ToList();
            return View(viewModels);
        }

        // GET: /AdminOrders/Detail/5
        public async Task<IActionResult> Detail(int id)
        {
            var token = HttpContext.Session.GetString("JwtToken");
            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var apiUrl = $"{_configuration["ApiBaseUrl"]}Orders/{id}";
            var response = await client.GetAsync(apiUrl);
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                TempData["ErrorMessage"] = $"Không thể lấy chi tiết đơn hàng. Lỗi: {error}";
                return RedirectToAction("Index");
            }
            var json = await response.Content.ReadAsStringAsync();
            var o = JsonSerializer.Deserialize<OrderApiResponse>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            if (o == null)
            {
                TempData["ErrorMessage"] = "Đơn hàng không tồn tại.";
                return RedirectToAction("Index");
            }
            var viewModel = new OrderViewModel
            {
                Id = o.Id,
                CustomerId = o.CustomerId,
                CustomerName = o.CustomerName,
                Phone = o.CustomerPhone,
                Address = o.ShippingAddress,
                TotalAmount = o.TotalAmount,
                Status = o.Status,
                OrderDate = o.OrderDate,
                Items = o.OrderDetails.Select(od => new CartItemViewModel
                {
                    ProductId = od.ProductId,
                    ProductName = od.ProductName,
                    ProductImage = od.ProductImageUrl,
                    Price = od.Price,
                    Size = od.Size,
                    Quantity = od.Quantity
                }).ToList()
            };
            return View(viewModel);
        }

        // POST: /AdminOrders/UpdateStatus
        [HttpPost]
        public async Task<IActionResult> UpdateStatus(int id, string status)
        {
            var token = HttpContext.Session.GetString("JwtToken");
            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var apiUrl = $"{_configuration["ApiBaseUrl"]}Orders/{id}/status";
            var body = JsonSerializer.Serialize(new { status });
            var content = new StringContent(body, Encoding.UTF8, "application/json");
            var response = await client.PutAsync(apiUrl, content);
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                TempData["ErrorMessage"] = $"Cập nhật trạng thái thất bại: {error}";
            }
            return RedirectToAction("Detail", new { id });
        }

        // DTO để deserialize từ API
        private class OrderApiResponse
        {
            public int Id { get; set; }
            public int CustomerId { get; set; }
            public string CustomerName { get; set; } = string.Empty;
            public string CustomerPhone { get; set; } = string.Empty;
            public DateTime OrderDate { get; set; }
            public decimal TotalAmount { get; set; }
            public string Status { get; set; } = string.Empty;
            public string ShippingAddress { get; set; } = string.Empty;
            public List<OrderDetailApiResponse> OrderDetails { get; set; } = new();
        }
        private class OrderDetailApiResponse
        {
            public int Id { get; set; }
            public int ProductId { get; set; }
            public string ProductName { get; set; } = string.Empty;
            public string ProductImageUrl { get; set; } = string.Empty;
            public int Size { get; set; }
            public int Quantity { get; set; }
            public decimal Price { get; set; }
            public decimal Subtotal { get; set; }
        }
    }
} 