using Microsoft.AspNetCore.Mvc;

namespace ShoppingWeb.MvcClient.Controllers
{
    // Controller dành cho staff quản lý sản phẩm
    public class StaffProductController : Controller
    {
        public IActionResult ProductList()
        {
            return View(); // Trả về Views/StaffProduct/ProductList.cshtml
        }
    }
} 