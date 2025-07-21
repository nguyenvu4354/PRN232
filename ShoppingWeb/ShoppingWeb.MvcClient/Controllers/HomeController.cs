using Microsoft.AspNetCore.Mvc;
using ShoppingWeb.MvcClient.DTOs.Blog;
using ShoppingWeb.MvcClient.DTOs.Product;
using System.Threading.Tasks;

namespace ShoppingWeb.MvcClient.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly HttpClient _httpClient;
        public HomeController(ILogger<HomeController> logger, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _httpClient = httpClientFactory.CreateClient("ShoppingWebApi");
        }

        public async Task<IActionResult> Index()
        {
            var model = new HomeResponseDto();
            var response = await _httpClient.GetAsync("api/home/home");
            if(response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                model = System.Text.Json.JsonSerializer.Deserialize<HomeResponseDto>(
                    json, 
                    new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                );
            }
            else
            {
                _logger.LogError($"Failed to fetch home data: {response.ReasonPhrase}");
                ViewBag.ErrorMessage = "Failed to load home data.";
            }

            return View(model);
        }

        public IActionResult Privacy()
        {
            return View();
        }
    }

    public class HomeResponseDto
    {
        public IEnumerable<ProductListItemResponseDTO> NewestProducts { get; set; }
        public IEnumerable<BlogPostDTO> LatestBlogs { get; set; }
    }
}


