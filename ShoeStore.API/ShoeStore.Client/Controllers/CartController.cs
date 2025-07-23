using Microsoft.AspNetCore.Mvc;
using ShoeStore.Client.Models;
using ShoeStore.Client.Services;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;

namespace ShoeStore.Client.Controllers
{
    [Authorize]
    public class CartController : Controller
    {
        private readonly IProductService _productService;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public CartController(IProductService productService, IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _productService = productService;
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        private bool IsLoggedIn()
        {
            return !string.IsNullOrEmpty(HttpContext.Session.GetString("Username"));
        }

        // GET: Cart
        public IActionResult Index()
        {
            if (!IsLoggedIn())
            {
                TempData["ErrorMessage"] = "Bạn cần đăng nhập/đăng kí trước.";
                return RedirectToAction("Login", "Auth", new { ReturnUrl = Request.Path });
            }
            var cart = GetCartFromSession();
            return View(cart);
        }

        // POST: Cart/AddToCart
        [HttpPost]
        public async Task<IActionResult> AddToCart(AddToCartViewModel model)
        {
            if (!IsLoggedIn())
            {
                TempData["ErrorMessage"] = "Bạn cần đăng nhập/đăng kí trước.";
                return RedirectToAction("Login", "Auth", new { ReturnUrl = Request.Path });
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var product = await _productService.GetProductByIdAsync(model.ProductId);
            if (product == null)
            {
                return NotFound("Product not found");
            }

            var cart = GetCartFromSession();
            var existingItem = cart.Items.FirstOrDefault(item => 
                item.ProductId == model.ProductId && item.Size == model.Size);

            if (existingItem != null)
            {
                existingItem.Quantity += model.Quantity;
            }
            else
            {
                var variant = product.Variants.FirstOrDefault(v => v.Size == model.Size);
                if (variant == null)
                {
                    return BadRequest("Size not available");
                }

                cart.Items.Add(new CartItemViewModel
                {
                    ProductId = product.Id,
                    ProductName = product.Name,
                    ProductImage = product.ImageUrl ?? "/images/default-shoe.jpg",
                    Price = product.Price,
                    Size = model.Size,
                    Quantity = model.Quantity,
                    Stock = variant.Stock
                });
            }

            SaveCartToSession(cart);
            TempData["SuccessMessage"] = "Sản phẩm đã được thêm vào giỏ hàng!";

            return RedirectToAction(nameof(Index));
        }

        // POST: Cart/UpdateQuantity
        [HttpPost]
        public IActionResult UpdateQuantity(int itemId, int quantity)
        {
            if (!IsLoggedIn())
            {
                TempData["ErrorMessage"] = "Bạn cần đăng nhập/đăng kí trước.";
                return RedirectToAction("Login", "Auth", new { ReturnUrl = Request.Path });
            }
            var cart = GetCartFromSession();
            var item = cart.Items.FirstOrDefault(i => i.Id == itemId);

            if (item == null)
            {
                return NotFound();
            }

            if (quantity <= 0)
            {
                cart.Items.Remove(item);
            }
            else if (quantity <= item.Stock)
            {
                item.Quantity = quantity;
            }
            else
            {
                return BadRequest("Số lượng vượt quá tồn kho");
            }

            SaveCartToSession(cart);
            return Json(new { success = true, totalItems = cart.TotalItems, totalAmount = cart.TotalAmount });
        }

        // POST: Cart/RemoveItem
        [HttpPost]
        public IActionResult RemoveItem(int itemId)
        {
            if (!IsLoggedIn())
            {
                TempData["ErrorMessage"] = "Bạn cần đăng nhập/đăng kí trước.";
                return RedirectToAction("Login", "Auth", new { ReturnUrl = Request.Path });
            }
            var cart = GetCartFromSession();
            var item = cart.Items.FirstOrDefault(i => i.Id == itemId);

            if (item != null)
            {
                cart.Items.Remove(item);
                SaveCartToSession(cart);
            }

            return RedirectToAction(nameof(Index));
        }

        // POST: Cart/Clear
        [HttpPost]
        public IActionResult Clear()
        {
            if (!IsLoggedIn())
            {
                TempData["ErrorMessage"] = "Bạn cần đăng nhập/đăng kí trước.";
                return RedirectToAction("Login", "Auth", new { ReturnUrl = Request.Path });
            }
            HttpContext.Session.Remove("Cart");
            return RedirectToAction(nameof(Index));
        }

        // GET: Cart/Checkout
        [HttpGet]
        public IActionResult Checkout()
        {
            if (!IsLoggedIn())
            {
                TempData["ErrorMessage"] = "Bạn cần đăng nhập/đăng kí trước.";
                return RedirectToAction("Login", "Auth", new { ReturnUrl = Request.Path });
            }
            var cart = GetCartFromSession();
            if (cart == null || !cart.Items.Any())
            {
                TempData["ErrorMessage"] = "Giỏ hàng của bạn đang trống.";
                return RedirectToAction("Index");
            }
            var checkoutModel = new CheckoutViewModel
            {
                Name = HttpContext.Session.GetString("FullName") ?? "",
                Phone = HttpContext.Session.GetString("Phone") ?? "",
                Address = HttpContext.Session.GetString("Address") ?? "",
                PaymentMethod = "COD"
            };
            return View(checkoutModel);
        }

        // POST: Cart/Checkout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Checkout(CheckoutViewModel model)
        {
            if (!IsLoggedIn())
            {
                TempData["ErrorMessage"] = "Bạn cần đăng nhập/đăng kí trước.";
                return RedirectToAction("Login", "Auth", new { ReturnUrl = Request.Path });
            }
            var cart = GetCartFromSession();
            if (cart == null || !cart.Items.Any())
            {
                TempData["ErrorMessage"] = "Giỏ hàng của bạn đang trống.";
                return RedirectToAction("Index");
            }
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Lấy token và customerId từ session
            var token = HttpContext.Session.GetString("JwtToken");
            var customerId = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(customerId))
            {
                TempData["ErrorMessage"] = "Bạn cần đăng nhập lại.";
                return RedirectToAction("Login", "Auth");
            }

            // Chuẩn bị dữ liệu gửi lên API
            var apiUrl = $"{_configuration["ApiBaseUrl"]}Orders";
            var orderDto = new
            {
                customerId = int.Parse(customerId),
                shippingAddress = model.Address,
                phoneNumber = model.Phone,
                notes = model.Note,
                orderDetails = cart.Items.Select(i => new {
                    productId = i.ProductId,
                    quantity = i.Quantity,
                    price = i.Price,
                    size = i.Size
                }).ToList()
            };

            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            var response = await client.PostAsJsonAsync(apiUrl, orderDto);

            if (!response.IsSuccessStatusCode)
            {
                TempData["ErrorMessage"] = "Đặt hàng thất bại!";
                return View(model);
            }

            var orderJson = await response.Content.ReadAsStringAsync();
            TempData["OrderSuccess"] = true;
            TempData["OrderInfo"] = orderJson;
            TempData["CheckoutName"] = model.Name;
            TempData["CheckoutPhone"] = model.Phone;
            TempData["CheckoutAddress"] = model.Address;
            TempData["CheckoutPaymentMethod"] = model.PaymentMethod;
            HttpContext.Session.Remove("Cart");
            return RedirectToAction("OrderSuccess");
        }

        // GET: Cart/OrderSuccess
        public IActionResult OrderSuccess()
        {
            if (TempData["OrderSuccess"] == null)
            {
                return RedirectToAction("Index");
            }
            var orderInfo = TempData["OrderInfo"] as string;
            OrderViewModel? order = null;
            if (!string.IsNullOrEmpty(orderInfo))
            {
                var apiOrder = System.Text.Json.JsonSerializer.Deserialize<OrderApiResponse>(orderInfo);
                if (apiOrder != null)
                {
                    order = new OrderViewModel
                    {
                        Id = apiOrder.Id,
                        CustomerId = apiOrder.CustomerId,
                        CustomerName = !string.IsNullOrEmpty(apiOrder.CustomerName) ? apiOrder.CustomerName : (TempData["CheckoutName"] as string ?? ""),
                        Phone = !string.IsNullOrEmpty(apiOrder.CustomerPhone) ? apiOrder.CustomerPhone : (TempData["CheckoutPhone"] as string ?? ""),
                        Address = !string.IsNullOrEmpty(apiOrder.ShippingAddress) ? apiOrder.ShippingAddress : (TempData["CheckoutAddress"] as string ?? ""),
                        PaymentMethod = TempData["CheckoutPaymentMethod"] as string ?? "COD",
                        TotalAmount = apiOrder.TotalAmount,
                        Status = apiOrder.Status,
                        OrderDate = apiOrder.OrderDate,
                        Items = apiOrder.OrderDetails.Select(od => new CartItemViewModel
                        {
                            ProductId = od.ProductId,
                            ProductName = od.ProductName,
                            ProductImage = od.ProductImageUrl,
                            Price = od.Price,
                            Size = od.Size,
                            Quantity = od.Quantity
                        }).ToList()
                    };
                }
            }
            return View(order);
        }

        private CartViewModel GetCartFromSession()
        {
            var cartJson = HttpContext.Session.GetString("Cart");
            if (string.IsNullOrEmpty(cartJson))
            {
                return new CartViewModel();
            }

            try
            {
                return JsonSerializer.Deserialize<CartViewModel>(cartJson) ?? new CartViewModel();
            }
            catch
            {
                return new CartViewModel();
            }
        }

        private void SaveCartToSession(CartViewModel cart)
        {
            var cartJson = JsonSerializer.Serialize(cart);
            HttpContext.Session.SetString("Cart", cartJson);
        }
    }

    // Thêm class tạm cho deserialize
    public class OrderApiResponse
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string CustomerPhone { get; set; } = string.Empty;
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; } = string.Empty;
        public string ShippingAddress { get; set; } = string.Empty;
        public List<OrderDetailApiResponse> OrderDetails { get; set; } = new();
    }
    public class OrderDetailApiResponse
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string ProductImageUrl { get; set; } = string.Empty;
        public int Size { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal Subtotal { get; set; }
    }
} 