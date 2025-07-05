using Microsoft.EntityFrameworkCore;
using ShoppingWeb.DTOs;
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
        public async Task<ProductReview> CreateProductReviewAsync(ProductReviewDTO review)
        {
            List<OrderDetail> orderdetails = _context.OrderDetails.
                Where(od => od.ProductId == review.ProductId && od.Cart.UserId == review.UserId)
                .ToList();
            if (orderdetails.Count == 0)
            {
                throw new InvalidOperationException("User has not purchased this product.");
            }
            var reviewAdd = new ProductReview
            {
                ProductId = review.ProductId,
                UserId = review.UserId,
                Rating = review.Rating,
                Comment = review.Comment,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            _context.ProductReviews.Add(reviewAdd);
            await _context.SaveChangesAsync();
            return reviewAdd;
        }

        public async Task<bool> DeleteProductReviewAsync(int reviewId)
        {
            var review = await _context.ProductReviews.FindAsync(reviewId);
            if (review == null)
            {
                return false;
            }

            _context.ProductReviews.Remove(review);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<int> GetProductReviewCountAsync(int productId)
        {
            return await _context.ProductReviews.CountAsync(r => r.ProductId == productId);
        }

        public async Task<IEnumerable<ProductReview>> GetProductReviewsAsync(int productId)
        {
            return await _context.ProductReviews
                .Where(r => r.ProductId == productId)
                .ToListAsync();
        }

        public async Task<IEnumerable<ProductReview>> GetProductReviewsByUserAsync(int userId)
        {
            return await _context.ProductReviews
                .Where(r => r.UserId == userId)
                .ToListAsync();
        }

        public async Task<ProductReview> UpdateProductReviewAsync(ProductReview review)
        {
            var existingReview = await _context.ProductReviews.FindAsync(review.ReviewId);
            if (existingReview == null)
            {
                throw new KeyNotFoundException("Review not found.");
            }

            existingReview.Rating = review.Rating;
            existingReview.Comment = review.Comment;
            existingReview.UpdatedAt = DateTime.UtcNow;

            _context.ProductReviews.Update(existingReview);
            await _context.SaveChangesAsync();
            return existingReview;
        }
    }
}
