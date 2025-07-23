using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace ShoeStore.Client.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminDashboardController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        public AdminDashboardController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        public async Task<IActionResult> Index(string from = null, string to = null, string status = null)
        {
            var token = HttpContext.Session.GetString("JwtToken");
            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var apiBase = _configuration["ApiBaseUrl"];
            var overviewUrl = $"{apiBase}Stats/Overview?from={from}&to={to}&status={status}";
            var revenueUrl = $"{apiBase}Stats/RevenueChart?from={from}&to={to}";
            var orderUrl = $"{apiBase}Stats/OrderChart?from={from}&to={to}";
            var topProductsUrl = $"{apiBase}Stats/TopProducts?from={from}&to={to}";

            var overview = await (await client.GetAsync(overviewUrl)).Content.ReadAsStringAsync();
            var revenue = await (await client.GetAsync(revenueUrl)).Content.ReadAsStringAsync();
            var orders = await (await client.GetAsync(orderUrl)).Content.ReadAsStringAsync();
            var topProducts = await (await client.GetAsync(topProductsUrl)).Content.ReadAsStringAsync();

            ViewBag.Overview = overview;
            ViewBag.Revenue = revenue;
            ViewBag.Orders = orders;
            ViewBag.TopProducts = topProducts;
            ViewBag.From = from;
            ViewBag.To = to;
            ViewBag.Status = status;
            return View();
        }
    }
} 