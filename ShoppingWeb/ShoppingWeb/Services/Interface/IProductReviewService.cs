using ShoppingWeb.Models;

namespace ShoppingWeb.Services.Interface
{
    public interface IProductReviewService
    {
        public Task<IEnumerable<ProductReview>> GetProductReviewsAsync(int productId);
        public Task<ProductReview> CreateProductReviewAsync(ProductReview review);
        public Task<ProductReview> UpdateProductReviewAsync(ProductReview review);
        public Task<bool> DeleteProductReviewAsync(int reviewId);
        public Task<int> GetProductReviewCountAsync(int productId);
        public Task<IEnumerable<ProductReview>> GetProductReviewsByUserAsync(string userId);
    }
}
