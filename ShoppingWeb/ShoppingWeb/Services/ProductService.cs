using Microsoft.EntityFrameworkCore;
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

        public async Task<IEnumerable<Product>> GetProductAdvancedAsync(string? search, string? brand, string? category, string? sortBy, int pageIndex, int pageSize)
        {
            var query = _context.Products.AsQueryable();

            // Filter by search term  
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(p => p.ProductName.Contains(search) || (p.Description != null && p.Description.Contains(search)));
            }

            // Filter by brand  
            if (!string.IsNullOrEmpty(brand))
            {
                query = query.Where(p => p.BrandId.HasValue && _context.Brands.Any(b => b.BrandId == p.BrandId && b.Description == brand));
            }

            // Filter by category  
            if (!string.IsNullOrEmpty(category))
            {
                query = query.Where(p => p.CategoryId.HasValue && _context.Categories.Any(c => c.CategoryId == p.CategoryId && c.Description == category));
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

            return await query.ToListAsync();
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

        public async Task<IEnumerable<Product>> GetProductsByBrandAsync(int brandId)
        {
            return await _context.Products.Where(p => p.BrandId == brandId).ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetProductsByCategoryAsync(int categoryId)
        {
            return await _context.Products.Where(p => p.CategoryId == categoryId).ToListAsync();
        }

    }
}
