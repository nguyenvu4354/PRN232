namespace ShoppingWeb.DTOs.Feedback
{
    public class FeedbackResponseDTO
    {
        public int FeedbackId { get; set; }
        public int UserId { get; set; }
        public string Username { get; set; } = null!;
        public string Content { get; set; } = null!;
        public string Status { get; set; } = null!;
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
