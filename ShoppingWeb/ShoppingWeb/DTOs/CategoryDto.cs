namespace ShoppingWeb.DTOs
{
    public class CategoryDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public bool IsDisabled { get; set; } // Thêm trường IsDisabled
    }
}
