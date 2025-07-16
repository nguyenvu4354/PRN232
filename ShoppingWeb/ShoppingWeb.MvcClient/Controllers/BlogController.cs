using Microsoft.AspNetCore.Mvc;
using ShoppingWeb.MvcClient.DTOs.Blog;
using System.Net.Http;

namespace ShoppingWeb.MvcClient.Controllers
{
    public class BlogController : Controller
    {
        private readonly HttpClient _httpClient;
        public BlogController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("ShoppingApi");
        }
        public IActionResult Index(string? search, int pageIndex = 1)
        {
            var viewModel = new BlogIndexViewModel
            {

            };

            var url = $"Blog/advanced?search={Uri.EscapeDataString(search ?? string.Empty)}" +
                      $"&pageIndex={pageIndex}&pageSize=10";
            var response = _httpClient.GetAsync(url).Result;
            if (!response.IsSuccessStatusCode)
            {
                viewModel.Success = false;
                var errorContent = response.Content.ReadAsStringAsync().Result;
                viewModel.Message = $"Error fetching API {errorContent}";
                viewModel.ErrorCode = response.StatusCode.ToString();
                return View(viewModel);
            }
            viewModel.Success = true;

            var json = response.Content.ReadAsStringAsync().Result;
            var result = System.Text.Json.JsonSerializer.Deserialize<List<BlogPostDTO>>(
                json,
                new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );
            viewModel.BlogPosts = result ?? new List<BlogPostDTO>();
            viewModel.PageIndex = pageIndex;
            viewModel.PageSize = 10;
            viewModel.TotalPages = (int)Math.Ceiling((double)viewModel.BlogPosts.Count / viewModel.PageSize);
            viewModel.Message = "Fetched successfully";
            viewModel.Search = search;

            return View(viewModel);
        }

        public async Task<IActionResult> Detail(int id)
        {
            var url = $"Blog/{id}";
            var blogDetailViewModel = new BlogDetailViewModel();
            var response = await _httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode)
            {
                blogDetailViewModel.Success = false;
                blogDetailViewModel.Message = $"Error fetching blog detail: {response.ReasonPhrase}";
                blogDetailViewModel.ErrorCode = response.StatusCode.ToString();
                return View(blogDetailViewModel);
            }
            var json = await response.Content.ReadAsStringAsync();
            var blogDetail = System.Text.Json.JsonSerializer.Deserialize<BlogDetailDTO>(
                json,
                new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );
            if (blogDetail == null)
            {
                blogDetailViewModel.Success = false;
                blogDetailViewModel.Message = "Blog detail not found.";
                blogDetailViewModel.ErrorCode = "404";
                return View(blogDetailViewModel);
            }
            blogDetailViewModel.Success = true;
            blogDetailViewModel.BlogDetail = blogDetail;
            blogDetailViewModel.Message = "Blog detail fetched successfully";

            return View(blogDetailViewModel);
        }
    }

    public class BlogIndexViewModel
    {
        public string? Search { get; set; }
        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public int TotalPages { get; set; } = 1;
        public string? Message { get; set; } = string.Empty;
        public string? ErrorCode { get; set; } = string.Empty;
        public bool Success { get; set; } = true;
        public List<BlogPostDTO> BlogPosts { get; set; } = new List<BlogPostDTO>();
    }

    public class BlogDetailViewModel
    {
        public BlogDetailDTO BlogDetail { get; set; } = new BlogDetailDTO();
        public string? Message { get; set; } = string.Empty;
        public string? ErrorCode { get; set; } = string.Empty;
        public bool Success { get; set; } = true;
    }
}
