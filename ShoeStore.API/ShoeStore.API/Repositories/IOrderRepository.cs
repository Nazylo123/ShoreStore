using ShoeStore.API.Models;

namespace ShoeStore.API.Repositories
{
    public interface IOrderRepository : IGenericRepository<Order>
    {
        Task<IEnumerable<Order>> GetOrdersWithDetailsAsync();
        Task<Order?> GetOrderWithDetailsAsync(int id);
        Task<IEnumerable<Order>> GetOrdersByCustomerAsync(int customerId);
    }
}
