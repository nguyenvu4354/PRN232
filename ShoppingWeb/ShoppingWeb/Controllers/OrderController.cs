using Microsoft.AspNetCore.Mvc;
using ShoppingWeb.DTOs;
using ShoppingWeb.Services.Interface;
using ShoppingWeb.Services.ThirdParty;

namespace ShoppingWeb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : Controller
    {
        private readonly ICartService _cartService;
        public OrderController(ICartService cartService)
        {
            _cartService = cartService;
        }

        [HttpPost("create-order")]
        public async Task<IActionResult> CreateOrder([FromBody] ToOrderDTO toOrderDTO)
        {
            if (toOrderDTO == null || !ModelState.IsValid)
            {
                return BadRequest("Invalid order data.");
            }
            try
            {
                await _cartService.ToOrderAsync(toOrderDTO);
                return Ok("Order created successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("order-status/{cartId}")]
        public async Task<IActionResult> GetOrderStatus(int cartId)
        {
            if (cartId <= 0)
            {
                return BadRequest("Invalid cart ID.");
            }
            try
            {
                var orderStatus = await _cartService.GetOrderStatus(cartId);
                return Ok(orderStatus);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        [HttpGet("shipping-fee")]
        public async Task<IActionResult> GetShippingFee(string wardCode, int districtId, int weight)
        {
            if (string.IsNullOrEmpty(wardCode) || districtId <= 0 || weight <= 0)
            {
                return BadRequest("Invalid shipping details.");
            }
            try
            {
                var shippingFee = await _cartService.GetShippingFee(wardCode, districtId, weight);
                return Ok(shippingFee);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        [HttpGet("payment-info/{cartId}")]
        public async Task<IActionResult> GetPaymentInfo(int cartId)
        {
            if (cartId <= 0)
            {
                return BadRequest("Invalid cart ID.");
            }
            try
            {
                var paymentInfo = await _cartService.GetPaymentInfo(cartId);
                return Ok(paymentInfo);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        [HttpPost("create-payment")]
        public async Task<IActionResult> CreatePayment([FromBody] CreatePaymentRequest request)
        {
            if (request == null || !ModelState.IsValid)
            {
                return BadRequest("Invalid payment request.");
            }
            try
            {
                var response = await _cartService.CreatePayment(request);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
