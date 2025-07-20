using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShoppingWeb.DTOs.Common;
using ShoppingWeb.DTOs.User;
using ShoppingWeb.Exceptions;
using ShoppingWeb.Response;
using ShoppingWeb.Services.Interface;
using System.Security.Claims;

namespace ShoppingWeb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<UserController> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserController(IUserService userService, ILogger<UserController> logger, IHttpContextAccessor httpContextAccessor)
        {
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
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
        

        [HttpPost("change-password")]
        [Authorize]
        public async Task<IActionResult> ChangePasswordAsync([FromBody] ChangePasswordRequestDTO requestDTO)
        {
            if (requestDTO == null)
            {
                _logger.LogWarning("Change password request DTO is null.");
                return BadRequest(ApiResponse<string>.ErrorResponse("Request cannot be null", StatusCodes.Status400BadRequest.ToString()));
            }


            var userId = GetUserIdOrThrow();
            try
            {
                await _userService.ChangePasswordAsync(userId, requestDTO);
                _logger.LogInformation("Password changed successfully for user ID {UserId}.", userId);
                return Ok(ApiResponse<string>.SuccessResponse("Password changed successfully"));
            }
            catch (UserNotFoundException ex)
            {
                _logger.LogWarning("User not found for ID {UserId}: {Message}", userId, ex.Message);
                return NotFound(ApiResponse<string>.ErrorResponse("User not found", StatusCodes.Status404NotFound.ToString()));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error changing password for user ID {UserId}.", userId);
                return StatusCode(StatusCodes.Status500InternalServerError, ApiResponse<string>.ErrorResponse("An error occurred", StatusCodes.Status500InternalServerError.ToString()));
            }

        }

        [HttpGet("profile")]
        [Authorize]
        public async Task<IActionResult> ViewProfile()
        {

            var userId = GetUserIdOrThrow();
            try
            {
                var data = await _userService.ViewProfileAsync(userId);
                _logger.LogInformation("Profile retrieved successfully for user ID {UserId}.", userId);
                return Ok(ApiResponse<UserProfileResponseDTO>.SuccessResponse(data));
            }
            catch (UserNotFoundException ex)
            {
                _logger.LogWarning("User not found for ID {UserId}: {Message}", userId, ex.Message);
                return NotFound(ApiResponse<string>.ErrorResponse("User not found", StatusCodes.Status404NotFound.ToString()));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving profile for user ID {UserId}.", userId);
                return StatusCode(StatusCodes.Status500InternalServerError, ApiResponse<string>.ErrorResponse("An error occurred", StatusCodes.Status500InternalServerError.ToString()));
            }


        }

        [HttpPost("update-profile")]
        [Authorize]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateUserProfileRequestDTO requestDTO)
        {
            if (requestDTO == null)
            {
                _logger.LogWarning("Update profile request DTO is null.");
                return BadRequest(ApiResponse<string>.ErrorResponse("Request cannot be null", StatusCodes.Status400BadRequest.ToString()));
            }

            var userId = GetUserIdOrThrow();
            try
            {
                var data = await _userService.UpdateProfileAsync(userId, requestDTO);
                _logger.LogInformation("Profile updated successfully for user ID {UserId}.", userId);
                return Ok(ApiResponse<UserProfileResponseDTO>.SuccessResponse(data));
            }
            catch (UserNotFoundException ex)
            {
                _logger.LogWarning("User not found for ID {UserId}: {Message}", userId, ex.Message);
                return NotFound(ApiResponse<string>.ErrorResponse("User not found", StatusCodes.Status404NotFound.ToString()));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating profile for user ID {UserId}.", userId);
                return StatusCode(StatusCodes.Status500InternalServerError, ApiResponse<string>.ErrorResponse("An error occurred", StatusCodes.Status500InternalServerError.ToString()));
            }
        }
        [HttpGet("paged")]
        public async Task<IActionResult> GetPagedUsers([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var data = await _userService.GetUsersPagedAsync(page, pageSize);
            return Ok(ApiResponse<PagedResultDTO<UserListItemResponseDTO>>.SuccessResponse(data));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserDetail(int id)
        {
            try
            {
                var user = await _userService.GetUserDetailByIdAsync(id);
                return Ok(ApiResponse<UserListItemResponseDTO>.SuccessResponse(user));
            }
            catch (UserNotFoundException ex)
            {
                _logger.LogWarning("User with ID {UserId} not found: {Message}", id, ex.Message);
                return NotFound(ApiResponse<string>.ErrorResponse("User not found", StatusCodes.Status404NotFound.ToString()));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching user detail.");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    ApiResponse<string>.ErrorResponse("An error occurred", StatusCodes.Status500InternalServerError.ToString()));
            }
        }
        [HttpGet("search")]
        public async Task<IActionResult> SearchUsersByUsername([FromQuery] string username)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                return BadRequest(ApiResponse<string>.ErrorResponse("Username cannot be empty", StatusCodes.Status400BadRequest.ToString()));
            }

            try
            {
                var users = await _userService.SearchUsersByUsernameAsync(username);
                return Ok(ApiResponse<IEnumerable<UserListItemResponseDTO>>.SuccessResponse(users));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching users by username.");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    ApiResponse<string>.ErrorResponse("An error occurred", StatusCodes.Status500InternalServerError.ToString()));
            }
        }

        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateUserStatus(int id, [FromBody] UpdateUserStatusDTO statusDto)
        {
            if (statusDto == null)
            {
                _logger.LogWarning("UpdateUserStatus request body is null.");
                return BadRequest(ApiResponse<string>.ErrorResponse("Request cannot be null", StatusCodes.Status400BadRequest.ToString()));
            }

            try
            {
                var result = await _userService.UpdateUserStatusAsync(id, statusDto.IsActive);
                return Ok(ApiResponse<string>.SuccessResponse("User status updated successfully"));
            }
            catch (UserNotFoundException ex)
            {
                _logger.LogWarning("User with ID {UserId} not found: {Message}", id, ex.Message);
                return NotFound(ApiResponse<string>.ErrorResponse("User not found", StatusCodes.Status404NotFound.ToString()));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user status for ID {UserId}.", id);
                return StatusCode(StatusCodes.Status500InternalServerError,
                    ApiResponse<string>.ErrorResponse("An error occurred", StatusCodes.Status500InternalServerError.ToString()));
            }
        }

        [HttpPost("create-staff")]
        public async Task<IActionResult> CreateStaffUser([FromBody] CreateUserRequestDTO requestDTO)
        {
            if (requestDTO == null)
            {
                _logger.LogWarning("CreateUser request DTO is null.");
                return BadRequest(ApiResponse<string>.ErrorResponse("Request cannot be null", StatusCodes.Status400BadRequest.ToString()));
            }

            try
            {
                var user = await _userService.CreateStaffUserAsync(requestDTO);
                return Ok(ApiResponse<UserListItemResponseDTO>.SuccessResponse(user, "Staff account created successfully."));
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning("Validation error: {Message}", ex.Message);
                return BadRequest(ApiResponse<string>.ErrorResponse(ex.Message, StatusCodes.Status400BadRequest.ToString()));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating staff account.");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    ApiResponse<string>.ErrorResponse("An error occurred", StatusCodes.Status500InternalServerError.ToString()));
            }
        }


    }
}
