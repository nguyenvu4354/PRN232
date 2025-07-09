
using Microsoft.AspNetCore.Mvc;
using ShoppingWeb.Services.Interface;

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
        public async Task<IActionResult> AddToCart([FromBody] AddToCartRequest request)
        {
            if (request.Quantity <= 0)
            {
                return BadRequest("Quantity must be greater than zero.");
            }

            try
            {
                await cartService.AddToCartAsync(request.ProductId, request.Quantity, request.UserId);
                return Ok("Item added to cart successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpDelete("remove")]
        public async Task<IActionResult> RemoveFromCart([FromQuery] int productId, [FromQuery] int userId)
        {
            if (productId <= 0 || userId <= 0)
            {
                return BadRequest("Invalid product or user ID.");
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
        public async Task<IActionResult> GetCartItems([FromQuery] int userId)
        {
            if (userId <= 0)
            {
                return BadRequest("Invalid user ID.");
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
            try
            {
                await cartService.UpdateCartItemAsync(request.ProductId, request.Quantity, request.UserId);
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
        public int UserId { get; set; }
    }

    public class UpdateCartItemRequest
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public int UserId { get; set; }
    }
}