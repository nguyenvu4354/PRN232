namespace ShoppingWeb.MvcClient.DTOs.Promotion
{
    public class PromotionRequestDTO
    {
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public decimal? DiscountPercentage { get; set; }
        public decimal? DiscountAmount { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
