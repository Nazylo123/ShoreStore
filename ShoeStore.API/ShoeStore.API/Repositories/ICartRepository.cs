using ShoeStore.API.Models;

namespace ShoeStore.API.Repositories
{
    public interface ICartRepository
    {
        Task<Cart?> GetCartByCustomerIdAsync(int customerId);
        Task<Cart> CreateCartAsync(Cart cart);
        Task<CartItem?> GetCartItemByIdAsync(int cartItemId);
        Task<CartItem?> GetCartItemByProductAndSizeAsync(int cartId, int productId, int size);
        Task<CartItem> AddCartItemAsync(CartItem cartItem);
        Task<CartItem> UpdateCartItemAsync(CartItem cartItem);
        Task DeleteCartItemAsync(int cartItemId);
        Task ClearCartAsync(int cartId);
        Task<bool> CartExistsAsync(int customerId);
        Task<bool> CartItemExistsAsync(int cartItemId);
    }
} 