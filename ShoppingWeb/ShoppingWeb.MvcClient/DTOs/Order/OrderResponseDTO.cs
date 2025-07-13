namespace ShoppingWeb.MvcClient.DTOs.Order
{
    public class OrderResponseDTO
    {
        public int OrderId { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; } = null!;
        public decimal TotalAmount { get; set; }
        public DateTime? OrderDate { get; set; }
        public string ShippingAddress { get; set; } = null!;
        public string Status { get; set; } = null!;
        public List<OrderDetailResponseDTO> Details { get; set; } = new();
    }
}
