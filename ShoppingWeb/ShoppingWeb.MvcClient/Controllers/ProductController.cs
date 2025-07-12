using Microsoft.AspNetCore.Mvc;
using ShoppingWeb.MvcClient.DTOs.Common;
using ShoppingWeb.MvcClient.DTOs.Product;

namespace ShoppingWeb.MvcClient.Controllers
{
    public class ProductController : Controller
    {
        private readonly HttpClient _httpClient;
        public ProductController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("ShoppingApi");
        }
        public async Task<IActionResult> Index(string? search = null,
            string? brand = null,
            string? category = null,
            string? sortBy = null,
            int pageIndex = 1,
            int pageSize = 10)
        {
            var viewModel = new ProductListViewModel
            {
                Search = search,
                SortBy = sortBy,
                PageIndex = pageIndex,
                PageSize = pageSize,
            };

            var url = $"Product/products/advanced?search={Uri.EscapeDataString(search ?? string.Empty)}" +
                      $"&brand={Uri.EscapeDataString(brand ?? string.Empty)}" +
                      $"&category={Uri.EscapeDataString(category ?? string.Empty)}" +
                      $"&sortBy={Uri.EscapeDataString(sortBy ?? string.Empty)}" +
                      $"&pageIndex={pageIndex}&pageSize={pageSize}";
            var response = await _httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode)
            {
                viewModel.Success = false;
                var errorContent = response.Content.ReadAsStringAsync().Result;
                viewModel.Message = $"Error fetching API {errorContent}";
                viewModel.ErrorCode = response.StatusCode.ToString();
                return View(viewModel);
            }
            else
            {
                viewModel.Success = true;
                viewModel.Message = "Fetched successfully";
            }
            var json = response.Content.ReadAsStringAsync().Result;
            var result = System.Text.Json.JsonSerializer.Deserialize<List<ProductListItemResponseDTO>>(
                json,
                new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );
            viewModel.Products = result;

            return View(viewModel);
        }

        public async Task<IActionResult> Detail(int id)
        {
            var response = await _httpClient.GetAsync($"Product/products/{id}");
            var viewModel = new ProductDetailViewModel();
            if (!response.IsSuccessStatusCode)
            {
                viewModel.Success = false;
                var errorContent = await response.Content.ReadAsStringAsync();
                viewModel.Message = $"Error fetching API {errorContent}";
                viewModel.ErrorCode = response.StatusCode.ToString();
                return View(viewModel);
            }
            else
            {
                viewModel.Success = true;
                viewModel.Message = "Fetched successfully";
            }
            var json = await response.Content.ReadAsStringAsync();
            var result = System.Text.Json.JsonSerializer.Deserialize<ProductDetailItemDto>(
                json,
                new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );
            viewModel.Product = result;
            return View(viewModel);
        }
    }

    public class ProductListViewModel
    {
        public string? Search { get; set; }
        public List<BrandDTO> Brands { get; set; } = new List<BrandDTO>();
        public List<CategoryDTO> Categories { get; set; } = new List<CategoryDTO>();
        public string? SortBy { get; set; }
        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? Message { get; set; } = string.Empty;
        public string? ErrorCode { get; set; } = string.Empty;
        public bool Success { get; set; } = true;
        public List<ProductListItemResponseDTO> Products { get; set; } = new List<ProductListItemResponseDTO>();
    }

    public class ProductDetailViewModel
    {
        public ProductDetailItemDto Product { get; set; } = new ProductDetailItemDto();
        public string? Message { get; set; } = string.Empty;
        public string? ErrorCode { get; set; } = string.Empty;
        public bool Success { get; set; } = true;
    }
}
