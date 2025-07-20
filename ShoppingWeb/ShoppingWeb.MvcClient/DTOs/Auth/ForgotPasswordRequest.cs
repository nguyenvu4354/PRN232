using System.ComponentModel.DataAnnotations;

namespace ShoppingWeb.MvcClient.DTOs.Auth;

public class ForgotPasswordRequest
{
    [Required]
    [EmailAddress]
    public string Email { get; set; }
}