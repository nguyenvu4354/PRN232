using Microsoft.AspNetCore.Mvc;

namespace ShoppingWeb.MvcClient.Controllers
{
    public class ProductController : Controller
    {
        // Action cho trang danh sách sản phẩm (Product List)
        public IActionResult ProductList()
        {
            // Có thể kiểm tra quyền staff ở đây nếu cần
            return View(); // Trả về Views/Product/ProductList.cshtml
        }
    }
} 