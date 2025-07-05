using Microsoft.AspNetCore.Mvc;
using ShoppingWeb.Services.Interface;

namespace ShoppingWeb.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductService _productService;
        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet("products")]
        public async Task<IActionResult> GetProducts()
        {
            try
            {
                var products = await _productService.GetProductsAsync();
                return Ok(products);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        [HttpGet("products/advanced")]
        public async Task<IActionResult> GetProductAdvanced(
            string? search = null,
            string? brand = null,
            string? category = null,
            string? sortBy = null,
            int pageIndex = 1,
            int pageSize = 10)
        {
            try
            {
                var products = await _productService.GetProductAdvancedAsync(search, brand, category, sortBy, pageIndex, pageSize);
                return Ok(products);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        [HttpGet("products/count")]
        public async Task<IActionResult> GetProductCount(
            string? search = null,
            string? category = null,
            string? brand = null)
        {
            try
            {
                var count = await _productService.GetProductCountAsync(search, category, brand);
                return Ok(count);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        [HttpGet("products/brand/{brandId}")]
        public async Task<IActionResult> GetProductsByBrand(int brandId)
        {
            try
            {
                var products = await _productService.GetProductsByBrandAsync(brandId);
                return Ok(products);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        [HttpGet("products/category/{categoryId}")]
        public async Task<IActionResult> GetProductsByCategory(int categoryId)
        {
            try
            {
                var products = await _productService.GetProductsByCategoryAsync(categoryId);
                return Ok(products);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

    }
}
