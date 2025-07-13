using ShoppingWeb.DTOs.Common;
using ShoppingWeb.DTOs.Order;

namespace ShoppingWeb.Services.Interface
{
    public interface IOrderService
    {
        Task<PagedResultDTO<OrderResponseDTO>> GetOrdersPagedAsync(int page, int pageSize);
        Task<OrderResponseDTO> GetOrderByIdAsync(int orderId);
        Task UpdateOrderStatusAsync(int orderId, string status);

    }
}