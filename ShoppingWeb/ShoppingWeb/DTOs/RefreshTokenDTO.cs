using System.ComponentModel.DataAnnotations;

namespace ShoppingWeb.DTOs;

public class RefreshTokenDTO
{
    [Required]
    public string RefreshToken { get; set; }
}