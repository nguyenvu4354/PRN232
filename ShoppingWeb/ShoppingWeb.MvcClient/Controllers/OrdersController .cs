using Microsoft.AspNetCore.Mvc;
using ShoppingWeb.MvcClient.DTOs.Common;
using ShoppingWeb.MvcClient.DTOs.Order;
using System.Text.Json;
using System.Text;

namespace ShoppingWeb.MvcClient.Controllers
{
    public class OrdersController : Controller
    {
        private readonly HttpClient _httpClient;

        public OrdersController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("ShoppingApi");
        }

        // ✅ GET: /Orders
        public async Task<IActionResult> Index(int page = 1, int pageSize = 10)
        {
            var response = await _httpClient.GetAsync($"orders/paged?page={page}&pageSize={pageSize}");

            if (!response.IsSuccessStatusCode)
            {
                ViewBag.Error = "Failed to load orders.";
                return View(new PagedResultDTO<OrderResponseDTO>());
            }

            var json = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<ApiResponseDTO<PagedResultDTO<OrderResponseDTO>>>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return View(result?.Data);
        }

        // ✅ GET: /Orders/Detail/5
        public async Task<IActionResult> Detail(int id)
        {
            var response = await _httpClient.GetAsync($"orders/{id}");

            if (!response.IsSuccessStatusCode)
            {
                TempData["Error"] = "Failed to load order details.";
                return RedirectToAction("Index");
            }

            var json = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<ApiResponseDTO<OrderResponseDTO>>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (result?.Data == null)
            {
                TempData["Error"] = "Order not found.";
                return RedirectToAction("Index");
            }

            return View(result.Data);
        }

        // ✅ GET: /Orders/UpdateStatus/5
        public async Task<IActionResult> UpdateStatus(int id)
        {
            var response = await _httpClient.GetAsync($"orders/{id}");

            if (!response.IsSuccessStatusCode)
            {
                TempData["Error"] = "Failed to load order.";
                return RedirectToAction("Index");
            }

            var json = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<ApiResponseDTO<OrderResponseDTO>>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (result?.Data == null)
            {
                TempData["Error"] = "Order not found.";
                return RedirectToAction("Index");
            }

            ViewBag.OrderId = id;
            return View(new UpdateOrderStatusDTO { Status = result.Data.Status });
        }

        // ✅ POST: /Orders/UpdateStatus/5
        [HttpPost]
        public async Task<IActionResult> UpdateStatus(int id, UpdateOrderStatusDTO dto)
        {
            var content = new StringContent(JsonSerializer.Serialize(dto), Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync($"orders/OrderDetail{id}/status", content);

            if (response.IsSuccessStatusCode)
            {
                TempData["Success"] = "Order status updated successfully.";
                return RedirectToAction("Detail", new { id });
            }

            TempData["Error"] = "Failed to update status.";
            ViewBag.OrderId = id;
            return View(dto);
        }
    }
}
