namespace ShoppingWeb.Config;

public class JwtSettings
{
    public string SecretKey { get; set; }
    public string Issuer { get; set; }
    public string Audience { get; set; }
    public int ExpiryMinutes { get; set; } // For access tokens
    public int RefreshTokenExpiryDays { get; set; } // For refresh tokens
    public int ResetTokenExpiryMinutes { get; set; } // For password reset tokens
}