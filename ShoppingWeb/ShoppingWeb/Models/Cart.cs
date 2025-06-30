using System;
using System.Collections.Generic;

namespace ShoppingWeb.Models;

public partial class Cart
{
    public int CartId { get; set; }

    public int UserId { get; set; }

    public DateTime? OrderDate { get; set; }

    public decimal TotalAmount { get; set; }

    public bool IsCart { get; set; }

    public string ShippingAddress { get; set; } = null!;
    public int? ProvinceId { get; set; } = null!;
    public int? DistrictId { get; set; } = null!;
    public int? WardId { get; set; } = null!;

    public int? ProvinceId { get; set; }

    public int? DistrictId { get; set; }

    public int? WardId { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual District? District { get; set; }

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

    public virtual Province? Province { get; set; }

    public virtual User User { get; set; } = null!;

    public virtual Ward? Ward { get; set; }
}
