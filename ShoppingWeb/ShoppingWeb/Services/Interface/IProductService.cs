using ShoppingWeb.Models;

namespace ShoppingWeb.Services.Interface
{
    public interface IProductService
    {
        public Task<IEnumerable<Product>> GetProductsAsync();
        public Task<Product> GetProductByIdAsync(int id);
        public Task<IEnumerable<Product>> GetProductAdvancedAsync(string? search, string? brand, string? category, string? sortBy, int pageIndex, int pageSize);
        public Task<int> GetProductCountAsync(string? search, string? category, string? brand);
        //public Task<Product> CreateProductAsync(Product product);
        //public Task<Product> UpdateProductAsync(Product product);
        //public Task<bool> DeleteProductAsync(int id);
        public Task<IEnumerable<Product>> GetProductsByBrandAsync(int brandId);
        public Task<IEnumerable<Product>> GetProductsByCategoryAsync(int categoryId);
    }
}
