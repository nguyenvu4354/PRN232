using System;

namespace ShoppingWeb.Models;

public partial class ProductPromotion
{
    public int ProductPromotionId { get; set; }

    public int ProductId { get; set; }

    public int PromotionId { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Product Product { get; set; } = null!;

    public virtual Promotion Promotion { get; set; } = null!;
}
