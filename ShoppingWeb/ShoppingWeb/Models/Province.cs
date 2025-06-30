using System;
using System.Collections.Generic;

namespace ShoppingWeb.Models;

public partial class Province
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<Cart> Carts { get; set; } = new List<Cart>();

    public virtual ICollection<District> Districts { get; set; } = new List<District>();

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
