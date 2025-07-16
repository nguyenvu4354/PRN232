using Microsoft.AspNetCore.Mvc;
using ShoppingWeb.DTOs;
using ShoppingWeb.Services.Interface;

namespace ShoppingWeb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BlogController : Controller
    {
        private readonly IBlogService _blogService;
        public BlogController(IBlogService blogService)
        {
            _blogService = blogService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var blogs = await _blogService.GetBlogsAsync();
            return Ok(blogs);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var blog = await _blogService.GetBlogByIdAsync(id);
            if (blog == null) return NotFound();
            return Ok(blog);
        }

        [HttpGet("author/{authorId}")]
        public async Task<IActionResult> GetByAuthor(int authorId)
        {
            var blogs = await _blogService.GetBlogsByAuthorAsync(authorId);
            return Ok(blogs);
        }

        [HttpGet("advanced")]
        public async Task<IActionResult> GetAdvanced([FromQuery] string? search, [FromQuery] string? author, [FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 10)
        {
            var blogs = await _blogService.GetBlogsAdvancedAsync(search, author, pageIndex, pageSize);
            return Ok(blogs);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateBlogDTO dto)
        {
            var blog = await _blogService.CreateBlogAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = blog.BlogId }, blog);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateBlogDTO dto)
        {
            if (id != dto.BlogId) return BadRequest();
            var updated = await _blogService.UpdateBlogAsync(dto);
            if (updated == null) return NotFound();
            return Ok(updated);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _blogService.DeleteBlogAsync(id);
            if (!deleted) return NotFound();
            return NoContent();
        }
    }
}
