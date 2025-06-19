using ShoppingWeb.DTOs;

namespace ShoppingWeb.Services.IServices;

public interface IAuthService
{
    Task<AuthResponseDTO> RegisterAsync(RegisterDTO registerRequest);
    Task<AuthResponseDTO> LoginAsync(LoginDTO loginRequest);

    Task ForgotPasswordAsync(string email);
    Task<AuthResponseDTO> RefreshTokenAsync(string token, string refreshToken);
    Task LogoutAsync(string token);
    Task<bool> RevokeTokenAsync(string refreshToken);
    Task<bool> ValidateTokenAsync(string token);
}