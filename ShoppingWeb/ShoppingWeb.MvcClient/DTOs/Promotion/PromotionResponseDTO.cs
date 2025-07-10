namespace ShoppingWeb.MvcClient.DTOs.Promotion
{
    public class PromotionResponseDTO
    {
        public int PromotionId { get; set; }
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public decimal? DiscountPercentage { get; set; }
        public decimal? DiscountAmount { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsActive { get; set; }
    }
}
