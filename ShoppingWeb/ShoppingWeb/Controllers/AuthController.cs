using Microsoft.AspNetCore.Mvc;
using ShoppingWeb.DTOs.Auth;
using ShoppingWeb.Response;
using ShoppingWeb.Services.Interface;
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
        private readonly IEmailService _emailService;

        public AuthController(IAuthService authService, ILogger<AuthController> logger, IEmailService emailService)
        {
            _authService = authService ?? throw new ArgumentNullException(nameof(authService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _emailService = emailService ?? throw new ArgumentNullException(nameof(_emailService));
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
                await _emailService.SendWelcomeEmailAsync(response.User.Email, response.User.FullName,null);
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

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword(string email)
        {
            await _authService.ForgotPasswordAsync(email);
            return Ok(ApiResponse<string>.SuccessResponse(null,"The request reset password have been sending to your email. Please check!"));
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword(string tokenResetPassword, string newPassword)
        {
            try
            {
               var stateResetPassword = await _authService.ResetPasswordAsync(tokenResetPassword, newPassword);
                if (!stateResetPassword)
                {
                    return BadRequest(ApiResponse<string>.ErrorResponse("Password reset faild",HttpStatusCode.BadRequest.ToString()));
                }
            }catch (Exception e)
            {
                _logger.LogError($"{e.Message}");
                throw;
            }
            return Ok(ApiResponse<string>.SuccessResponse(null,"Password reset successfully"));
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            string token = HttpContext.Request.Headers["Authorization"].ToString();
            if (string.IsNullOrEmpty(token))
            {
                _logger.LogWarning("Logout request without token.");
                return BadRequest(ApiResponse<string>.ErrorResponse(null, HttpStatusCode.BadRequest.ToString()));
            }
            await _authService.LogoutAsync(token);
            return NoContent();
        }

    }
}
