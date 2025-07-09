using ShoppingWeb.Models;
using ShoppingWeb.Services.Interface;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace ShoppingWeb.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ShoppingWebContext _context;
        public CategoryService(ShoppingWebContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<Category>> GetCategoriesAsync()
        {
            return await _context.Categories.ToListAsync();
        }
        public async Task<Category> GetCategoryByIdAsync(int id)
        {
            return await _context.Categories.FindAsync(id);
        }
        public async Task<Category> CreateCategoryAsync(Category category)
        {
            category.CreatedAt = DateTime.Now;
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();
            return category;
        }
        public async Task<Category> UpdateCategoryAsync(Category category)
        {
            var existing = await _context.Categories.FindAsync(category.CategoryId);
            if (existing == null) return null;
            existing.CategoryName = category.CategoryName;
            existing.Description = category.Description;
            existing.UpdatedAt = DateTime.Now;
            await _context.SaveChangesAsync();
            return existing;
        }
        public async Task<bool> DeleteCategoryAsync(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null) return false;
            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
            return true;
        }
    }
} 