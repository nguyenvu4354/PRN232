using System.ComponentModel.DataAnnotations;

namespace ShoppingWeb.DTOs.User
{
    public class UpdateUserProfileRequestDTO
    {

        public string Username { get; set; }

        public string Email { get; set; }
        [Required(ErrorMessage ="full name require")]
        public string? FullName { get; set; }
        [Required(ErrorMessage ="Phone require")]
        [Phone]
        public string? Phone { get; set; }
        [Required(ErrorMessage = "address require")]
        public string? Address { get; set; }

    }
}
