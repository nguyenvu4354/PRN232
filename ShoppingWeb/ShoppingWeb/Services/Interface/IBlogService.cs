using ShoppingWeb.DTOs;
using ShoppingWeb.Models;

namespace ShoppingWeb.Services.Interface
{
    public interface IBlogService
    {
        public Task<IEnumerable<Blog>> GetBlogsAsync();
        public Task<Blog> GetBlogByIdAsync(int id);
        public Task<IEnumerable<Blog>> GetBlogsByAuthorAsync(int authorId);
        public Task<Blog> CreateBlogAsync(CreateBlogDTO blog);
        public Task<Blog> UpdateBlogAsync(UpdateBlogDTO blog);
        public Task<bool> DeleteBlogAsync(int id);
        public Task<IEnumerable<Blog>> GetBlogsAdvancedAsync(string? search, string? author, int pageIndex, int pageSize);
    }
}
