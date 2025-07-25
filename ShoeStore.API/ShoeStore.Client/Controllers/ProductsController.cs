using Microsoft.AspNetCore.Mvc;
using ShoeStore.Client.Models;
using ShoeStore.Client.Services;

namespace ShoeStore.Client.Controllers
{
    public class ProductsController : Controller
    {
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;
        private readonly IBrandService _brandService;

        public ProductsController(IProductService productService, ICategoryService categoryService, IBrandService brandService)
        {
            _productService = productService;
            _categoryService = categoryService;
            _brandService = brandService;
        }

        // GET: Products
        public async Task<IActionResult> Index(int page = 1, string? searchTerm = null, int? categoryId = null, int? brandId = null, string? sortBy = null)
        {
            var productList = await _productService.GetProductsWithPaginationAsync(page, 6, searchTerm, categoryId, brandId, sortBy);
            
            ViewBag.Categories = await _categoryService.GetActiveCategoriesAsync();
            ViewBag.Brands = await _brandService.GetActiveBrandsAsync();
            
            return View(productList);
        }

        // GET: Products/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // GET: Products/Category/5
        public async Task<IActionResult> Category(int id, int page = 1)
        {
            var category = await _categoryService.GetCategoryByIdAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            var products = await _productService.GetProductsByCategoryAsync(id);
            var productList = new ProductListViewModel
            {
                Products = products.Skip((page - 1) * 12).Take(12).ToList(),
                CurrentPage = page,
                TotalPages = (int)Math.Ceiling((double)products.Count / 12),
                TotalItems = products.Count,
                CategoryId = id
            };

            ViewBag.Category = category;
            ViewBag.Categories = await _categoryService.GetActiveCategoriesAsync();
            ViewBag.Brands = await _brandService.GetActiveBrandsAsync();

            return View("Index", productList);
        }

        // GET: Products/Brand/5
        public async Task<IActionResult> Brand(int id, int page = 1)
        {
            var brand = await _brandService.GetBrandByIdAsync(id);
            if (brand == null)
            {
                return NotFound();
            }

            var products = await _productService.GetProductsByBrandAsync(id);
            var productList = new ProductListViewModel
            {
                Products = products.Skip((page - 1) * 12).Take(12).ToList(),
                CurrentPage = page,
                TotalPages = (int)Math.Ceiling((double)products.Count / 12),
                TotalItems = products.Count,
                BrandId = id
            };

            ViewBag.Brand = brand;
            ViewBag.Categories = await _categoryService.GetActiveCategoriesAsync();
            ViewBag.Brands = await _brandService.GetActiveBrandsAsync();

            return View("Index", productList);
        }
    }
} 