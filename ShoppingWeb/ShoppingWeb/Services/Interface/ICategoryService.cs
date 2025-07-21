using ShoppingWeb.Models;
using ShoppingWeb.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ShoppingWeb.Services.Interface
{
    public interface ICategoryService
    {
        Task<IEnumerable<Category>> GetCategoriesAsync();
        Task<Category> GetCategoryByIdAsync(int id);
        Task<Category> CreateCategoryAsync(Category category);
        Task<Category> UpdateCategoryAsync(Category category);
        Task<bool> SetCategoryDisableAsync(int id, bool disable);
    }
} 