namespace ShoppingWeb.MvcClient.DTOs.User;

public class UserProfileUpdateDTO
{
    public string Username { get; set; }

    public string Email { get; set; }
    public string? FullName { get; set; }
    public string? Phone { get; set; }
    public string? Address { get; set; }
}