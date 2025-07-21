namespace ShoppingWeb.MvcClient.DTOs.User
{

    public class CreateUserRequestDTO
    {
        public string Username { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? FullName { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }

    }
}
