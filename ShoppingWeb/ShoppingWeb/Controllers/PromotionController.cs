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
        public async Task<IActionResult> GetAll()
        {
            var data = await _promotionService.GetAllAsync();
            return Ok(ApiResponse<IEnumerable<PromotionResponseDTO>>.SuccessResponse(data));
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
    }
}
