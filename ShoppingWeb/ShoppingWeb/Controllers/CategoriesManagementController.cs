using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShoppingWeb.Models;
using ShoppingWeb.DTOs;
using System.Threading.Tasks;

[Route("api/[controller]")]
[ApiController]
public class CategoriesManagementController : ControllerBase
{
    private readonly ShoppingWebContext _context;

    public CategoriesManagementController(ShoppingWebContext context)
    {
        _context = context;
    }

    // GET: api/categories
    [HttpGet]
    public async Task<ActionResult<IEnumerable<CategoryDto>>> GetCategories()
    {
        var categories = await _context.Categories
            .Select(c => new CategoryDto
            {
                Id = c.CategoryId,
                Name = c.CategoryName,
                Description = c.Description
            })
            .ToListAsync();

        return categories;
    }

    // GET: api/categories/5
    [HttpGet("{id}")]
    public async Task<ActionResult<CategoryDto>> GetCategory(int id)
    {
        var category = await _context.Categories
            .Where(c => c.CategoryId == id)
            .Select(c => new CategoryDto
            {
                Id = c.CategoryId,
                Name = c.CategoryName,
                Description = c.Description
            })
            .FirstOrDefaultAsync();

        if (category == null)
        {
            return NotFound();
        }
        return category;
    }

    // POST: api/categories
    [HttpPost]
    public async Task<ActionResult<CategoryDto>> CreateCategory(CategoryDto categoryDto)
    {
        if (string.IsNullOrEmpty(categoryDto.Name))
        {
            return BadRequest("CategoryName is required.");
        }

        var category = new Category
        {
            CategoryName = categoryDto.Name,
            Description = categoryDto.Description,
            CreatedAt = DateTime.Now
        };

        _context.Categories.Add(category);
        await _context.SaveChangesAsync();

        categoryDto.Id = category.CategoryId;
        return CreatedAtAction(nameof(GetCategory), new { id = categoryDto.Id }, categoryDto);
    }

    // PUT: api/categories/5
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateCategory(int id, CategoryDto categoryDto)
    {
        if (id != categoryDto.Id)
        {
            return BadRequest();
        }

        var category = await _context.Categories.FindAsync(id);
        if (category == null)
        {
            return NotFound();
        }

        category.CategoryName = categoryDto.Name;
        category.Description = categoryDto.Description;
        category.UpdatedAt = DateTime.Now;

        await _context.SaveChangesAsync();

        return NoContent();
    }

    // DELETE: api/categories/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCategory(int id)
    {
        var category = await _context.Categories.FindAsync(id);
        if (category == null)
        {
            return NotFound();
        }

        _context.Categories.Remove(category);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}