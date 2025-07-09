namespace ShoppingWeb.DTOs.Feedback
{
    public class FeedbackRequestDTO
    {
        public string Content { get; set; } = null!;
        public string Status { get; set; } = "Pending"; 
    }
}
