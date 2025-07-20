using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ShoppingWeb.MvcClient.DTOs.Addresses;
using ShoppingWeb.MvcClient.DTOs.Cart;
using ShoppingWeb.MvcClient.DTOs.User;
using ShoppingWeb.MvcClient.Helper;
using ShoppingWeb.MvcClient.Response;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Transactions;

namespace ShoppingWeb.MvcClient.Controllers
{
    public class OrderController : Controller
    {
        private readonly HttpClient _httpClient;
        public OrderController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("ShoppingApi");
        }

        public async Task<IActionResult> Checkout(int cartId)
        {
            var model = new CheckoutViewModel
            {
                CartId = cartId
            };
            var accessToken = AuthHelper.GetAccessToken(HttpContext);
            if (string.IsNullOrEmpty(accessToken))
            {
                TempData["Error"] = "You need to log in to proceed with checkout.";
                return RedirectToAction("Login", "Auth");
            }
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", accessToken);
            var response = await _httpClient.GetAsync("User/profile");

            if (!response.IsSuccessStatusCode)
            {
                TempData["Error"] = "Session expired. Please login again.";
                return RedirectToAction("Login", "Auth");
            }
            var json = await response.Content.ReadAsStringAsync();
            var wrapper = System.Text.Json.JsonSerializer.Deserialize<ApiResponse<UserProfileResponseDTO>>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (wrapper?.Data == null)
            {
                TempData["Error"] = "Cannot load user data.";
                return RedirectToAction("Login", "Auth");
            }
            model.UserProfile = wrapper.Data;

            var addressResponse = await _httpClient.GetAsync("Order/all-addresses");
            if (!addressResponse.IsSuccessStatusCode)
            {
                TempData["Error"] = "Cannot load address data.";
                return RedirectToAction("Index", "Cart");
            }
            var addresses = await addressResponse.Content.ReadFromJsonAsync<AllAddressDTO>(new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
            if (addresses == null)
            {
                TempData["Error"] = "No address data found.";
                return RedirectToAction("Index", "Cart");
            }
            model.AddressData = addresses;
            
            var cartResponse = await _httpClient.GetAsync($"Cart/items");
            if (!cartResponse.IsSuccessStatusCode)
            {
                TempData["Error"] = "Cannot retrieve cart items.";
                return RedirectToAction("Index", "Cart");
            }
            var cartData = await cartResponse.Content.ReadFromJsonAsync<CartDTO>(new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
            var cartItems = cartData.CartItems?.Select(item => new CartItem
            {
                ProductId = item.ProductId,
                ProductName = item.ProductName,
                ProductImage = item.ProductImage,
                Price = item.Price,
                Quantity = item.Quantity
            }).ToList();
            if (cartItems == null || !cartItems.Any())
            {
                TempData["Error"] = "Your cart is empty.";
                return RedirectToAction("Index", "Cart");
            }
            model.CartId = cartData.CartId;
            model.CartItems = cartItems;
            model.TotalQuantity = cartItems.Sum(item => item.Quantity);
            model.SubTotal = cartItems.Sum(item => item.Price * item.Quantity);
            return View(model);
        }

        public async Task<IActionResult> CreateOrder(int cartId, int provinceId, int districtId, int wardId, string detailAddress)
        {
            var accessToken = AuthHelper.GetAccessToken(HttpContext);
            if (string.IsNullOrEmpty(accessToken))
            {
                TempData["Error"] = "You need to log in to proceed.";
                return RedirectToAction("Login", "Auth");
            }
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", accessToken);
            if(provinceId == 0 || districtId == 0 || wardId == 0 || string.IsNullOrEmpty(detailAddress))
            {
                TempData["Error"] = "Please select a valid address.";
                return RedirectToAction("Checkout", new { cartId });
            }

            var orderRequest = new ToOrderDTO
            {
                ProvinceId = provinceId,
                DistrictId = districtId,
                WardId = wardId,
                ShippingAddress = detailAddress
            };
            var url = $"Order/create-order";
            var response = await _httpClient.PostAsJsonAsync(url, orderRequest);
            if (!response.IsSuccessStatusCode)
            {
                var errorResponse = await response.Content.ReadAsStringAsync();
                TempData["Error"] = $"Error creating order: {errorResponse}";
                return RedirectToAction("Checkout", new { cartId });
            }

            var paymentResponse = await _httpClient.PostAsJsonAsync($"Order/create-payment", cartId);
            if (!paymentResponse.IsSuccessStatusCode)
            {
                var errorResponse = await paymentResponse.Content.ReadAsStringAsync();
                TempData["Error"] = $"Error creating payment: {errorResponse}";
                return RedirectToAction("Checkout", new { cartId });
            }
            var paymentData = await paymentResponse.Content.ReadFromJsonAsync<CreatePaymentResponse>(new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
            if (paymentData == null)
            {
                TempData["Error"] = "Failed to create payment.";
                return RedirectToAction("Checkout", new { cartId });
            }
            return Redirect(paymentData.CheckoutUrl);
        }

        public async Task<IActionResult> PaymentResult(int cartId)
        {
            var model = new PaymentResultViewModel();
            var response = await _httpClient.GetAsync($"Order/payment-info/?cartid={Uri.EscapeDataString(cartId.ToString())}");
            if (!response.IsSuccessStatusCode)
            {
                model.IsSuccess = false;
                model.Message = "Failed to retrieve payment information.";
                return View(model);
            }
            var paymentResult = await response.Content.ReadFromJsonAsync<PaymentInfo>(new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
            if (paymentResult == null)
            {
                model.IsSuccess = false;
                model.Message = "Payment information is not available.";
                return View(model);
            }
            model.PaymentInfo = paymentResult;
            if (paymentResult.Status == "PAID")
            {
                model.IsSuccess = true;
                model.Message = "Payment completed successfully.";
            }
            else
            {
                model.IsSuccess = false;
                model.Message = "Payment failed or is pending.";
            }

            var orderResponse = await _httpClient.PostAsJsonAsync($"Order/place-order", cartId);
            if (!orderResponse.IsSuccessStatusCode)
            {
                model.IsSuccess = false;
                model.Message = "Failed to place order.";
                return View(model);
            }
            var orderData = await orderResponse.Content.ReadFromJsonAsync<CreateOrderResponse>(new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
            if (orderData == null)
            {
                model.IsSuccess = false;
                model.Message = "Order data is not available.";
                return View(model);
            }
            model.IsSuccess = true;
            model.Message = "Order paid and placed successfully.";
            orderData.ExpectedDeliveryTime = orderData.ExpectedDeliveryTime.Split('T')[0];
            model.OrderResponse = orderData;

            return View(model);
        }
    }

    public class PaymentResultViewModel
    {
        public string Message { get; set; }
        public PaymentInfo PaymentInfo { get; set; } = new PaymentInfo();
        public CreateOrderResponse OrderResponse { get; set; } = new CreateOrderResponse();
        public bool IsSuccess { get; set; }
    }

    public record CreateOrderResponse
    {
        [JsonProperty("order_code")]
        public string OrderCode { get; set; }

        [JsonProperty("expected_delivery_time")]
        public string ExpectedDeliveryTime { get; set; }

        [JsonProperty("total_fee")]
        public int TotalFee { get; set; }
    }

    public class PaymentInfo
    {
        public int CartId { get; set; }
        public int Amount { get; set; }
        public int AmountPaid { get; set; }
        public int AmountRemaining { get; set; }
        public string Status { get; set; } = string.Empty;
    }

    public class ToOrderDTO
    {
        public int UserId { get; set; }
        public string ShippingAddress { get; set; } = null!;
        public int? ProvinceId { get; set; } = null!;
        public int? DistrictId { get; set; } = null!;
        public int? WardId { get; set; } = null!;
    }

    public record CreatePaymentResponse
    {
        public required long OrderCode { get; set; }
        public required string CheckoutUrl { get; set; }
    }
    public class CheckoutViewModel
    {
        public UserProfileResponseDTO UserProfile { get; set; }
        public AllAddressDTO AddressData { get; set; }
        public List<CartItem> CartItems { get; set; }
        public int CartId { get; set; }
        public decimal SubTotal { get; set; }
        public int TotalQuantity { get; set; }
        public decimal ShippingFee { get; set; } = 0;
        public int SelectedProvince = 1;
        public int SelectedDistrict = 1;
        public int SelectedWard = 1;
    }

}