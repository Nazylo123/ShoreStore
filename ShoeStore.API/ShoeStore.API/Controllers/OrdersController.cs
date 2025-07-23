using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShoeStore.API.DTOs;
using ShoeStore.API.Services;
using ShoeStore.API.Data;
using ShoeStore.API.Models;

namespace ShoeStore.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrdersController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        // GET: api/Orders
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<OrderResponseDTO>>> GetOrders()
        {
            try
            {
                var orders = await _orderService.GetAllOrdersAsync();
                return Ok(orders);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // GET: api/Orders/5
        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<OrderResponseDTO>> GetOrder(int id)
        {
            try
            {
                var order = await _orderService.GetOrderWithDetailsAsync(id);

                if (order == null)
                {
                    return NotFound($"Order with ID {id} not found");
                }

                // Check if user is authorized to view this order
                var userRole = User.Claims.FirstOrDefault(c => c.Type == "role")?.Value
                    ?? User.Claims.FirstOrDefault(c => c.Type == "Role")?.Value
                    ?? User.Claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.Role)?.Value;
                var customerId = User.Claims.FirstOrDefault(c => c.Type == "CustomerId")?.Value;

                if (userRole != "Admin" && (customerId == null || order.CustomerId != int.Parse(customerId)))
                {
                    return Forbid();
                }

                return Ok(order);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // GET: api/Orders/customer/5
        [HttpGet("customer/{customerId}")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<OrderResponseDTO>>> GetOrdersByCustomer(int customerId)
        {
            try
            {
                // Check if user is authorized to view these orders
                var userRole = User.Claims.FirstOrDefault(c => c.Type == "role")?.Value;
                var userCustomerId = User.Claims.FirstOrDefault(c => c.Type == "CustomerId")?.Value;

                if (userRole != "Admin" && (userCustomerId == null || customerId != int.Parse(userCustomerId)))
                {
                    return Forbid();
                }

                var orders = await _orderService.GetOrdersByCustomerAsync(customerId);
                return Ok(orders);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // POST: api/Orders
        [HttpPost]
        [Authorize(Roles = "Customer")]
        public async Task<ActionResult<OrderResponseDTO>> PostOrder([FromBody] CreateOrderDTO orderDto)
        {
            try
            {
                // Set customer ID from token
                var customerId = User.Claims.FirstOrDefault(c => c.Type == "CustomerId")?.Value;
                if (customerId != null)
                {
                    orderDto.CustomerId = int.Parse(customerId);
                }

                var createdOrder = await _orderService.CreateOrderAsync(orderDto);
                return CreatedAtAction(nameof(GetOrder), new { id = createdOrder.Id }, createdOrder);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // PUT: api/Orders/5/status
        [HttpPut("{id}/status")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateOrderStatus(int id, [FromBody] UpdateOrderStatusDTO statusDto)
        {
            try
            {
                await _orderService.UpdateOrderStatusAsync(id, statusDto.Status);
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // DELETE: api/Orders/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            try
            {
                await _orderService.DeleteOrderAsync(id);
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
} 