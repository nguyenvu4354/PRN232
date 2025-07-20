using ShoppingWeb.Models;

namespace ShoppingWeb.DTOs
{

    public class PaymentInfo
    {
        public int CartId { get; set; }
        public int Amount { get; set; }
        public int AmountPaid { get; set; }
        public int AmountRemaining { get; set; }
        public string Status { get; set; } = string.Empty;
    }
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

    public class CartDTO
    {
        public string UserId { get; set; }
        public int CartId { get; set; }
        public List<CartItemDTO> CartItems { get; set; } = new List<CartItemDTO>();
        public decimal TotalPrice { get; set; }

    }
    public class ProvinceDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
    }
    public class DistrictDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public int ProvinceId { get; set; }
    }
    public class WardDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public int DistrictId { get; set; }
    }
    public class AllAddressesDTO
    {
        public List<ProvinceDTO> Provinces { get; set; } = new List<ProvinceDTO>();
        public List<DistrictDTO> Districts { get; set; } = new List<DistrictDTO>();
        public List<WardDTO> Wards { get; set; } = new List<WardDTO>();
    }
}
