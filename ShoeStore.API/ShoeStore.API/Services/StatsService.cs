using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using ShoeStore.API.Data;
using Microsoft.EntityFrameworkCore;

namespace ShoeStore.API.Services
{
    public class StatsService : IStatsService
    {
        private readonly AppDbContext _context;
        public StatsService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<object> GetOverviewAsync(string? from, string? to, string? status)
        {
            // TODO: Parse date, filter by status
            var orders = await _context.Orders.Include(o => o.OrderDetails).ToListAsync();
            var totalRevenue = orders.Where(o => o.Status == "Delivered").Sum(o => o.TotalAmount);
            var totalOrders = orders.Count;
            var totalProductsSold = orders.SelectMany(o => o.OrderDetails).Sum(i => i.Quantity);
            var totalCustomers = await _context.Customers.CountAsync();
            return new { totalRevenue, totalOrders, totalProductsSold, totalCustomers };
        }

        public async Task<IEnumerable<object>> GetRevenueChartAsync(string? from, string? to)
        {
            // TODO: Group by day/month, filter by date
            var data = await _context.Orders.Where(o => o.Status == "Delivered")
                .GroupBy(o => o.OrderDate.Date)
                .Select(g => new { date = g.Key, revenue = g.Sum(x => x.TotalAmount) })
                .OrderBy(x => x.date)
                .ToListAsync();
            return data;
        }

        public async Task<IEnumerable<object>> GetOrderChartAsync(string? from, string? to)
        {
            var data = await _context.Orders
                .GroupBy(o => o.OrderDate.Date)
                .Select(g => new { date = g.Key, orders = g.Count() })
                .OrderBy(x => x.date)
                .ToListAsync();
            return data;
        }

        public async Task<IEnumerable<object>> GetTopProductsAsync(string? from, string? to)
        {
            var data = await _context.OrderDetails
                .Include(od => od.Product)
                .GroupBy(i => new { i.ProductId, i.Product.Name })
                .Select(g => new { productId = g.Key.ProductId, productName = g.Key.Name, quantity = g.Sum(x => x.Quantity) })
                .OrderByDescending(x => x.quantity)
                .Take(10)
                .ToListAsync();
            return data;
        }
    }
} 