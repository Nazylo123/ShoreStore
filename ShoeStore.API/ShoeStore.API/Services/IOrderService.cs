using ShoeStore.API.DTOs;

namespace ShoeStore.API.Services
{
    public interface IOrderService
    {
        Task<IEnumerable<OrderResponseDTO>> GetAllOrdersAsync();
        Task<OrderResponseDTO?> GetOrderByIdAsync(int id);
        Task<OrderResponseDTO?> GetOrderWithDetailsAsync(int id);
        Task<OrderResponseDTO> CreateOrderAsync(CreateOrderDTO orderDto);
        Task UpdateOrderStatusAsync(int id, string status);
        Task DeleteOrderAsync(int id);
        Task<bool> OrderExistsAsync(int id);
        Task<IEnumerable<OrderResponseDTO>> GetOrdersByCustomerAsync(int customerId);
    }
}
