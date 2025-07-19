
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShoppingWeb.Services.Interface;
using System.Security.Claims;

namespace ShoppingWeb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : Controller
    {
        private readonly ICartService cartService;
        private readonly IProductService productService;

        public CartController(ICartService cartService, IProductService productService)
        {
            this.cartService = cartService;
            this.productService = productService;
        }

        [HttpPost("add")]
        [Authorize]
        public async Task<IActionResult> AddToCart([FromBody] AddToCartRequest request)
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdString, out var userId))
            {
                return Unauthorized("User is not authenticated.");
            }
            if (request.Quantity <= 0)
            {
                return BadRequest("Quantity must be greater than zero.");
            }
            try
            {
                await cartService.AddToCartAsync(request.ProductId, request.Quantity, userId);
                return Ok("Item added to cart successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpDelete("remove")]
        [Authorize]
        public async Task<IActionResult> RemoveFromCart([FromQuery] int productId)
        {
            if (productId <= 0)
            {
                return BadRequest("Invalid product or user ID.");
            }
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdString, out var userId))
            {
                return Unauthorized("User is not authenticated.");
            }
            try
            {
                await cartService.RemoveFromCartAsync(productId, userId);
                return Ok("Item removed from cart successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpDelete("clear")]
        public async Task<IActionResult> ClearCart([FromQuery] int userId)
        {
            if (userId <= 0)
            {
                return BadRequest("Invalid user ID.");
            }

            try
            {
                await cartService.ClearCartAsync(userId);
                return Ok("Cart cleared successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("items")]
        [Authorize]
        public async Task<IActionResult> GetCartItems()
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdString, out var userId))
            {
                return Unauthorized("User is not authenticated.");
            }

            try
            {
                var items = await cartService.GetCartItemsAsync(userId);
                return Ok(items);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("total-price")]
        public async Task<IActionResult> GetTotalPrice([FromQuery] int userId)
        {
            if (userId <= 0)
            {
                return BadRequest("Invalid user ID.");
            }

            try
            {
                var totalPrice = await cartService.GetTotalPriceAsync(userId);
                return Ok(totalPrice);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPut("update")]
        public async Task<IActionResult> UpdateCartItem([FromBody] UpdateCartItemRequest request)
        {
            if (request.Quantity <= 0)
            {
                return BadRequest("Quantity must be greater than zero.");
            }
            var product = await productService.GetProductByIdAsync(request.ProductId);
            if(product == null)
            {
                return NotFound("Product not found.");
            }
            if(product.StockQuantity < request.Quantity)
            {
                return BadRequest("Insufficient stock for the requested quantity.");
            }
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdString, out var userId))
            {
                return Unauthorized("User is not authenticated.");
            }
            try
            {
                await cartService.UpdateCartItemAsync(request.ProductId, request.Quantity, userId);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }

    public class AddToCartRequest
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }

    public class UpdateCartItemRequest
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }
}