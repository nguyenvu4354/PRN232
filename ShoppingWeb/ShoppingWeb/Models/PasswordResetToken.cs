using System;
using System.Collections.Generic;

namespace ShoppingWeb.Models;

public partial class PasswordResetToken
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public string? Token { get; set; }

    public DateTime? ExpiresAt { get; set; }

    public bool? Used { get; set; }

    public virtual User User { get; set; } = null!;
}
