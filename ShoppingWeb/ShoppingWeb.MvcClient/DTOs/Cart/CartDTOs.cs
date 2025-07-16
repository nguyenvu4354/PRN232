namespace ShoppingWeb.MvcClient.DTOs.Cart
{
    public class AddToCartRequest
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public int UserId { get; set; }
    }
    public class UpdateCartItemRequest
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public int UserId { get; set; }
    }
}
