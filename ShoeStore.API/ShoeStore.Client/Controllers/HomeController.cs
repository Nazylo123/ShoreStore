using Microsoft.AspNetCore.Mvc;
using ShoeStore.Client.Models;
using ShoeStore.Client.Services;
using System.Diagnostics;

namespace ShoeStore.Client.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;
        private readonly IBrandService _brandService;

        public HomeController(ILogger<HomeController> logger, IProductService productService, ICategoryService categoryService, IBrandService brandService)
        {
            _logger = logger;
            _productService = productService;
            _categoryService = categoryService;
            _brandService = brandService;
        }

        public async Task<IActionResult> Index()
        {
            var featuredProducts = await _productService.GetActiveProductsAsync();
            var categories = await _categoryService.GetActiveCategoriesAsync();
            var brands = await _brandService.GetActiveBrandsAsync();

            ViewBag.FeaturedProducts = featuredProducts.Take(6).ToList();
            ViewBag.Categories = categories;
            ViewBag.Brands = brands;

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult About()
        {
            return View();
        }

        public IActionResult Contact()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
