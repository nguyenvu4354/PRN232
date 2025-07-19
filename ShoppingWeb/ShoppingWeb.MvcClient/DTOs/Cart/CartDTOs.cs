using ShoppingWeb.MvcClient.Controllers;

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
    public class CartDTO
    {
        public string UserId { get; set; }
        public int CartId { get; set; }
        public List<CartItem> CartItems { get; set; } = new List<CartItem>();
        public decimal TotalPrice { get; set; }

    }
}
