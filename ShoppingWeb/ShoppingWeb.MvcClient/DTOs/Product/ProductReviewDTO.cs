﻿namespace ShoppingWeb.MvcClient.DTOs.Product
{
    public class ProductReviewDto
    {
        public int ReviewId { get; set; }
        public int ProductId { get; set; }
        public int UserId { get; set; }
        public int? Rating { get; set; }
        public string? Comment { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string? FullName { get; set; }
    }
}
