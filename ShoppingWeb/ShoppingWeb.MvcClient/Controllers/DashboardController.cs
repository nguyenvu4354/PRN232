using Microsoft.AspNetCore.Mvc;
using ShoppingWeb.MvcClient.DTOs.Statistics;
using ShoppingWeb.MvcClient.DTOs.Common;
using System.Text.Json;

namespace ShoppingWeb.MvcClient.Controllers
{
    public class DashboardController : Controller
    {
        private readonly HttpClient _httpClient;

        public DashboardController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("ShoppingApi");
        }

        public async Task<IActionResult> Index()
        {
            var summaryTask = _httpClient.GetAsync("statistics/summary");
            var weeklyTask = _httpClient.GetAsync("statistics/weekly-product-sales");

            await Task.WhenAll(summaryTask, weeklyTask);

            var summaryResult = await summaryTask.Result.Content.ReadAsStringAsync();
            var weeklyResult = await weeklyTask.Result.Content.ReadAsStringAsync();

            var summary = JsonSerializer.Deserialize<ApiResponseDTO<StatisticsResponseDTO>>(summaryResult, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            var weekly = JsonSerializer.Deserialize<ApiResponseDTO<List<WeeklyProductSalesDTO>>>(weeklyResult, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            ViewBag.Summary = summary?.Data;
            ViewBag.WeeklySales = weekly?.Data;

            return View();
        }
    }
}
