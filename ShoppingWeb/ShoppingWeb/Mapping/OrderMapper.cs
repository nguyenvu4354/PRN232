using ShoppingWeb.DTOs.Order;
using ShoppingWeb.Models;

namespace ShoppingWeb.Mapping
{
    public static class OrderMapper
    {
        public static OrderResponseDTO ToResponseDTO(Cart cart)
        {
            return new OrderResponseDTO
            {
                OrderId = cart.CartId,
                UserId = cart.UserId,
                UserName = cart.User?.Username ?? "",
                TotalAmount = cart.TotalAmount,
                OrderDate = cart.OrderDate,
                ShippingAddress = cart.ShippingAddress ?? "",
                Status = cart.OrderDetails.Any() ? cart.OrderDetails.First().Status : "Pending",
                Details = cart.OrderDetails.Select(od => new OrderDetailResponseDTO
                {
                    ProductId = od.ProductId,
                    Quantity = od.Quantity,
                    UnitPrice = od.UnitPrice,
                    Status = od.Status
                }).ToList()
            };
        }
    }
}
