namespace ShoppingWeb.DTOs.Promotion
{
    public class ProductPromotionRequestDTO
    {
        public int PromotionId { get; set; }
        public List<int> ProductIds { get; set; } = new List<int>();
    }
}
