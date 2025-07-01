using ShoppingWeb.DTOs;
using ShoppingWeb.Models;

namespace ShoppingWeb.Services.Interface
{
    public interface ICartService
    {
        public Task AddToCartAsync(int productId, int quantity, int userId);
        public Task RemoveFromCartAsync(int productId, int userId);
        public Task ClearCartAsync(int userId);
        public Task<IEnumerable<OrderDetail>> GetCartItemsAsync(int userId);
        public Task<decimal> GetTotalPriceAsync(int userId);
        public Task<int> GetCartItemCountAsync(int serId);
        public Task<OrderDetail> UpdateCartItemAsync(int productId, int quantity, int userId);
        public Task ToOrderAsync(ToOrderDTO toOderDTO);
    }
}
