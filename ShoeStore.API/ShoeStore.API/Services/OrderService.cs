using AutoMapper;
using ShoeStore.API.DTOs;
using ShoeStore.API.Models;
using ShoeStore.API.Repositories;
using ShoeStore.API.Data;
using Microsoft.EntityFrameworkCore;

namespace ShoeStore.API.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IMapper _mapper;
        private readonly AppDbContext _dbContext;

        public OrderService(IOrderRepository orderRepository, IMapper mapper, AppDbContext dbContext)
        {
            _orderRepository = orderRepository;
            _mapper = mapper;
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<OrderResponseDTO>> GetAllOrdersAsync()
        {
            var orders = await _orderRepository.GetOrdersWithDetailsAsync();
            return _mapper.Map<IEnumerable<OrderResponseDTO>>(orders);
        }

        public async Task<OrderResponseDTO?> GetOrderByIdAsync(int id)
        {
            var order = await _orderRepository.GetByIdAsync(id);
            return _mapper.Map<OrderResponseDTO?>(order);
        }

        public async Task<OrderResponseDTO?> GetOrderWithDetailsAsync(int id)
        {
            var order = await _orderRepository.GetOrderWithDetailsAsync(id);
            return _mapper.Map<OrderResponseDTO?>(order);
        }

        public async Task<OrderResponseDTO> CreateOrderAsync(CreateOrderDTO orderDto)
        {
            // Convert DTO to domain model
            var order = _mapper.Map<Order>(orderDto);

            // Gán lại Order cho từng OrderDetail để đảm bảo EF lưu đúng quan hệ
            if (order.OrderDetails != null)
            {
                foreach (var detail in order.OrderDetails)
                {
                    detail.Order = order;
                }
            }

            // Business logic validation
            if (order.CustomerId <= 0)
                throw new ArgumentException("Customer is required");

            if (order.OrderDetails == null || !order.OrderDetails.Any())
                throw new ArgumentException("Order must have at least one item");

            // Validate shipping information
            if (string.IsNullOrWhiteSpace(orderDto.ShippingAddress))
                throw new ArgumentException("Shipping address is required");

            if (string.IsNullOrWhiteSpace(orderDto.PhoneNumber))
                throw new ArgumentException("Phone number is required");

            // Validate order details
            foreach (var detail in order.OrderDetails)
            {
                if (detail.ProductId <= 0)
                    throw new ArgumentException("Product is required for each order detail");

                if (detail.Quantity <= 0)
                    throw new ArgumentException("Quantity must be greater than zero");

                if (detail.Price <= 0)
                    throw new ArgumentException("Price must be greater than zero");

                if (detail.Size <= 0)
                    throw new ArgumentException("Size is required");
            }

            // Calculate total amount
            order.TotalAmount = CalculateOrderTotal(order);

            // Save to database
            var createdOrder = await _orderRepository.AddAsync(order);

            // Trừ stock cho từng sản phẩm variant (theo size)
            foreach (var detail in order.OrderDetails)
            {
                // Lấy variant theo ProductId và Size
                var variant = await _dbContext.ProductVariants
                    .FirstOrDefaultAsync(v => v.ProductId == detail.ProductId && v.Size == detail.Size);
                if (variant != null)
                {
                    variant.Stock -= detail.Quantity;
                    if (variant.Stock < 0) variant.Stock = 0;
                    await _dbContext.SaveChangesAsync();
                }
            }
            
            // Get the order with details for response
            var orderWithDetails = await _orderRepository.GetOrderWithDetailsAsync(createdOrder.Id);
            return _mapper.Map<OrderResponseDTO>(orderWithDetails);
        }

        public async Task UpdateOrderStatusAsync(int id, string status)
        {
            var order = await _orderRepository.GetByIdAsync(id);
            if (order == null)
                throw new ArgumentException("Order not found");

            // Validate status
            var validStatuses = new[] { "Pending", "Processing", "Shipped", "Delivered", "Cancelled" };
            if (!validStatuses.Contains(status, StringComparer.OrdinalIgnoreCase))
                throw new ArgumentException("Invalid order status");

            // Business rules for status changes
            if (order.Status == "Delivered" && status != "Delivered")
                throw new InvalidOperationException("Cannot change status of delivered order");

            if (order.Status == "Cancelled" && status != "Cancelled")
                throw new InvalidOperationException("Cannot change status of cancelled order");

            order.Status = status;
            await _orderRepository.UpdateAsync(order);
        }

        public async Task DeleteOrderAsync(int id)
        {
            var order = await _orderRepository.GetOrderWithDetailsAsync(id);
            if (order == null)
                throw new ArgumentException("Order not found");

            // Business rule: Cannot delete orders that are already shipped or delivered
            if (order.Status == "Shipped" || order.Status == "Delivered")
                throw new InvalidOperationException("Cannot delete orders that are shipped or delivered");

            await _orderRepository.DeleteAsync(order);
        }

        public async Task<bool> OrderExistsAsync(int id)
        {
            return await _orderRepository.ExistsAsync(id);
        }

        public async Task<IEnumerable<OrderResponseDTO>> GetOrdersByCustomerAsync(int customerId)
        {
            var orders = await _orderRepository.GetOrdersByCustomerAsync(customerId);
            return _mapper.Map<IEnumerable<OrderResponseDTO>>(orders);
        }

        private static decimal CalculateOrderTotal(Order order)
        {
            if (order.OrderDetails == null || !order.OrderDetails.Any())
                return 0;

            return order.OrderDetails.Sum(od => od.Price * od.Quantity);
        }
    }
}
