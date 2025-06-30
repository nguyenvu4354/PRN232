namespace ShoppingWeb.DTOs.Cart
{
    public class ToOrderDTO
    {
        public int UserId { get; set; }
        public string ShippingAddress { get; set; } = null!;
        public int? ProvinceId { get; set; } = null!;
        public int? DistrictId { get; set; } = null!;
        public int? WardId { get; set; } = null!;
    }
}
