using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShoppingWeb.DTOs.Statistics;
using ShoppingWeb.Response;
using ShoppingWeb.Services.Interface;

namespace ShoppingWeb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StatisticsController : ControllerBase
    {
        private readonly IStatisticsService _statisticsService;
        private readonly ILogger<StatisticsController> _logger;

        public StatisticsController(IStatisticsService statisticsService, ILogger<StatisticsController> logger)
        {
            _statisticsService = statisticsService;
            _logger = logger;
        }

        [HttpGet("summary")]
        public async Task<IActionResult> GetSummary()
        {
            try
            {
                var data = await _statisticsService.GetSummaryStatisticsAsync();
                _logger.LogInformation("Statistics summary fetched.");
                return Ok(ApiResponse<StatisticsResponseDTO>.SuccessResponse(data));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching statistics summary.");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    ApiResponse<string>.ErrorResponse("An error occurred", StatusCodes.Status500InternalServerError.ToString()));
            }
        }
        [HttpGet("weekly-product-sales")]
        public async Task<IActionResult> GetWeeklyProductSales()
        {
            try
            {
                var data = await _statisticsService.GetWeeklyProductSalesAsync();
                _logger.LogInformation("Weekly product sales statistics fetched.");
                return Ok(ApiResponse<List<WeeklyProductSalesDTO>>.SuccessResponse(data));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching weekly product sales statistics.");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    ApiResponse<string>.ErrorResponse("An error occurred", StatusCodes.Status500InternalServerError.ToString()));
            }
        }

    }
}
