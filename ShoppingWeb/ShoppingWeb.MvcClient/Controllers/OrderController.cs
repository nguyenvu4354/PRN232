using Microsoft.AspNetCore.Mvc;
using ShoppingWeb.MvcClient.DTOs.Addresses;
using ShoppingWeb.MvcClient.DTOs.Cart;
using ShoppingWeb.MvcClient.DTOs.User;
using ShoppingWeb.MvcClient.Helper;
using ShoppingWeb.MvcClient.Response;
using System.Net.Http.Headers;
using System.Text.Json;

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
                return RedirectToAction("Login", "User");
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
            var wrapper = JsonSerializer.Deserialize<ApiResponse<UserProfileResponseDTO>>(json, new JsonSerializerOptions
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