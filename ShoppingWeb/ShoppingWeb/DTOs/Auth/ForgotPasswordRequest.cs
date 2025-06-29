using System.ComponentModel.DataAnnotations;

namespace ShoppingWeb.DTOs.Auth
{
    public class ForgotPasswordRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
