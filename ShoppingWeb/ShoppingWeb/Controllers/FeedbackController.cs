using Microsoft.AspNetCore.Mvc;
using ShoppingWeb.DTOs.Feedback;
using ShoppingWeb.Services.Interface;
using ShoppingWeb.Response;

namespace ShoppingWeb.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FeedbackController : ControllerBase
    {
        private readonly IFeedbackService _feedbackService;
        private readonly ILogger<FeedbackController> _logger;

        public FeedbackController(IFeedbackService feedbackService, ILogger<FeedbackController> logger)
        {
            _feedbackService = feedbackService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var feedbacks = await _feedbackService.GetAllAsync();
            return Ok(new ApiResponse<IEnumerable<FeedbackResponseDTO>>(feedbacks));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var feedback = await _feedbackService.GetByIdAsync(id);
            return Ok(new ApiResponse<FeedbackResponseDTO>(feedback));
        }

        [HttpPost("{userId}")]
        public async Task<IActionResult> Create(int userId, [FromBody] FeedbackRequestDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse<string>("Invalid input data"));
            }

            var result = await _feedbackService.CreateAsync(userId, dto);
            return Ok(new ApiResponse<FeedbackResponseDTO>(result, "Feedback created successfully"));
        }

        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateStatus(int id, [FromQuery] string status)
        {
            await _feedbackService.UpdateStatusAsync(id, status);
            return Ok(new ApiResponse<string>("Status updated successfully"));
        }
    }
}
