using Microsoft.AspNetCore.Mvc;
using ShoppingWeb.DTOs;
using ShoppingWeb.Models;
using ShoppingWeb.Services.Interface;

namespace ShoppingWeb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductReviewController : Controller
    {
        private readonly IProductReviewService _productReviewService;

        public ProductReviewController(IProductReviewService productReviewService)
        {
            _productReviewService = productReviewService;
        }

        [HttpGet("productreviews/{productId}")]
        public async Task<IActionResult> GetProductReviews(int productId)
        {
            var reviews = await _productReviewService.GetProductReviewsAsync(productId);
            return Ok(reviews);
        }


        [HttpPost("productreviews")]
        public async Task<IActionResult> CreateProductReview([FromBody] ProductReviewDTO review)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var createdReview = await _productReviewService.CreateProductReviewAsync(review);
            return CreatedAtAction(nameof(GetProductReviews), new { productId = createdReview.ProductId }, createdReview);
        }

        [HttpPut("productreviews/{reviewId}")]
        public async Task<IActionResult> UpdateProductReview(int reviewId, [FromBody] ProductReview review)
        {
            if (reviewId != review.ReviewId)
                return BadRequest("Review ID mismatch.");

            var updatedReview = await _productReviewService.UpdateProductReviewAsync(review);
            return Ok(updatedReview);
        }

        [HttpDelete("productreviews/{reviewId}")]
        public async Task<IActionResult> DeleteProductReview(int reviewId)
        {
            var result = await _productReviewService.DeleteProductReviewAsync(reviewId);
            if (!result)
                return NotFound();

            return NoContent();
        }

        [HttpGet("productreviews/count/{productId}")]
        public async Task<IActionResult> GetProductReviewCount(int productId)
        {
            var count = await _productReviewService.GetProductReviewCountAsync(productId);
            return Ok(count);
        }

        [HttpGet("productreviews/user/{userId}")]
        public async Task<IActionResult> GetProductReviewsByUser(int userId)
        {
            var reviews = await _productReviewService.GetProductReviewsByUserAsync(userId);
            return Ok(reviews);
        }
    }
}
