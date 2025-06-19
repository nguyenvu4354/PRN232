namespace ShoppingWeb.DTOs;

public class AuthResponseDTO
{
    public string Token { get; set; }
    public string RefreshToken { get; set; }
    public DateTime Expires { get; set; }
    public UserInfoDTO User { get; set; }
}
