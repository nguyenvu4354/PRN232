using Microsoft.AspNetCore.Mvc;
using Microsoft.DotNet.Scaffolding.Shared.Messaging;
using ShoppingWeb.MvcClient.DTOs.Cart;

namespace ShoppingWeb.MvcClient.Controllers
{
    public class CartController : Controller
    {
        private readonly HttpClient _httpClient;
        public CartController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("ShoppingApi");
        }
        public async Task<IActionResult> Index()
        {
            var model = new ViewCartModel
            {

            };
            var accessToken = Request.Cookies["AccessToken"];
            if (string.IsNullOrEmpty(accessToken))
            {
                model.Success = false;
                model.ErrorMessage = "Please log in to view your cart.";
                return View("Index", model);
            }

            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
            var url = $"Cart/items";
            var response = await _httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode)
            {
                model.Success = false;
                model.ErrorMessage = "Failed to retrieve cart items.";
                model.ErrorCode = response.StatusCode.ToString();
                return View("Index", model);
            }
            var cart = await response.Content.ReadFromJsonAsync<CartDTO>();
            var cartItems = cart.CartItems?.Select(item => new CartItem
            {
                ProductId = item.ProductId,
                ProductName = item.ProductName,
                ProductImage = item.ProductImage,
                Price = item.Price,
                Quantity = item.Quantity
            }).ToList();
            if (cartItems == null || !cartItems.Any())
            {
                model.Success = false;
                model.ErrorMessage = "Your cart is empty.";
                return View("Index", model);
            }
            model.CartId = cart.CartId;
            model.CartItems = cartItems;
            model.TotalPrice = cartItems.Sum(item => item.Price * item.Quantity);
            return View("Index", model);
        }
        [HttpPost]
        public async Task<IActionResult> AddToCart(int productId, int quantity)
        {
            var accessToken = Request.Cookies["AccessToken"];
            if (string.IsNullOrEmpty(accessToken))
            {
                return Json(new
                {
                    Redirect = true,
                    Url = Url.Action("Login", "Auth"),
                    Message = "Please log in to add items to your cart."
                });
            }

            var request = new AddToCartRequest
            {
                ProductId = productId,
                Quantity = quantity
            };
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
            var response = await _httpClient.PostAsJsonAsync("Cart/add", request);
            if (!response.IsSuccessStatusCode)
            {
                var errorResponse = await response.Content.ReadAsStringAsync();
                return Json(new
                {
                    Redirect = false,
                    Message = $"Error adding item to cart: {errorResponse}"
                });
            }

            return Json(new
            {
                Redirect = false,
                Message = "Item added to cart successfully."
            });
        }

        [HttpPost]
        public async Task<IActionResult> UpdateCart(int productId, int quantity)
        {
            var accessToken = Request.Cookies["AccessToken"];
            if (string.IsNullOrEmpty(accessToken))
            {
                return Json(new
                {
                    Redirect = true,
                    Url = Url.Action("Login", "Auth"),
                    Message = "Please log in to update your cart."
                });
            }
            var request = new UpdateCartItemRequest
            {
                ProductId = productId,
                Quantity = quantity,
            };
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
            var response = await _httpClient.PutAsJsonAsync("Cart/update", request);
            if (!response.IsSuccessStatusCode)
            {
                var errorResponse = await response.Content.ReadAsStringAsync();
                return Json(new
                {
                    Redirect = false,
                    Message = $"Error updating cart: {errorResponse}"
                });
            }
            return Json(new
            {
                Redirect = false,
                Message = "Cart updated successfully."
            });
        }

        [HttpPost]
        public async Task<IActionResult> RemoveFromCart(int productId)
        {
            var accessToken = Request.Cookies["AccessToken"];
            if (string.IsNullOrEmpty(accessToken))
            {
                return Json(new
                {
                    Redirect = true,
                    Url = Url.Action("Login", "Auth"),
                    Message = "Please log in to remove items from your cart."
                });
            }
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
            var url = $"Cart/remove?productId={productId}";
            var response = await _httpClient.DeleteAsync(url);
            if (!response.IsSuccessStatusCode)
            {
                var errorResponse = await response.Content.ReadAsStringAsync();
                return Json(new
                {
                    Redirect = false,
                    Message = $"Error removing item from cart: {errorResponse}"
                });
            }
            return RedirectToAction("Index", "Cart");
        }
    }

    public class CartItem
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string ProductImage { get; set; } = null!;
        public decimal Price { get; set; }
        public int Quantity { get; set; }
    }


    public class ViewCartModel
    {
        public string UserId { get; set; }
        public int CartId { get; set; }
        public List<CartItem> CartItems { get; set; } = new List<CartItem>();
        public decimal TotalPrice { get; set; }
        public bool Success { get; set; } = true;
        public string ErrorMessage { get; set; } = string.Empty;
        public string ErrorCode { get; set; } = string.Empty;
    }
}
