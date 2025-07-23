using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShoeStore.API.DTOs;
using ShoeStore.API.Services;

namespace ShoeStore.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Customer")]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;

        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        /// <summary>
        /// Get customer's shopping cart
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<CartResponseDTO>> GetCart([FromQuery] int customerId)
        {
            try
            {
                // Verify user can only access their own cart
                var userCustomerId = User.Claims.FirstOrDefault(c => c.Type == "CustomerId")?.Value;
                if (userCustomerId == null || int.Parse(userCustomerId) != customerId)
                {
                    return Forbid();
                }

                var cart = await _cartService.GetCartAsync(customerId);
                if (cart == null)
                {
                    return NotFound("Cart not found");
                }

                return Ok(cart);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Get cart summary (total items, total amount, item count)
        /// </summary>
        [HttpGet("summary")]
        public async Task<ActionResult<CartSummaryDTO>> GetCartSummary([FromQuery] int customerId)
        {
            try
            {
                // Verify user can only access their own cart
                var userCustomerId = User.Claims.FirstOrDefault(c => c.Type == "CustomerId")?.Value;
                if (userCustomerId == null || int.Parse(userCustomerId) != customerId)
                {
                    return Forbid();
                }

                var summary = await _cartService.GetCartSummaryAsync(customerId);
                return Ok(summary);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Add product to cart
        /// </summary>
        [HttpPost("add")]
        public async Task<ActionResult<CartItemResponseDTO>> AddToCart([FromQuery] int customerId, [FromBody] AddToCartDTO addToCartDto)
        {
            try
            {
                // Verify user can only add to their own cart
                var userCustomerId = User.Claims.FirstOrDefault(c => c.Type == "CustomerId")?.Value;
                if (userCustomerId == null || int.Parse(userCustomerId) != customerId)
                {
                    return Forbid();
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var cartItem = await _cartService.AddToCartAsync(customerId, addToCartDto);
                return CreatedAtAction(nameof(GetCart), new { customerId }, cartItem);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while adding item to cart");
            }
        }

        /// <summary>
        /// Update cart item quantity
        /// </summary>
        [HttpPut("items/{cartItemId}")]
        public async Task<ActionResult<CartItemResponseDTO>> UpdateCartItem(
            [FromQuery] int customerId, 
            int cartItemId, 
            [FromBody] UpdateCartItemDTO updateDto)
        {
            try
            {
                // Verify user can only update their own cart
                var userCustomerId = User.Claims.FirstOrDefault(c => c.Type == "CustomerId")?.Value;
                if (userCustomerId == null || int.Parse(userCustomerId) != customerId)
                {
                    return Forbid();
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var cartItem = await _cartService.UpdateCartItemAsync(customerId, cartItemId, updateDto);
                return Ok(cartItem);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while updating cart item");
            }
        }

        /// <summary>
        /// Remove item from cart
        /// </summary>
        [HttpDelete("items/{cartItemId}")]
        public async Task<ActionResult> RemoveFromCart([FromQuery] int customerId, int cartItemId)
        {
            try
            {
                // Verify user can only remove from their own cart
                var userCustomerId = User.Claims.FirstOrDefault(c => c.Type == "CustomerId")?.Value;
                if (userCustomerId == null || int.Parse(userCustomerId) != customerId)
                {
                    return Forbid();
                }

                await _cartService.RemoveFromCartAsync(customerId, cartItemId);
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while removing item from cart");
            }
        }

        /// <summary>
        /// Clear entire cart
        /// </summary>
        [HttpDelete("clear")]
        public async Task<ActionResult> ClearCart([FromQuery] int customerId)
        {
            try
            {
                // Verify user can only clear their own cart
                var userCustomerId = User.Claims.FirstOrDefault(c => c.Type == "CustomerId")?.Value;
                if (userCustomerId == null || int.Parse(userCustomerId) != customerId)
                {
                    return Forbid();
                }

                await _cartService.ClearCartAsync(customerId);
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while clearing cart");
            }
        }

        /// <summary>
        /// Checkout cart and create order
        /// </summary>
        [HttpPost("checkout")]
        public async Task<ActionResult<OrderResponseDTO>> Checkout([FromQuery] int customerId, [FromBody] CheckoutDTO checkoutDto)
        {
            try
            {
                // Verify user can only checkout their own cart
                var userCustomerId = User.Claims.FirstOrDefault(c => c.Type == "CustomerId")?.Value;
                if (userCustomerId == null || int.Parse(userCustomerId) != customerId)
                {
                    return Forbid();
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var order = await _cartService.CheckoutAsync(customerId, checkoutDto);
                return CreatedAtAction(nameof(GetCart), new { customerId }, order);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred during checkout");
            }
        }
    }
} 