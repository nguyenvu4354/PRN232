using Microsoft.EntityFrameworkCore;
using ShoppingWeb.Data;
using ShoppingWeb.DTOs.Common;
using ShoppingWeb.DTOs.Order;
using ShoppingWeb.Exceptions;
using ShoppingWeb.Services.Interface;

namespace ShoppingWeb.Services
{
    public class OrderService : IOrderService
    {
        private readonly ShoppingWebContext _context;
        private readonly ILogger<OrderService> _logger;

        public OrderService(ShoppingWebContext context, ILogger<OrderService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<PagedResultDTO<OrderResponseDTO>> GetOrdersPagedAsync(int page, int pageSize)
        {
            var query = _context.Carts
                .Include(c => c.OrderDetails)
                .Include(c => c.User)
                .Where(c => c.IsCart == false);

            var totalItems = await query.CountAsync();
            var orders = await query
                .OrderByDescending(c => c.OrderDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var result = orders.Select(cart => new OrderResponseDTO
            {
                OrderId = cart.CartId,
                UserId = cart.UserId,
                UserName = cart.User.Username,
                TotalAmount = cart.TotalAmount,
                OrderDate = cart.OrderDate,
                ShippingAddress = cart.ShippingAddress,
                Status = cart.OrderDetails.Any() ? cart.OrderDetails.First().Status : "Pending",
                Details = cart.OrderDetails.Select(od => new OrderDetailResponseDTO
                {
                    ProductId = od.ProductId,
                    Quantity = od.Quantity,
                    UnitPrice = od.UnitPrice,
                    Status = od.Status
                }).ToList()
            });

            return new PagedResultDTO<OrderResponseDTO>
            {
                Items = result,
                TotalItems = totalItems,
                Page = page,
                PageSize = pageSize
            };
        }

        public async Task<OrderResponseDTO> GetOrderByIdAsync(int orderId)
        {
            var cart = await _context.Carts
                .Include(c => c.User)
                .Include(c => c.OrderDetails)
                .FirstOrDefaultAsync(c => c.CartId == orderId && c.IsCart == false);

            if (cart == null)
            {
                _logger.LogWarning("Order with ID {OrderId} not found.", orderId);
                throw new OrderNotFoundException("Order not found");
            }

            return new OrderResponseDTO
            {
                OrderId = cart.CartId,
                UserId = cart.UserId,
                UserName = cart.User.Username,
                TotalAmount = cart.TotalAmount,
                OrderDate = cart.OrderDate,
                ShippingAddress = cart.ShippingAddress,
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
        public async Task UpdateOrderStatusAsync(int orderId, string status)
        {
            var orderDetails = await _context.OrderDetails
                .Where(od => od.CartId == orderId)
                .ToListAsync();

            if (!orderDetails.Any())
            {
                _logger.LogWarning("No order details found for order ID {OrderId}", orderId);
                throw new OrderNotFoundException($"Order with ID {orderId} not found.");
            }

            foreach (var detail in orderDetails)
            {
                detail.Status = status;
            }

            _context.OrderDetails.UpdateRange(orderDetails);
            await _context.SaveChangesAsync();
        }

    }
}