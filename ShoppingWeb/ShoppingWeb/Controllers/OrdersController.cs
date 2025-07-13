using MailKit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShoppingWeb.DTOs.Common;
using ShoppingWeb.DTOs.Order;
using ShoppingWeb.Exceptions;
using ShoppingWeb.Response;
using ShoppingWeb.Services.Interface;
using System.Security.Claims;

namespace ShoppingWeb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly ILogger<OrdersController> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public OrdersController(IOrderService orderService, ILogger<OrdersController> logger, IHttpContextAccessor httpContextAccessor)
        {
            _orderService = orderService;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }

        protected int GetUserIdOrThrow()
        {
            var userIdStr = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdStr, out var userId))
            {
                throw new UnauthorizedAccessException("User ID not found in claims.");
            }
            return userId;
        }

        [HttpGet("paged")]
        public async Task<IActionResult> GetPagedOrders([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var data = await _orderService.GetOrdersPagedAsync(page, pageSize);
                return Ok(ApiResponse<PagedResultDTO<OrderResponseDTO>>.SuccessResponse(data));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching paged orders.");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    ApiResponse<string>.ErrorResponse("An error occurred", StatusCodes.Status500InternalServerError.ToString()));
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrderById(int id)
        {
            try
            {
                var order = await _orderService.GetOrderByIdAsync(id);
                return Ok(ApiResponse<OrderResponseDTO>.SuccessResponse(order));
            }
            catch (OrderNotFoundException ex)
            {
                _logger.LogWarning("Order with ID {OrderId} not found: {Message}", id, ex.Message);
                return NotFound(ApiResponse<string>.ErrorResponse("Order not found", StatusCodes.Status404NotFound.ToString()));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching order by ID.");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    ApiResponse<string>.ErrorResponse("An error occurred", StatusCodes.Status500InternalServerError.ToString()));
            }
        }
        [HttpPut("OrderDetail{id}/status")]
        public async Task<IActionResult> UpdateOrderStatus(int id, [FromBody] UpdateOrderStatusDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Status))
            {
                return BadRequest(ApiResponse<string>.ErrorResponse("Status is required", StatusCodes.Status400BadRequest.ToString()));
            }

            try
            {
                await _orderService.UpdateOrderStatusAsync(id, dto.Status);
                return Ok(ApiResponse<string>.SuccessResponse("Order status updated successfully"));
            }
            catch (OrderNotFoundException ex)
            {
                _logger.LogWarning("Order not found when updating status: {Message}", ex.Message);
                return NotFound(ApiResponse<string>.ErrorResponse("Order not found", StatusCodes.Status404NotFound.ToString()));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating order status.");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    ApiResponse<string>.ErrorResponse("An error occurred", StatusCodes.Status500InternalServerError.ToString()));
            }
        }

    }
}
