namespace ShoppingWeb.DTOs
{
    public class ToOrderDTO
    {
        public int UserId { get; set; }
        public string ShippingAddress { get; set; } = null!;
        public int? ProvinceId { get; set; } = null!;
        public int? DistrictId { get; set; } = null!;
        public int? WardId { get; set; } = null!;
    }

    public class CartItemDTO
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = null!;
        public string ProductImage { get; set; } = null!;
        public decimal Price { get; set; }
        public int Quantity { get; set; }
    }
}
