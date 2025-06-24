using System.ComponentModel.DataAnnotations;

namespace ShoppingWeb.DTOs.Auth;

public class LoginDTO
{
    [Required(ErrorMessage = "Please enter your email!"), EmailAddress]
    public string Email { get; set; }

    [Required]
    public string Password { get; set; }
}