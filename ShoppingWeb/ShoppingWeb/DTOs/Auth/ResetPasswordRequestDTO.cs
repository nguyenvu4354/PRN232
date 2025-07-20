using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ShoppingWeb.DTOs.Auth;

public class ResetPasswordRequestDTO
{

    [Required(ErrorMessage = "Token is required")]
    public string TokenResetPassword { get; set; }

    [PasswordPropertyText]
    [Required(ErrorMessage = "New password is required")]
    [StringLength(100, ErrorMessage = "Password must be at least 6 characters long.", MinimumLength = 8)]
    public string NewPassword { get; set; }
}