using ShoppingWeb.DTOs;
using ShoppingWeb.Models;

namespace ShoppingWeb.Services.Interface
{
    public interface IProductService
    {
        public Task<IEnumerable<Product>> GetProductsAsync();
        public Task<ProductDetailItemDto> GetProductByIdAsync(int id);
        public Task<IEnumerable<ProductListItemResponseDto>> GetProductAdvancedAsync(string? search, int? brand, int? category, string? sortBy, int pageIndex, int pageSize);
        public Task<int> GetProductCountAsync(string? search, string? category, string? brand);
        //public Task<Product> CreateProductAsync(Product product);
        //public Task<Product> UpdateProductAsync(Product product);
        //public Task<bool> DeleteProductAsync(int id);
        public Task<IEnumerable<ProductListItemResponseDto>> GetProductsByBrandAsync(int brandId);
        public Task<IEnumerable<ProductListItemResponseDto>> GetProductsByCategoryAsync(int categoryId);
    }
}
