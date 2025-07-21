using Net.payOS.Types;

namespace ShoppingWeb.Services.ThirdParty
{
    public record CreatePaymentRequest
    {
        public required int Id { get; set; }
        public required int Amount { get; set; }
        public required string Description { get; set; }
        public required List<ItemData> Items { get; set; }
    }
    public record CreatePaymentResponse
    {
        public required long OrderCode { get; set; }
        public required string CheckoutUrl { get; set; }
    }
    public interface IPayOS
    {
        public Task<CreatePaymentResponse> CreatePayment(CreatePaymentRequest request);
        public Task<PaymentLinkInformation> GetPayment(long orderCode);
    }
    public class PayOS : IPayOS
    {
        private readonly Net.payOS.PayOS _client;

        public PayOS(IConfiguration configuration)
        {
            _client = new Net.payOS.PayOS
            (
                configuration["PayOS:clientId"],
                configuration["PayOS:apiKey"],
                configuration["PayOS:checkSumKey"]
            );
        }

        public async Task<CreatePaymentResponse> CreatePayment(CreatePaymentRequest request)
        {
            var code = Random.Shared.Next(0x100000, 0xFFFFFF);
            PaymentData paymentData = new
            (
                code,
                request.Amount,
                request.Description,
                request.Items,
                cancelUrl: $@"https://localhost:5001/order/paymentresult?cartid={request.Id}",
                returnUrl: $@"https://localhost:5001/order/paymentresult?cartid={request.Id}"
            );
            CreatePaymentResult createPayment;
            try
            {
                createPayment = await _client.createPaymentLink(paymentData);
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to create payment link", ex);
            }
            return new CreatePaymentResponse
            {
                CheckoutUrl = createPayment.checkoutUrl,
                OrderCode = createPayment.orderCode
            };
        }

        public async Task<PaymentLinkInformation> GetPayment(long orderCode)
        {
            PaymentLinkInformation paymentLinkInformation;
            try
            {
                paymentLinkInformation = await _client.getPaymentLinkInformation(orderId: orderCode);
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to retrieve payment link information", ex);
            }
            return paymentLinkInformation;
        }
    }
}
