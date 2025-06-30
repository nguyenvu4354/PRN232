using ShoppingWeb.Models;
using ShoppingWeb.Services.Interface;

namespace ShoppingWeb.Services
{
    public class ProductReviewService : IProductReviewService
    {
        private readonly ShoppingWebContext _context;
        public ProductReviewService(ShoppingWebContext context)
        {
            _context = context;
        }
        public Task<ProductReview> CreateProductReviewAsync(ProductReview review)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteProductReviewAsync(int reviewId)
        {
            throw new NotImplementedException();
        }

        public Task<int> GetProductReviewCountAsync(int productId)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<ProductReview>> GetProductReviewsAsync(int productId)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<ProductReview>> GetProductReviewsByUserAsync(string userId)
        {
            throw new NotImplementedException();
        }

        public Task<ProductReview> UpdateProductReviewAsync(ProductReview review)
        {
            throw new NotImplementedException();
        }
    }
}
