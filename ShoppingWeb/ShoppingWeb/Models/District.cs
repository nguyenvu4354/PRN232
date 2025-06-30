using System;
using System.Collections.Generic;

namespace ShoppingWeb.Models;

public partial class District
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public int ProvinceId { get; set; }

    public virtual ICollection<Cart> Carts { get; set; } = new List<Cart>();

    public virtual Province Province { get; set; } = null!;

    public virtual ICollection<User> Users { get; set; } = new List<User>();

    public virtual ICollection<Ward> Wards { get; set; } = new List<Ward>();
}
