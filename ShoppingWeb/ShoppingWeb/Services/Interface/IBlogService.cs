using ShoppingWeb.DTOs;
using ShoppingWeb.Models;

namespace ShoppingWeb.Services.Interface
{
    public interface IBlogService
    {
        public Task<IEnumerable<BlogPostDTO>> GetNewestBlog();
        public Task<IEnumerable<BlogPostDTO>> GetBlogsAsync();
        public Task<BlogDetailDTO> GetBlogByIdAsync(int id);
        public Task<IEnumerable<BlogPostDTO>> GetBlogsByAuthorAsync(int authorId);
        public Task<Blog> CreateBlogAsync(CreateBlogDTO blog);
        public Task<Blog> UpdateBlogAsync(UpdateBlogDTO blog);
        public Task<bool> DeleteBlogAsync(int id);
        public Task<IEnumerable<BlogPostDTO>> GetBlogsAdvancedAsync(string? search, string? author, int pageIndex, int pageSize);
    }
}
