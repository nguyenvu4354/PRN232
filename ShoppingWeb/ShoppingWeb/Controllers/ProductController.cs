using Microsoft.AspNetCore.Mvc;
using ShoppingWeb.Services.Interface;

namespace ShoppingWeb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : Controller
    {
        private readonly IProductService _productService;
        public ProductController(IProductService productService)
        {
            _productService = productService;
        }
        [HttpGet("related/{productId}")]
        public async Task<IActionResult> GetRelatedProducts(int productId)
        {
            try
            {
                var relatedProducts = await _productService.GetRelatedProduct(productId);
                return Ok(relatedProducts);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
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
            int? brand = null,
            int? category = null,
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

        [HttpGet("products/{id}")]
        public async Task<IActionResult> GetProductById(int id)
        {
            try
            {
                var product = await _productService.GetProductByIdAsync(id);
                if (product == null)
                {
                    return NotFound($"Product with ID {id} not found.");
                }
                return Ok(product);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
