using Microsoft.AspNetCore.Mvc;
using ShoeStore.Client.Services;
using ShoeStore.Client.Models;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;
using System.Linq;
using System;

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
            var brands = await _productService.GetAllBrandsAsync();
            Console.WriteLine($"[DEBUG] Brands count: {brands.Count}");
            ViewBag.Brands = brands.Select(b => new SelectListItem
            {
                Value = b.Id.ToString(),
                Text = b.Name
            }).ToList();
            var categories = await _productService.GetAllCategoriesAsync();
            Console.WriteLine($"[DEBUG] Categories count: {categories.Count}");
            ViewBag.Categories = categories.Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.Name
            }).ToList();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(ProductViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var brands = await _productService.GetAllBrandsAsync();
                ViewBag.Brands = brands.Select(b => new SelectListItem
                {
                    Value = b.Id.ToString(),
                    Text = b.Name
                }).ToList();
                var categories = await _productService.GetAllCategoriesAsync();
                ViewBag.Categories = categories.Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name
                }).ToList();
                return View(model);
            }
            await _productService.CreateAsync(model);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Edit(int id)
        {
            var product = await _productService.GetByIdAsync(id);
            if (product == null) return NotFound();
            var brands = await _productService.GetAllBrandsAsync();
            ViewBag.Brands = brands.Select(b => new SelectListItem
            {
                Value = b.Id.ToString(),
                Text = b.Name,
                Selected = (b.Id == product.BrandId)
            }).ToList();
            var categories = await _productService.GetAllCategoriesAsync();
            ViewBag.Categories = categories.Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.Name,
                Selected = (c.Id == product.CategoryId)
            }).ToList();
            return View(product);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(ProductViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var brands = await _productService.GetAllBrandsAsync();
                ViewBag.Brands = brands.Select(b => new SelectListItem
                {
                    Value = b.Id.ToString(),
                    Text = b.Name,
                    Selected = (b.Id == model.BrandId)
                }).ToList();
                var categories = await _productService.GetAllCategoriesAsync();
                ViewBag.Categories = categories.Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name,
                    Selected = (c.Id == model.CategoryId)
                }).ToList();
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