using Microsoft.AspNetCore.Mvc;
using ShoppingWeb.Services.Interface;

namespace ShoppingWeb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WishListController : Controller
    {
        private readonly IWishListService _wishListService;
        public WishListController(IWishListService wishListService)
        {
            _wishListService = wishListService;
        }
        [HttpPost("AddToWishList")]
        public async Task<IActionResult> AddToWishList(int userId, int productId)
        {
            var result = await _wishListService.AddToWishList(userId, productId);
            if (result)
            {
                return Ok("Item added to wishlist successfully.");
            }
            return BadRequest("Failed to add item to wishlist.");
        }
        [HttpGet("GetWishList")]
        public async Task<IActionResult> GetWishList(int userId)
        {
            try
            {
                var wishList = await _wishListService.GetWishList(userId);
                return Ok(wishList);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error retrieving wishlist: {ex.Message}");
            }
        }
        [HttpGet("IsInWishList")]
        public async Task<IActionResult> IsInWishList(int userId, int productId)
        {
            try
            {
                var isInWishList = await _wishListService.IsInWishList(userId, productId);
                return Ok(isInWishList);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error checking wishlist: {ex.Message}");
            }
        }
        [HttpDelete("RemoveFromWishList")]
        public async Task<IActionResult> RemoveFromWishList(int userId, int productId)
        {
            try
            {
                var result = await _wishListService.RemoveFromWishList(userId, productId);
                if (result)
                {
                    return Ok("Item removed from wishlist successfully.");
                }
                return BadRequest("Failed to remove item from wishlist.");
            }
            catch (Exception ex)
            {
                return BadRequest($"Error removing item from wishlist: {ex.Message}");
            }
        }

    }
}
