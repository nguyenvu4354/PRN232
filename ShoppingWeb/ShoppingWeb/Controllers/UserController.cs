using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ShoppingWeb.DTOs.User;
using ShoppingWeb.Enum;
using ShoppingWeb.Services.Interface;

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
            _logger = logger;
            _userService = userService;
            _httpContextAccessor = httpContextAccessor;
        }
        [HttpGet]
        [Authorize(Roles = "ADMIN")]
        public string TestData()
        {
            return "Hello world";
        }

        [HttpPost("change-password")]
        [Authorize(Roles = "CUSTOMER")]
        public async Task<IActionResult> ChangePasswordAsync(ChangePasswordRequestDTO requestDTO)
        {

            var userIdStr = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdStr, out var userId))
                return BadRequest("Invalid user ID");


            var result = await _userService.ChangePassowrdAsync(userId, requestDTO);

            if (!result)
                return BadRequest(result); // Hoặc return StatusCode(403), tùy tình huống

            return Ok("Password changed successfully.");
        }

        [HttpGet("profile")]
        [Authorize(Roles = "CUSTOMER")]
        public async Task<IActionResult> ViewProfile()
        {
            try
            {
                var userIdStr = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (!int.TryParse(userIdStr, out var userId))
                    return BadRequest("Invalid user ID");

                var data = await _userService.ViewProfileAsync(userId);

                return data == null ? BadRequest("ko cos data") : Ok(data);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        
        }

        [HttpPost("update-profile")]
        [Authorize(Roles = "CUSTOMER")]
        public async Task<IActionResult> UpdateProfile(UpdateUserProfileRequestDTO requestDTO)
        {
            var userIdStr = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdStr, out var userId))
                return BadRequest("Invalid user ID");
            var data = await _userService.UpdateProfileAsync(userId,requestDTO);

            return data == null ? BadRequest("ko cos data") : Ok(data);
        }
    }
}
