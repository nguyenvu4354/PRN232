using Microsoft.AspNetCore.Mvc;
using ShoppingWeb.Models;
using ShoppingWeb.DTOs;
using ShoppingWeb.Services.Interface;
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
        private readonly IWebHostEnvironment _environment;

        public ProductsManagementController(IProductManagementService productService, IWebHostEnvironment environment)
        {
            _productService = productService;
            _environment = environment;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetProducts()
        {
            var products = await _productService.GetProductsAsync();
            var result = new List<ProductDto>();
            foreach (var p in products)
            {
                result.Add(new ProductDto { Id = p.ProductId, Name = p.ProductName, Description = p.Description, Price = p.Price, StockQuantity = p.StockQuantity });
            }
            return result;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProductDto>> GetProduct(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null) return NotFound();
            return new ProductDto { Id = product.ProductId, Name = product.ProductName, Description = product.Description, Price = product.Price, StockQuantity = product.StockQuantity };
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
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(request.Product.ImageFile.FileName);
                var filePath = Path.Combine(_environment.WebRootPath, "uploads", fileName);
                Directory.CreateDirectory(Path.GetDirectoryName(filePath) ?? throw new InvalidOperationException());
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await request.Product.ImageFile.CopyToAsync(stream);
                }
                product.ImageUrl = $"/uploads/{fileName}";
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
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(request.Product.ImageFile.FileName);
                var filePath = Path.Combine(_environment.WebRootPath, "uploads", fileName);
                Directory.CreateDirectory(Path.GetDirectoryName(filePath) ?? throw new InvalidOperationException());
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await request.Product.ImageFile.CopyToAsync(stream);
                }
                product.ImageUrl = $"/uploads/{fileName}";
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