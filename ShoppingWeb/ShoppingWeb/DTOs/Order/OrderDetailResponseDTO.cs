namespace ShoppingWeb.DTOs.Order
{
    public class OrderDetailResponseDTO
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public string Status { get; set; } = null!;
    }
}
