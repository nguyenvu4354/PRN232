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

        public Task<bool> DeleteBlogAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<Blog> GetBlogByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Blog>> GetBlogsAdvancedAsync(string? search, string? author, int pageIndex, int pageSize)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Blog>> GetBlogsAsync()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Blog>> GetBlogsByAuthorAsync(int authorId)
        {
            throw new NotImplementedException();
        }

        public Task<Blog> UpdateBlogAsync(UpdateBlogDTO blog)
        {
            throw new NotImplementedException();
        }
    }
}
