namespace ShoppingWeb.DTOs;

public class UserInfoDTO
{
    public int UserID { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
    public string? FullName { get; set; }
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public int RoleID { get; set; }
}