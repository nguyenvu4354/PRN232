using Microsoft.AspNetCore.Mvc;
using ShoppingWeb.DTOs;
using ShoppingWeb.Response;
using ShoppingWeb.Services.IServices;
using System.Net;

namespace ShoppingWeb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService ?? throw new ArgumentNullException(nameof(authService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpPost("register")]
        [ProducesResponseType(typeof(ApiResponse<AuthResponseDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> RegisterAsync(RegisterDTO registerDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ApiResponse<object>.ErrorResponse("Invalid model state",
                        HttpStatusCode.BadRequest.ToString()));
                }
                var response = await _authService.RegisterAsync(registerDto);

                return Ok(ApiResponse<AuthResponseDTO>.SuccessResponse(response, "Register successful"));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse(ex.Message,
                    HttpStatusCode.BadRequest.ToString()));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during registration");
                return StatusCode(500, ApiResponse<object>.ErrorResponse("Internal server error", "500"));

            }
        }

        [HttpPost("login")]
        [ProducesResponseType(typeof(ApiResponse<AuthResponseDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> LoginAsync(LoginDTO loginDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ApiResponse<object>.ErrorResponse("Invalid model state",
                        HttpStatusCode.BadRequest.ToString()));
                }
                var response = await _authService.LoginAsync(loginDto);

                return Ok(ApiResponse<AuthResponseDTO>.SuccessResponse(response, "Login successful"));
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ApiResponse<object>.ErrorResponse(ex.Message, HttpStatusCode.Unauthorized.ToString()));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during login");
                return StatusCode(500, ApiResponse<object>.ErrorResponse("Internal server error", "500"));
            }
        }

    }
}
