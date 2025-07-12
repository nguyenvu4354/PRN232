using ShoppingWeb.DTOs;

namespace ShoppingWeb.MvcClient.DTOs.Auth;

public class AuthResponseDto
{
    public string Token { get; set; }
    public string RefreshToken { get; set; }
    public DateTime Expires { get; set; }
    public UserInfoDTO User { get; set; }
}