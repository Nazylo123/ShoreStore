using AutoMapper;
using ShoeStore.API.DTOs;
using ShoeStore.API.Models;
using ShoeStore.API.Repositories;

namespace ShoeStore.API.Services
{
    public class CartService : ICartService
    {
        private readonly ICartRepository _cartRepository;
        private readonly IProductRepository _productRepository;
        private readonly IOrderService _orderService;
        private readonly IMapper _mapper;

        public CartService(
            ICartRepository cartRepository,
            IProductRepository productRepository,
            IOrderService orderService,
            IMapper mapper)
        {
            _cartRepository = cartRepository;
            _productRepository = productRepository;
            _orderService = orderService;
            _mapper = mapper;
        }

        public async Task<CartResponseDTO?> GetCartAsync(int customerId)
        {
            var cart = await _cartRepository.GetCartByCustomerIdAsync(customerId);
            if (cart == null) return null;

            var cartDto = _mapper.Map<CartResponseDTO>(cart);
            cartDto.TotalItems = cart.CartItems.Sum(ci => ci.Quantity);
            cartDto.TotalAmount = cart.CartItems.Sum(ci => ci.Product.Price * ci.Quantity);
            cartDto.Items = _mapper.Map<List<CartItemResponseDTO>>(cart.CartItems);

            return cartDto;
        }

        public async Task<CartSummaryDTO> GetCartSummaryAsync(int customerId)
        {
            var cart = await _cartRepository.GetCartByCustomerIdAsync(customerId);
            if (cart == null)
            {
                return new CartSummaryDTO { TotalItems = 0, TotalAmount = 0, ItemCount = 0 };
            }

            var totalItems = cart.CartItems.Sum(ci => ci.Quantity);
            var totalAmount = cart.CartItems.Sum(ci => ci.Product.Price * ci.Quantity);
            var itemCount = cart.CartItems.Count;

            return new CartSummaryDTO
            {
                TotalItems = totalItems,
                TotalAmount = totalAmount,
                ItemCount = itemCount
            };
        }

        public async Task<CartItemResponseDTO> AddToCartAsync(int customerId, AddToCartDTO addToCartDto)
        {
            // Validate product exists and is active
            var product = await _productRepository.GetByIdAsync(addToCartDto.ProductId);
            if (product == null || !product.IsActive)
            {
                throw new InvalidOperationException("Product not found or not available");
            }

            // Get or create cart
            var cart = await _cartRepository.GetCartByCustomerIdAsync(customerId);
            if (cart == null)
            {
                cart = new Cart { CustomerId = customerId };
                cart = await _cartRepository.CreateCartAsync(cart);
            }

            // Check if item already exists in cart
            var existingItem = await _cartRepository.GetCartItemByProductAndSizeAsync(
                cart.Id, addToCartDto.ProductId, addToCartDto.Size);

            CartItem cartItem;
            if (existingItem != null)
            {
                // Update quantity
                existingItem.Quantity += addToCartDto.Quantity;
                if (existingItem.Quantity > 10)
                {
                    throw new InvalidOperationException("Maximum quantity per item is 10");
                }
                cartItem = await _cartRepository.UpdateCartItemAsync(existingItem);
            }
            else
            {
                // Add new item
                cartItem = new CartItem
                {
                    CartId = cart.Id,
                    ProductId = addToCartDto.ProductId,
                    Size = addToCartDto.Size,
                    Quantity = addToCartDto.Quantity
                };
                cartItem = await _cartRepository.AddCartItemAsync(cartItem);
            }

            // Update cart timestamp
            cart.UpdatedAt = DateTime.UtcNow;
            await _cartRepository.UpdateCartItemAsync(cartItem);

            return _mapper.Map<CartItemResponseDTO>(cartItem);
        }

        public async Task<CartItemResponseDTO> UpdateCartItemAsync(int customerId, int cartItemId, UpdateCartItemDTO updateDto)
        {
            var cart = await _cartRepository.GetCartByCustomerIdAsync(customerId);
            if (cart == null)
            {
                throw new InvalidOperationException("Cart not found");
            }

            var cartItem = await _cartRepository.GetCartItemByIdAsync(cartItemId);
            if (cartItem == null || cartItem.CartId != cart.Id)
            {
                throw new InvalidOperationException("Cart item not found");
            }

            cartItem.Quantity = updateDto.Quantity;
            cartItem = await _cartRepository.UpdateCartItemAsync(cartItem);

            // Update cart timestamp
            cart.UpdatedAt = DateTime.UtcNow;

            return _mapper.Map<CartItemResponseDTO>(cartItem);
        }

        public async Task RemoveFromCartAsync(int customerId, int cartItemId)
        {
            var cart = await _cartRepository.GetCartByCustomerIdAsync(customerId);
            if (cart == null)
            {
                throw new InvalidOperationException("Cart not found");
            }

            var cartItem = await _cartRepository.GetCartItemByIdAsync(cartItemId);
            if (cartItem == null || cartItem.CartId != cart.Id)
            {
                throw new InvalidOperationException("Cart item not found");
            }

            await _cartRepository.DeleteCartItemAsync(cartItemId);
        }

        public async Task ClearCartAsync(int customerId)
        {
            var cart = await _cartRepository.GetCartByCustomerIdAsync(customerId);
            if (cart == null)
            {
                throw new InvalidOperationException("Cart not found");
            }

            await _cartRepository.ClearCartAsync(cart.Id);
        }

        public async Task<OrderResponseDTO> CheckoutAsync(int customerId, CheckoutDTO checkoutDto)
        {
            var cart = await _cartRepository.GetCartByCustomerIdAsync(customerId);
            if (cart == null || !cart.CartItems.Any())
            {
                throw new InvalidOperationException("Cart is empty");
            }

            // Create order from cart items
            var orderDetails = cart.CartItems.Select(ci => new CreateOrderDetailDTO
            {
                ProductId = ci.ProductId,
                Size = ci.Size,
                Quantity = ci.Quantity,
                Price = ci.Product.Price
            }).ToList();

            var createOrderDto = new CreateOrderDTO
            {
                CustomerId = customerId,
                ShippingAddress = checkoutDto.ShippingAddress,
                PhoneNumber = checkoutDto.PhoneNumber,
                Notes = checkoutDto.Notes,
                OrderDetails = orderDetails
            };

            // Create order
            var order = await _orderService.CreateOrderAsync(createOrderDto);

            // Clear cart after successful order
            await _cartRepository.ClearCartAsync(cart.Id);

            return order;
        }
    }
} 