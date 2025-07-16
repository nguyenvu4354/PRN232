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
            int? brand = null,
            int? category = null,
            string? sortBy = null,
            int pageIndex = 1)
        {
            var viewModel = new ProductListViewModel
            {
                Search = search,
                SortBy = sortBy,
                PageIndex = pageIndex,
                PageSize = 10,
                SelectedBrand = brand,
                SelectedCategory = category
            };

            var url = $"Product/products/advanced?search={Uri.EscapeDataString(search ?? string.Empty)}" +
                      $"&brand={Uri.EscapeDataString(brand.ToString() ?? string.Empty)}" +
                      $"&category={Uri.EscapeDataString(category.ToString()  ?? string.Empty)}" +
                      $"&sortBy={Uri.EscapeDataString(sortBy ?? string.Empty)}" +
                      $"&pageIndex={pageIndex}&pageSize={10}";
            var response = await _httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode)
            {
                viewModel.Success = false;
                var errorContent = response.Content.ReadAsStringAsync().Result;
                viewModel.Message = $"Error fetching API {errorContent}";
                viewModel.ErrorCode = response.StatusCode.ToString();
                return View(viewModel);
            }
            viewModel.Success = true;
            viewModel.Message = "Fetched successfully";
            var json = response.Content.ReadAsStringAsync().Result;
            var result = System.Text.Json.JsonSerializer.Deserialize<List<ProductListItemResponseDTO>>(
                json,
                new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );
            viewModel.Products = result;
            viewModel.TotalPages = (int)Math.Ceiling((double)result.Count / viewModel.PageSize);

            // Fetch brands
            var brandResponse = await _httpClient.GetAsync("BrandManagement");
            if(!brandResponse.IsSuccessStatusCode)
            {
                viewModel.Success = false;
                var brandErrorContent = await brandResponse.Content.ReadAsStringAsync();
                viewModel.Message = $"Error fetching brands API {brandErrorContent}";
                viewModel.ErrorCode = brandResponse.StatusCode.ToString();
                return View(viewModel);
            }
            var brandJson = await brandResponse.Content.ReadAsStringAsync();
            var brands = System.Text.Json.JsonSerializer.Deserialize<List<BrandDTO>>(
                brandJson,
                new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );
            viewModel.Brands = brands ?? new List<BrandDTO>();
            // Fetch categories
            var categoryResponse = await _httpClient.GetAsync("CategoriesManagement");
            if (!categoryResponse.IsSuccessStatusCode)
            {
                viewModel.Success = false;
                var categoryErrorContent = await categoryResponse.Content.ReadAsStringAsync();
                viewModel.Message = $"Error fetching categories API {categoryErrorContent}";
                viewModel.ErrorCode = categoryResponse.StatusCode.ToString();
                return View(viewModel);
            }
            var categoryJson = await categoryResponse.Content.ReadAsStringAsync();
            var categories = System.Text.Json.JsonSerializer.Deserialize<List<CategoryDTO>>(
                categoryJson,
                new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );
            viewModel.Categories = categories ?? new List<CategoryDTO>();

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
        public int TotalPages { get; set; } = 1;
        public string? Message { get; set; } = string.Empty;
        public string? ErrorCode { get; set; } = string.Empty;
        public bool Success { get; set; } = true;
        public int? SelectedBrand { get; set; } = null;
        public int? SelectedCategory { get; set; } = null;
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
