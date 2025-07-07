using Net.payOS.Types;
using ShoppingWeb.DTOs;
using ShoppingWeb.Models;
using ShoppingWeb.Services.ThirdParty;

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
        public Task<CreatePaymentResponse> CreatePayment(CreatePaymentRequest request);
        public Task<PaymentLinkInformation> GetPaymentInfo(int cartId);
        public Task<CreateOrderResponse> CreateOrder(int request);
        public Task<OrderStatusResponse> GetOrderStatus(int cartid);
        public Task<int> GetShippingFee(string wardCode, int districtId, int weight);
        public Task<List<Province>> GetProvinces();
        public Task<List<District>> GetDistricts(int provinceId);
        public Task<List<Ward>> GetWards(int districtId);
    }
}
