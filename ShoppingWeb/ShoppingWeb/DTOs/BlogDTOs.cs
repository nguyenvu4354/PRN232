namespace ShoppingWeb.DTOs
{
    public class CreateBlogDTO
    {
        public string Title { get; set; } = null!;
        public string Content { get; set; } = null!;
        public int AuthorId { get; set; }
    }

    public class UpdateBlogDTO
    {
        public int BlogId { get; set; }
        public string Title { get; set; } = null!;
        public string Content { get; set; } = null!;
    }
}
