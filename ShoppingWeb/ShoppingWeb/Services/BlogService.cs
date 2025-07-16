using Microsoft.EntityFrameworkCore;
using ShoppingWeb.Data;
using ShoppingWeb.DTOs;
using ShoppingWeb.Models;
using ShoppingWeb.Services.Interface;

namespace ShoppingWeb.Services
{
    public class BlogService : IBlogService
    {
        private readonly ShoppingWebContext _context;
        public BlogService(ShoppingWebContext context)
        {
            _context = context;
        }
        public Task<Blog> CreateBlogAsync(CreateBlogDTO blog)
        {
            var newBlog = new Blog
            {
                Title = blog.Title,
                Content = blog.Content,
                AuthorId = blog.AuthorId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            _context.Blogs.Add(newBlog);
            _context.SaveChanges();
            return Task.FromResult(newBlog);
        }

        public async Task<bool> DeleteBlogAsync(int id)
        {
            var blog = await _context.Blogs.FindAsync(id);
            if (blog == null) return false;

            _context.Blogs.Remove(blog);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<BlogDetailDTO> GetBlogByIdAsync(int id)
        {
            var blog = await _context.Blogs.Include(b => b.Author).FirstOrDefaultAsync(b => b.BlogId == id);
            if (blog == null) return null;
            return new BlogDetailDTO
            {
                Id = blog.BlogId,
                Title = blog.Title,
                Content = blog.Content,
                Author = blog.Author.Username,
                CreatedAt = blog.CreatedAt.Value,
                UpdatedAt = blog.UpdatedAt.GetValueOrDefault(),
                ImageUrl = blog.Thumbnail
            };
        }

        public async Task<IEnumerable<BlogPostDTO>> GetBlogsAdvancedAsync(string? search, string? author, int pageIndex, int pageSize)
        {
            var query = _context.Blogs.AsQueryable();

            if (!string.IsNullOrEmpty(search))
                query = query.Where(b => b.Title.Contains(search) || b.Content.Contains(search));

            if (!string.IsNullOrEmpty(author))
                query = query.Where(b => b.Author.Username.Contains(author));

            return await query.Skip((pageIndex - 1) * pageSize).Take(pageSize).Select(b => new BlogPostDTO
            {
                Id = b.BlogId,
                Title = b.Title,
                ContentSummary = b.Content.Length > 100 ? b.Content.Substring(0, 100) + "..." : b.Content,
                Author = b.Author.Username,
                CreatedAt = b.CreatedAt.Value,
                ImageUrl = b.Thumbnail
            }).ToListAsync();
        }

        public async Task<IEnumerable<BlogPostDTO>> GetBlogsAsync()
        {
            return await _context.Blogs.Select(b => new BlogPostDTO
            {
                Id = b.BlogId,
                Title = b.Title,
                ContentSummary = b.Content.Length > 100 ? b.Content.Substring(0, 100) + "..." : b.Content,
                Author = b.Author.Username,
                CreatedAt = b.CreatedAt.Value,
                ImageUrl = b.Thumbnail
            }).ToListAsync();
        }

        public async Task<IEnumerable<BlogPostDTO>> GetBlogsByAuthorAsync(int authorId)
        {
            return await _context.Blogs.Where(b => b.AuthorId == authorId).Select(b => new BlogPostDTO
            {
                Id = b.BlogId,
                Title = b.Title,
                ContentSummary = b.Content.Length > 100 ? b.Content.Substring(0, 100) + "..." : b.Content,
                Author = b.Author.Username,
                CreatedAt = b.CreatedAt.Value,
                ImageUrl = b.Thumbnail
            }).ToListAsync();
        }

        public async Task<Blog> UpdateBlogAsync(UpdateBlogDTO blog)
        {
            var existingBlog = await _context.Blogs.FindAsync(blog.BlogId);
            if (existingBlog == null) return null;

            existingBlog.Title = blog.Title;
            existingBlog.Content = blog.Content;
            existingBlog.UpdatedAt = DateTime.UtcNow;

            _context.Blogs.Update(existingBlog);
            await _context.SaveChangesAsync();
            return existingBlog;
        }
    }
}
