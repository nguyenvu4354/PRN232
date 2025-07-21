namespace ShoppingWeb.DTOs
{
    public class ProductDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public string? ImageUrl { get; set; }
        public string? BrandName { get; set; } // Thêm trường BrandName
        public string? CategoryName { get; set; } // Thêm trường CategoryName

        public int? BrandId { get; set; } // Thêm trường BrandId
        public int? CategoryId { get; set; } // Thêm trường CategoryId
    }

    public class ProductListItemResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string Promotion { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public string Brand { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
    }
    public class ProductDetailItemDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public int? BrandId { get; set; }
        public string Brand { get; set; } = string.Empty;
        public int? CategoryId { get; set; }
        public string Category { get; set; } = string.Empty;
    }
}
