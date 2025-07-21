using Microsoft.AspNetCore.Mvc;
using ShoppingWeb.DTOs;
using ShoppingWeb.Services.Interface;

namespace ShoppingWeb.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HomeController : Controller
    {
        private readonly IProductService _productService;
        private readonly IBlogService _blogService;
        
        public HomeController(IProductService productService, IBlogService blogService)
        {
            _productService = productService;
            _blogService = blogService;
        }

        [HttpGet("home")]
        public async Task<IActionResult> GetHomeData()
        {
            var newestProducts = await _productService.GetNewestProduct();
            var latestBlogs = await _blogService.GetNewestBlog();
            var response = new HomeResponseDto
            {
                NewestProducts = newestProducts,
                LatestBlogs = latestBlogs
            };
            return Ok(response);
        }

    }

    public class HomeResponseDto
    {
        public IEnumerable<ProductListItemResponseDto> NewestProducts { get; set; }
        public IEnumerable<BlogPostDTO> LatestBlogs { get; set; }
    }
}
