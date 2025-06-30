namespace ShoppingWeb.DTOs
{
    public class CreateProductDto
    {
        //public int ProductId { get; set; }

        public string ProductName { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public IFormFile? ImageFile { get; set; } // Cho phép upload ảnh
        public int? BrandId { get; set; } // Liên kết với Brand
        public int? CategoryId { get; set; } // Liên kết với Category
    }
}
