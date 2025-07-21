using Microsoft.AspNetCore.Mvc;
using ShoppingWeb.Models;
using ShoppingWeb.DTOs;
using ShoppingWeb.Services.Interface;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace ShoppingWeb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesManagementController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoriesManagementController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CategoryDto>>> GetCategories()
        {
            var categories = await _categoryService.GetCategoriesAsync();
            var result = new List<CategoryDto>();
            foreach (var c in categories)
            {
                result.Add(new CategoryDto { Id = c.CategoryId, Name = c.CategoryName, Description = c.Description });
            }
            return result;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CategoryDto>> GetCategory(int id)
        {
            var category = await _categoryService.GetCategoryByIdAsync(id);
            if (category == null) return NotFound();
            return new CategoryDto { Id = category.CategoryId, Name = category.CategoryName, Description = category.Description };
        }

        [HttpPost]
        public async Task<ActionResult<CategoryDto>> CreateCategory(CategoryDto categoryDto)
        {
            if (string.IsNullOrEmpty(categoryDto.Name)) return BadRequest("CategoryName is required.");
            var category = new Category { CategoryName = categoryDto.Name, Description = categoryDto.Description };
            var created = await _categoryService.CreateCategoryAsync(category);
            categoryDto.Id = created.CategoryId;
            return CreatedAtAction(nameof(GetCategory), new { id = categoryDto.Id }, categoryDto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCategory(int id, CategoryDto categoryDto)
        {
            if (id != categoryDto.Id) return BadRequest();
            var category = new Category { CategoryId = categoryDto.Id, CategoryName = categoryDto.Name, Description = categoryDto.Description };
            var updated = await _categoryService.UpdateCategoryAsync(category);
            if (updated == null) return NotFound();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var deleted = await _categoryService.DeleteCategoryAsync(id);
            if (!deleted) return NotFound();
            return NoContent();
        }
    }
}