using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShoeStore.API.Services;
using System.Globalization;
using ShoeStore.API.Data;
using System.Linq;

namespace ShoeStore.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class StatsController : ControllerBase
    {
        private readonly IStatsService _statsService;
        public StatsController(IStatsService statsService)
        {
            _statsService = statsService;
        }

        // GET: api/Stats/Overview?from=2024-01-01&to=2024-12-31&status=Completed
        [HttpGet("Overview")]
        public async Task<IActionResult> GetOverview([FromQuery] string? from, [FromQuery] string? to, [FromQuery] string? status)
        {
            var overview = await _statsService.GetOverviewAsync(from, to, status);
            return Ok(overview);
        }

        // GET: api/Stats/RevenueChart?from=2024-01-01&to=2024-12-31
        [HttpGet("RevenueChart")]
        public async Task<IActionResult> GetRevenueChart([FromQuery] string? from, [FromQuery] string? to)
        {
            var data = await _statsService.GetRevenueChartAsync(from, to);
            return Ok(data);
        }

        // GET: api/Stats/OrderChart?from=2024-01-01&to=2024-12-31
        [HttpGet("OrderChart")]
        public async Task<IActionResult> GetOrderChart([FromQuery] string? from, [FromQuery] string? to)
        {
            var data = await _statsService.GetOrderChartAsync(from, to);
            return Ok(data);
        }

        // GET: api/Stats/TopProducts?from=2024-01-01&to=2024-12-31
        [HttpGet("TopProducts")]
        public async Task<IActionResult> GetTopProducts([FromQuery] string? from, [FromQuery] string? to)
        {
            var data = await _statsService.GetTopProductsAsync(from, to);
            return Ok(data);
        }
    }
} 