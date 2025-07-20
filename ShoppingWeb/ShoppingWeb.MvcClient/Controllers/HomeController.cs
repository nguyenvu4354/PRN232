using Microsoft.AspNetCore.Mvc;
using ShoppingWeb.MvcClient.DTOs.Promotion;
using ShoppingWeb.MvcClient.DTOs.Common;
using System.Text.Json;
using ShoppingWeb.MvcClient.Response;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly HttpClient _httpClient;

    public HomeController(ILogger<HomeController> logger, IHttpClientFactory httpClientFactory)
    {
        _logger = logger;
        _httpClient = httpClientFactory.CreateClient("API");
    }

    public async Task<IActionResult> Index()
    {
        try
        {
            var response = await _httpClient.GetAsync("promotion/list?page=1&pageSize=100");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var apiResponse = JsonSerializer.Deserialize<ApiResponse<PagedResultDTO<PromotionResponseDTO>>>(
                    content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                var promotions = apiResponse?.Data?.Items?.Where(p => p.IsActive).ToList();
                ViewBag.Promotions = promotions;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load promotions");
        }

        return View();
    }


    public IActionResult Privacy()
    {
        return View();
    }
}
