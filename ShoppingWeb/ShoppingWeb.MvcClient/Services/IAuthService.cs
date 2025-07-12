using ShoppingWeb.MvcClient.DTOs.Auth;
using ShoppingWeb.MvcClient.Models;
using ShoppingWeb.MvcClient.Response;

namespace ShoppingWeb.MvcClient.Services;

public interface IAuthService
{
    Task<ApiResponse<AuthResponseDto>> LoginAsync(LoginViewModel model);
    Task<ApiResponse<AuthResponseDto>> RegisterAsync(RegisterViewModel model);
    Task<bool> LogoutAsync(string accessToken);
}