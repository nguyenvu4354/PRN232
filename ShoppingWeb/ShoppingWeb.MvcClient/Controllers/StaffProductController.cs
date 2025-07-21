using Microsoft.AspNetCore.Mvc;
using ShoppingWeb.MvcClient.Helper;

namespace ShoppingWeb.MvcClient.Controllers
{
    // Controller dành cho staff quản lý sản phẩm
    public class StaffProductController : Controller
    {
        // Danh sách sản phẩm
        public IActionResult StaffProductList()
        {
            var role = AuthHelper.GetRole(HttpContext);
            if (role != "STAFF")
                return RedirectToAction("Login", "Auth");
            return View(); // Trả về Views/StaffProduct/StaffProductList.cshtml
        }

        // Form thêm sản phẩm
        public IActionResult StaffProductCreate()
        {
            var role = AuthHelper.GetRole(HttpContext);
            if (role != "STAFF")
                return RedirectToAction("Login", "Auth");
            return View(); // Trả về Views/StaffProduct/StaffProductCreate.cshtml
        }

        // Form sửa sản phẩm
        public IActionResult StaffProductEdit(int id)
        {
            var role = AuthHelper.GetRole(HttpContext);
            if (role != "STAFF")
                return RedirectToAction("Login", "Auth");
            ViewBag.ProductId = id;
            return View(); // Trả về Views/StaffProduct/StaffProductEdit.cshtml
        }

        // Xác nhận xóa sản phẩm
        public IActionResult StaffProductDelete(int id)
        {
            var role = AuthHelper.GetRole(HttpContext);
            if (role != "STAFF")
                return RedirectToAction("Login", "Auth");
            ViewBag.ProductId = id;
            return View(); // Trả về Views/StaffProduct/StaffProductDelete.cshtml
        }
    }
} 