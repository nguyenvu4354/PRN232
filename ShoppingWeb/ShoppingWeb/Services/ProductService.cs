using Microsoft.EntityFrameworkCore;
using ShoppingWeb.Data;
using ShoppingWeb.DTOs;
using ShoppingWeb.Models;
using ShoppingWeb.Services.Interface;

namespace ShoppingWeb.Services
{
    public class ProductService : IProductService
    {
        private readonly ShoppingWebContext _context;
        public ProductService(ShoppingWebContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ProductListItemResponseDto>> GetProductAdvancedAsync(string? search, int? brand, int? category, string? sortBy, int pageIndex, int pageSize)
        {
            var query = _context.Products.Include(p => p.Brand).Include(p => p.Category)
                                        .AsQueryable();

            // Filter by search term  
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(p => p.ProductName.Contains(search) || (p.Description != null && p.Description.Contains(search)));
            }

            // Filter by brand  
            if(brand != null || brand >= 1)
            {
                query = query.Where(p => p.BrandId == brand);
            }

            // Filter by category  
            if(category != null || category >= 1)
            {
                query = query.Where(p => p.CategoryId == category);
            }

            // Sorting  
            query = sortBy?.ToLower() switch
            {
                "name" => query.OrderBy(p => p.ProductName),
                "price" => query.OrderBy(p => p.StockQuantity), // Assuming StockQuantity is used as a proxy for price  
                "name_desc" => query.OrderByDescending(p => p.ProductName),
                "price_desc" => query.OrderByDescending(p => p.StockQuantity),
                _ => query
            };

            // Pagination  
            query = query.Skip((pageIndex - 1) * pageSize).Take(pageSize);

            return await query.Select(p => new ProductListItemResponseDto
            {
                Id = p.ProductId,
                Name = p.ProductName,
                Description = p.Description,
                Price = p.Price,
                ImageUrl = p.ImageUrl,
                Brand = p.Brand != null ? p.Brand.Description : null,
                Category = p.Category != null ? p.Category.Description : null
            }).ToListAsync();
        }

        public async Task<ProductDetailItemDto> GetProductByIdAsync(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentException("invalid id");
            }
            var product = await _context.Products.Include(product => product.Brand)
                                           .Include(product => product.Category)
                                           .Include(product => product.ProductReviews)
                                           .Include(product => product.Wishlists)
                                           .AsNoTracking()
                .FirstOrDefaultAsync(p => p.ProductId == id);
            if (product == null)
            {
                throw new KeyNotFoundException($"Product with ID {id} not found.");
            }
            return new ProductDetailItemDto
            {
                Id = product.ProductId,
                Name = product.ProductName,
                Description = product.Description,
                Price = product.Price,
                StockQuantity = product.StockQuantity,
                ImageUrl = product.ImageUrl,
                BrandId = product.BrandId,
                Brand = product.Brand != null ? product.Brand.Description : string.Empty,
                CategoryId = product.CategoryId,
                Category = product.Category != null ? product.Category.Description : string.Empty
            };
        }

        public async Task<int> GetProductCountAsync(string? search, string? category, string? brand)
        {
            var query = _context.Products.AsQueryable();

            // Filter by search term  
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(p => p.ProductName.Contains(search) || (p.Description != null && p.Description.Contains(search)));
            }

            if (!string.IsNullOrEmpty(brand))
            {
                query = query.Where(p => p.BrandId.HasValue && _context.Brands.Any(b => b.BrandId == p.BrandId && b.Description == brand));
            }

            // Filter by category  
            if (!string.IsNullOrEmpty(category))
            {
                query = query.Where(p => p.CategoryId.HasValue && _context.Categories.Any(c => c.CategoryId == p.CategoryId && c.Description == category));
            }

            return await query.CountAsync();
        }

        public async Task<IEnumerable<Product>> GetProductsAsync()
        {
            return await _context.Products.ToListAsync();
        }

        public async Task<IEnumerable<ProductListItemResponseDto>> GetProductsByBrandAsync(int brandId)
        {
            return await _context.Products.Where(p => p.BrandId == brandId)
                .Select(p => new ProductListItemResponseDto
                {
                    Id = p.ProductId,
                    Name = p.ProductName,
                    Description = p.Description,
                    Price = p.Price,
                    ImageUrl = p.ImageUrl,
                    Brand = p.Brand != null ? p.Brand.Description : string.Empty,
                    Category = p.Category != null ? p.Category.Description : string.Empty
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<ProductListItemResponseDto>> GetProductsByCategoryAsync(int categoryId)
        {
            return await _context.Products.Where(p => p.CategoryId == categoryId)
                .Select(p => new ProductListItemResponseDto
                {
                    Id = p.ProductId,
                    Name = p.ProductName,
                    Description = p.Description,
                    Price = p.Price,
                    ImageUrl = p.ImageUrl,
                    Brand = p.Brand != null ? p.Brand.Description : string.Empty,
                    Category = p.Category != null ? p.Category.Description : string.Empty
                })
                .ToListAsync();
        }

    }
}