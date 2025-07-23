using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ShoeStore.API.DTOs;
using ShoeStore.API.Services;

namespace ShoeStore.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BrandsController : ControllerBase
    {
        private readonly IBrandService _brandService;

        public BrandsController(IBrandService brandService)
        {
            _brandService = brandService;
        }

        // GET: api/Brands
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BrandResponseDTO>>> GetBrands()
        {
            try
            {
                var brands = await _brandService.GetAllBrandsAsync();
                return Ok(brands);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // GET: api/Brands/with-products
        [HttpGet("with-products")]
        public async Task<ActionResult<IEnumerable<BrandResponseDTO>>> GetBrandsWithProducts()
        {
            try
            {
                var brands = await _brandService.GetBrandsWithProductsAsync();
                return Ok(brands);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // GET: api/Brands/5
        [HttpGet("{id}")]
        public async Task<ActionResult<BrandResponseDTO>> GetBrand(int id)
        {
            try
            {
                var brand = await _brandService.GetBrandByIdAsync(id);

                if (brand == null)
                {
                    return NotFound($"Brand with ID {id} not found");
                }

                return Ok(brand);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // GET: api/Brands/5/with-products
        [HttpGet("{id}/with-products")]
        public async Task<ActionResult<BrandResponseDTO>> GetBrandWithProducts(int id)
        {
            try
            {
                var brand = await _brandService.GetBrandWithProductsAsync(id);

                if (brand == null)
                {
                    return NotFound($"Brand with ID {id} not found");
                }

                return Ok(brand);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // PUT: api/Brands/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBrand(int id, UpdateBrandDTO brandDto)
        {
            try
            {
                await _brandService.UpdateBrandAsync(id, brandDto);
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // POST: api/Brands
        [HttpPost]
        public async Task<ActionResult<BrandResponseDTO>> PostBrand(CreateBrandDTO brandDto)
        {
            try
            {
                var createdBrand = await _brandService.CreateBrandAsync(brandDto);
                return CreatedAtAction(nameof(GetBrand), new { id = createdBrand.Id }, createdBrand);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // DELETE: api/Brands/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBrand(int id)
        {
            try
            {
                await _brandService.DeleteBrandAsync(id);
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
