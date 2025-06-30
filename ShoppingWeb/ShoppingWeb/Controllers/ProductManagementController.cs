using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShoppingWeb.Models;
using ShoppingWeb.DTOs;
using System.Threading.Tasks;

namespace ShoppingWeb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsManagementController : ControllerBase
    {
        private readonly ShoppingWebContext _context;
        private readonly IWebHostEnvironment _environment;

        public ProductsManagementController(ShoppingWebContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        // GET: api/products
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetProducts()
        {
            var products = await _context.Products
                .Select(p => new ProductDto
                {
                    Id = p.ProductId,
                    Name = p.ProductName,
                    Description = p.Description,
                    Price = p.Price,
                    StockQuantity = p.StockQuantity
                })
                .ToListAsync();

            return products;
        }

        // GET: api/products/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ProductDto>> GetProduct(int id)
        {
            var product = await _context.Products
                .Where(p => p.ProductId == id)
                .Select(p => new ProductDto
                {
                    Id = p.ProductId,
                    Name = p.ProductName,
                    Description = p.Description,
                    Price = p.Price,
                    StockQuantity = p.StockQuantity
                })
                .FirstOrDefaultAsync();

            if (product == null)
            {
                return NotFound();
            }
            return product;
        }

        // POST: api/products
        [HttpPost]
        public async Task<ActionResult<ProductDto>> CreateProduct([FromForm] ProductRequest request)
        {
            if (string.IsNullOrEmpty(request.Product.ProductName))
            {
                return BadRequest("ProductName is required.");
            }

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

            if (request.Product.ImageFile != null)
            {
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(request.Product.ImageFile.FileName);
                var filePath = Path.Combine(_environment.WebRootPath, "uploads", fileName);
                Directory.CreateDirectory(Path.GetDirectoryName(filePath) ?? throw new InvalidOperationException());
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await request.Product.ImageFile.CopyToAsync(stream);
                }
                product.ImageUrl = $"/uploads/{fileName}"; // Sử dụng ImageURL thay vì ImageUrl
            }

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            var createdProduct = await _context.Products
                .Where(p => p.ProductId == product.ProductId)
                .Select(p => new ProductDto
                {
                    Id = p.ProductId,
                    Name = p.ProductName,
                    Description = p.Description,
                    Price = p.Price,
                    StockQuantity = p.StockQuantity
                })
                .FirstOrDefaultAsync();

            return CreatedAtAction(nameof(GetProduct), new { id = createdProduct.Id }, createdProduct);
        }

        // PUT: api/products/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(int id, [FromForm] ProductRequest request)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            product.ProductName = request.Product.ProductName;
            product.Description = request.Product.Description;
            product.Price = request.Product.Price;
            product.StockQuantity = request.Product.StockQuantity;
            product.BrandId = request.Product.BrandId;
            product.CategoryId = request.Product.CategoryId;

            if (request.Product.ImageFile != null) // Sửa lại từ ImageUrl thành ImageFile
            {
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(request.Product.ImageFile.FileName);
                var filePath = Path.Combine(_environment.WebRootPath, "uploads", fileName);
                Directory.CreateDirectory(Path.GetDirectoryName(filePath) ?? throw new InvalidOperationException());
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await request.Product.ImageFile.CopyToAsync(stream);
                }
                product.ImageUrl = $"/uploads/{fileName}"; // Sử dụng ImageURL
            }

            product.UpdatedAt = DateTime.Now;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/products/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // Model cho request (hỗ trợ upload file)
        public class ProductRequest
        {
            public CreateProductDto Product { get; set; }
        }
    }
}