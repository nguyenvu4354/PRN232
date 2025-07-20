using ShoppingWeb.DTOs.Auth;

namespace ShoppingWeb.Services.IServices;

public interface IAuthService
{
    Task<AuthResponseDTO> RegisterAsync(RegisterDTO registerRequest);
    Task<AuthResponseDTO> LoginAsync(LoginDTO loginRequest);
    Task<bool> ForgotPasswordAsync(string email);
    Task<bool> ResetPasswordAsync(ResetPasswordRequestDTO request);
    Task<AuthResponseDTO> RefreshTokenAsync(string refreshToken);
    Task LogoutAsync(string token);
    Task<bool> RevokeTokenAsync(string refreshToken);
    Task<bool> ValidateTokenAsync(string token);
}