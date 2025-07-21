using Microsoft.EntityFrameworkCore;
using ShoppingWeb.Data;
using ShoppingWeb.Models;
using ShoppingWeb.Services.Interface;

namespace ShoppingWeb.Services
{
    public class BrandService : IBrandService
    {
        private readonly ShoppingWebContext _context;
        public BrandService(ShoppingWebContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<Brand>> GetBrandsAsync()
        {
            return await _context.Brands.ToListAsync();
        }
        public async Task<Brand> GetBrandByIdAsync(int id)
        {
            return await _context.Brands.FindAsync(id);
        }
        public async Task<Brand> CreateBrandAsync(Brand brand)
        {
            brand.CreatedAt = DateTime.Now;
            _context.Brands.Add(brand);
            await _context.SaveChangesAsync();
            return brand;
        }
        public async Task<Brand> UpdateBrandAsync(Brand brand)
        {
            var existing = await _context.Brands.FindAsync(brand.BrandId);
            if (existing == null) return null;
            existing.BrandName = brand.BrandName;
            existing.Description = brand.Description;
            existing.UpdatedAt = DateTime.Now;
            await _context.SaveChangesAsync();
            return existing;
        }
        public async Task<bool> DeleteBrandAsync(int id)
        {
            var brand = await _context.Brands.FindAsync(id);
            if (brand == null) return false;
            _context.Brands.Remove(brand);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}