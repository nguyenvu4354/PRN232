using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ShoppingWeb.DTOs.Auth;

public class RegisterDTO
{
    [Required, MinLength(6)]
    public string Username { get; set; } = null!;
    [Required, MinLength(8)]
    public string Password { get; set; } = null!;
    [Required, EmailAddress]
    public string Email { get; set; } = null!;
    [Required, MinLength(6)]
    public string? FullName { get; set; }
    [Required, Phone]
    public string? Phone { get; set; } // may be send opt code



}