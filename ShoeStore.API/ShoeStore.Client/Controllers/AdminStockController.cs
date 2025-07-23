using Microsoft.AspNetCore.Mvc;
using ShoeStore.Client.Services;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace ShoeStore.Client.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminStockController : Controller
    {
        private readonly IProductService _productService;
        public AdminStockController(IProductService productService)
        {
            _productService = productService;
        }

        public async Task<IActionResult> Index()
        {
            var products = await _productService.GetAllAsync();
            return View(products);
        }
    }
} 