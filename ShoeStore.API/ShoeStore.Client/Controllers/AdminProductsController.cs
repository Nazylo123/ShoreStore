using Microsoft.AspNetCore.Mvc;
using ShoeStore.Client.Services;
using ShoeStore.Client.Models;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;

namespace ShoeStore.Client.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminProductsController : Controller
    {
        private readonly IProductService _productService;
        public AdminProductsController(IProductService productService)
        {
            _productService = productService;
        }

        public async Task<IActionResult> Index()
        {
            var products = await _productService.GetAllAsync();
            return View(products);
        }

        public async Task<IActionResult> Create()
        {
            ViewBag.Brands = new SelectList(await _productService.GetAllBrandsAsync(), "Id", "Name");
            ViewBag.Categories = new SelectList(await _productService.GetAllCategoriesAsync(), "Id", "Name");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(ProductViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Brands = new SelectList(await _productService.GetAllBrandsAsync(), "Id", "Name");
                ViewBag.Categories = new SelectList(await _productService.GetAllCategoriesAsync(), "Id", "Name");
                return View(model);
            }
            await _productService.CreateAsync(model);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Edit(int id)
        {
            var product = await _productService.GetByIdAsync(id);
            if (product == null) return NotFound();
            ViewBag.Brands = new SelectList(await _productService.GetAllBrandsAsync(), "Id", "Name", product.BrandId);
            ViewBag.Categories = new SelectList(await _productService.GetAllCategoriesAsync(), "Id", "Name", product.CategoryId);
            return View(product);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(ProductViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Brands = new SelectList(await _productService.GetAllBrandsAsync(), "Id", "Name", model.BrandId);
                ViewBag.Categories = new SelectList(await _productService.GetAllCategoriesAsync(), "Id", "Name", model.CategoryId);
                return View(model);
            }
            await _productService.UpdateAsync(model);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            await _productService.DeleteAsync(id);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Details(int id)
        {
            var product = await _productService.GetByIdAsync(id);
            if (product == null) return NotFound();
            return View(product);
        }
    }
} 