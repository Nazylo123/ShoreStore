using ShoeStore.API.DTOs;

namespace ShoeStore.API.Services
{
    public interface ICartService
    {
        Task<CartResponseDTO?> GetCartAsync(int customerId);
        Task<CartSummaryDTO> GetCartSummaryAsync(int customerId);
        Task<CartItemResponseDTO> AddToCartAsync(int customerId, AddToCartDTO addToCartDto);
        Task<CartItemResponseDTO> UpdateCartItemAsync(int customerId, int cartItemId, UpdateCartItemDTO updateDto);
        Task RemoveFromCartAsync(int customerId, int cartItemId);
        Task ClearCartAsync(int customerId);
        Task<OrderResponseDTO> CheckoutAsync(int customerId, CheckoutDTO checkoutDto);
    }
} 