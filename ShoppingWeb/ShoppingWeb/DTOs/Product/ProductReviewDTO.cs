﻿namespace ShoppingWeb.DTOs
{
    public class ProductReviewWithUserDTO
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
