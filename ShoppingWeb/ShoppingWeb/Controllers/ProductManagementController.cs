using Microsoft.AspNetCore.Mvc;
using ShoppingWeb.Models;
using ShoppingWeb.DTOs;
using ShoppingWeb.Services.Interface;
using ShoppingWeb.Services;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using System;

namespace ShoppingWeb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsManagementController : ControllerBase
    {
        private readonly IProductManagementService _productService;
        private readonly ICloudinaryService _cloudinaryService;

        public ProductsManagementController(IProductManagementService productService, ICloudinaryService cloudinaryService)
        {
            _productService = productService;
            _cloudinaryService = cloudinaryService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetProducts()
        {
            var products = await _productService.GetProductsAsync();
            var result = new List<ProductDto>();
            foreach (var p in products)
            {
                result.Add(new ProductDto { Id = p.ProductId, Name = p.ProductName, Description = p.Description, Price = p.Price, StockQuantity = p.StockQuantity, ImageUrl = p.ImageUrl });
            }
            return result;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProductDto>> GetProduct(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null) return NotFound();
            return new ProductDto { Id = product.ProductId, Name = product.ProductName, Description = product.Description, Price = product.Price, StockQuantity = product.StockQuantity, ImageUrl = product.ImageUrl };
        }

        [HttpPost]
        public async Task<ActionResult<ProductDto>> CreateProduct([FromForm] ProductRequest request)
        {
            if (string.IsNullOrEmpty(request.Product.ProductName)) return BadRequest("ProductName is required.");
            var product = new Product
            {
                ProductName = request.Product.ProductName,
                Description = request.Product.Description,
                Price = request.Product.Price,
                StockQuantity = request.Product.StockQuantity,
                BrandId = request.Product.BrandId,
                CategoryId = request.Product.CategoryId,
                CreatedAt = DateTime.Now
            };
            // Xử lý ảnh nếu có
            if (request.Product.ImageFile != null)
            {
                var imageUrl = await _cloudinaryService.UploadImageAsync(request.Product.ImageFile);
                product.ImageUrl = imageUrl;
            }
            var created = await _productService.CreateProductAsync(product);
            var createdDto = new ProductDto { Id = created.ProductId, Name = created.ProductName, Description = created.Description, Price = created.Price, StockQuantity = created.StockQuantity };
            return CreatedAtAction(nameof(GetProduct), new { id = createdDto.Id }, createdDto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(int id, [FromForm] ProductRequest request)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null) return NotFound();
            product.ProductName = request.Product.ProductName;
            product.Description = request.Product.Description;
            product.Price = request.Product.Price;
            product.StockQuantity = request.Product.StockQuantity;
            product.BrandId = request.Product.BrandId;
            product.CategoryId = request.Product.CategoryId;
            if (request.Product.ImageFile != null)
            {
                var imageUrl = await _cloudinaryService.UploadImageAsync(request.Product.ImageFile);
                product.ImageUrl = imageUrl;
            }
            await _productService.UpdateProductAsync(product);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var deleted = await _productService.DeleteProductAsync(id);
            if (!deleted) return NotFound();
            return NoContent();
        }

        public class ProductRequest
        {
            public CreateProductDto Product { get; set; }
        }
    }
}