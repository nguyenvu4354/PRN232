using System.ComponentModel.DataAnnotations;

namespace ShoppingWeb.DTOs
{
    public class ForgotPasswordRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
