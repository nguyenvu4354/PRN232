using System;
using System.Collections.Generic;

namespace ShoppingWeb.Models;

public partial class Ward
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public int DistrictId { get; set; }

    public virtual ICollection<Cart> Carts { get; set; } = new List<Cart>();

    public virtual District District { get; set; } = null!;

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
