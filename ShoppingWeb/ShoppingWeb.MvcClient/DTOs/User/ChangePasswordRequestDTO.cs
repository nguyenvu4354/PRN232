using System.ComponentModel.DataAnnotations;

namespace ShoppingWeb.MvcClient.DTOs.User;

public class ChangePasswordRequestDTO
{
    [Required(ErrorMessage = "Old password is required.")]
    public string OldPassword { get; set; }

    [Required(ErrorMessage = "New password is required.")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "New password must be at least 6 characters.")]
    public string NewPassword { get; set; }

    [Required(ErrorMessage = "Please confirm your new password.")]
    [Compare("NewPassword", ErrorMessage = "Confirm password does not match.")]
    public string ConfirmPassword { get; set; }
}