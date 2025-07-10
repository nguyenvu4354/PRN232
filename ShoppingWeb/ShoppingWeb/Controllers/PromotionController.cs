using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShoppingWeb.DTOs.Common;
using ShoppingWeb.DTOs.Promotion;
using ShoppingWeb.Exceptions;
using ShoppingWeb.Response;
using ShoppingWeb.Services.Interface;

namespace ShoppingWeb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PromotionController : ControllerBase
    {
        private readonly IPromotionService _promotionService;
        private readonly ILogger<PromotionController> _logger;

        public PromotionController(IPromotionService promotionService, ILogger<PromotionController> logger)
        {
            _promotionService = promotionService;
            _logger = logger;
        }

        [HttpGet("list")]
        public async Task<IActionResult> GetPaged([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var data = await _promotionService.GetPagedAsync(page, pageSize);
                return Ok(ApiResponse<PagedResultDTO<PromotionResponseDTO>>.SuccessResponse(data));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching paged promotions.");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    ApiResponse<string>.ErrorResponse("An error occurred", StatusCodes.Status500InternalServerError.ToString()));
            }
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var data = await _promotionService.GetByIdAsync(id);
                return Ok(ApiResponse<PromotionResponseDTO>.SuccessResponse(data));
            }
            catch (NotFoundException ex)
            {
                _logger.LogWarning(ex.Message);
                return NotFound(ApiResponse<string>.ErrorResponse(ex.Message));
            }
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] PromotionRequestDTO dto)
        {
            await _promotionService.CreateAsync(dto);
            return Ok(ApiResponse<string>.SuccessResponse("Promotion created successfully"));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] PromotionRequestDTO dto)
        {
            try
            {
                await _promotionService.UpdateAsync(id, dto);
                return Ok(ApiResponse<string>.SuccessResponse("Promotion updated successfully"));
            }
            catch (NotFoundException ex)
            {
                _logger.LogWarning(ex.Message);
                return NotFound(ApiResponse<string>.ErrorResponse(ex.Message));
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _promotionService.DeleteAsync(id);
                return Ok(ApiResponse<string>.SuccessResponse("Promotion deleted successfully"));
            }
            catch (NotFoundException ex)
            {
                _logger.LogWarning(ex.Message);
                return NotFound(ApiResponse<string>.ErrorResponse(ex.Message));
            }
        }

        [HttpGet("{id}/products")]
        public async Task<IActionResult> GetProductsInPromotion(int id)
        {
            try
            {
                var products = await _promotionService.GetProductsByPromotionIdAsync(id);
                return Ok(products);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting products in promotion");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("{id}/add-products")]
        public async Task<IActionResult> AddProductsToPromotion(int id, [FromBody] ProductPromotionRequestDTO dto)
        {
            if (dto == null || dto.ProductIds == null || !dto.ProductIds.Any())
                return BadRequest(ApiResponse<string>.ErrorResponse("No products provided"));

            dto.PromotionId = id;

            try
            {
                await _promotionService.AddProductsToPromotionAsync(dto);
                return Ok(ApiResponse<string>.SuccessResponse("Products added to promotion successfully"));
            }
            catch (NotFoundException ex)
            {
                _logger.LogWarning(ex.Message);
                return NotFound(ApiResponse<string>.ErrorResponse(ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding products to promotion");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    ApiResponse<string>.ErrorResponse("An error occurred"));
            }
        }

    }
}
