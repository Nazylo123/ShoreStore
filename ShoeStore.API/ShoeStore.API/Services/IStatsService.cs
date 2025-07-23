using System.Threading.Tasks;
using System.Collections.Generic;

namespace ShoeStore.API.Services
{
    public interface IStatsService
    {
        Task<object> GetOverviewAsync(string? from, string? to, string? status);
        Task<IEnumerable<object>> GetRevenueChartAsync(string? from, string? to);
        Task<IEnumerable<object>> GetOrderChartAsync(string? from, string? to);
        Task<IEnumerable<object>> GetTopProductsAsync(string? from, string? to);
    }
} 