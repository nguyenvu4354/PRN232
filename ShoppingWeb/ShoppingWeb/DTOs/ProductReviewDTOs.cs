namespace ShoppingWeb.DTOs
{
    public class ProductReviewDTO
    {
        public int ProductId { get; set; }
        public int UserId { get; set; }
        public string ReviewText { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; }
    }
}
