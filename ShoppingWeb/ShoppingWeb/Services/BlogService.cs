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
        public Task<Blog> CreateBlogAsync(Blog blog)
        {
            throw new NotImplementedException();
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

        public Task<IEnumerable<Blog>> GetBlogsByAuthorAsync(string author)
        {
            throw new NotImplementedException();
        }

        public Task<Blog> UpdateBlogAsync(Blog blog)
        {
            throw new NotImplementedException();
        }
    }
}
