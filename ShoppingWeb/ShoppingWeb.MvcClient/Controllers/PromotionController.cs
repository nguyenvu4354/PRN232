using Microsoft.AspNetCore.Mvc;
using ShoppingWeb.MvcClient.DTOs.Promotion;
using ShoppingWeb.MvcClient.DTOs.Common;
using System.Text.Json;
using ShoppingWeb.MvcClient.DTOs.Product;
using System.Text;

namespace ShoppingWeb.MvcClient.Controllers
{
    public class PromotionController : Controller
    {
        private readonly HttpClient _httpClient;

        public PromotionController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("ShoppingApi");
        }

        public async Task<IActionResult> Index(int page = 1, int pageSize = 10)
        {
            var response = await _httpClient.GetAsync($"promotion/list?page={page}&pageSize={pageSize}");

            if (!response.IsSuccessStatusCode)
            {
                ViewBag.Error = "Failed to load promotions.";
                return View(new PagedResultDTO<PromotionResponseDTO>());
            }

            var json = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<ApiResponseDTO<PagedResultDTO<PromotionResponseDTO>>>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return View(result?.Data);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(PromotionRequestDTO dto)
        {
            var content = new StringContent(JsonSerializer.Serialize(dto), System.Text.Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("promotion/create", content);

            if (response.IsSuccessStatusCode)
            {
                TempData["Success"] = "Promotion created successfully.";
                return RedirectToAction("Index");
            }

            TempData["Error"] = "Failed to create promotion.";
            return View(dto);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var res = await _httpClient.GetAsync($"promotion/{id}");
            var json = await res.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<ApiResponseDTO<PromotionResponseDTO>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (result?.Data == null) return NotFound();

            var dto = new PromotionRequestDTO
            {
                Title = result.Data.Title,
                Description = result.Data.Description,
                DiscountAmount = result.Data.DiscountAmount,
                DiscountPercentage = result.Data.DiscountPercentage,
                StartDate = result.Data.StartDate,
                EndDate = result.Data.EndDate
            };

            ViewBag.PromotionId = id;
            return View(dto);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, PromotionRequestDTO dto)
        {
            var content = new StringContent(JsonSerializer.Serialize(dto), System.Text.Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync($"promotion/{id}", content);

            if (response.IsSuccessStatusCode)
            {
                TempData["Success"] = "Promotion updated.";
                return RedirectToAction("Index");
            }

            TempData["Error"] = "Update failed.";
            return View(dto);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var response = await _httpClient.DeleteAsync($"promotion/{id}");

            if (response.IsSuccessStatusCode)
            {
                TempData["Success"] = "Promotion deleted.";
            }
            else
            {
                TempData["Error"] = "Failed to delete promotion.";
            }

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Detail(int id)
        {
            var promotionResponse = await _httpClient.GetAsync($"promotion/{id}");
            if (!promotionResponse.IsSuccessStatusCode)
            {
                TempData["Error"] = "Failed to load promotion details.";
                return RedirectToAction("Index");
            }

            var promotionJson = await promotionResponse.Content.ReadAsStringAsync();
            var promotionResult = JsonSerializer.Deserialize<ApiResponseDTO<PromotionResponseDTO>>(promotionJson,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (promotionResult?.Data == null)
            {
                TempData["Error"] = "Promotion not found.";
                return RedirectToAction("Index");
            }
            var productResponse = await _httpClient.GetAsync($"promotion/{id}/products");

            if (!productResponse.IsSuccessStatusCode)
            {
                TempData["Error"] = "Failed to load products in promotion.";
                return RedirectToAction("Index");
            }

            var productJson = await productResponse.Content.ReadAsStringAsync();

            var products = JsonSerializer.Deserialize<List<ProductResponseDTO>>(productJson,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            var viewModel = new PromotionDetailViewModel
            {
                Promotion = promotionResult.Data,
                Products = products ?? new List<ProductResponseDTO>()
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> ToggleStatus(int id, bool isActive)
        {
            var response = await _httpClient.PutAsync($"promotion/{id}/status?isActive={isActive}", null);

            if (response.IsSuccessStatusCode)
            {
                TempData["Success"] = isActive ? "Promotion enabled." : "Promotion disabled.";
            }
            else
            {
                TempData["Error"] = "Failed to update promotion status.";
            }

            return RedirectToAction("Index");
        }


    }
}