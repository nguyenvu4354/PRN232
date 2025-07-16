namespace ShoppingWeb.MvcClient.DTOs.Blog
{
    public class BlogPostDTO
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string ContentSummary { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public string? ImageUrl { get; set; }
    }
    public class BlogDetailDTO
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string? ImageUrl { get; set; }
    }
}
