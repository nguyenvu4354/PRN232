using System;
using System.Collections.Generic;

namespace ShoppingWeb.Models;

public partial class Promotion
{
    public int PromotionId { get; set; }
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    public decimal? DiscountPercentage { get; set; }
    public decimal? DiscountAmount { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public bool IsDisabled { get; set; } = false;
    public virtual ICollection<ProductPromotion> ProductPromotions { get; set; } = new List<ProductPromotion>();
}
