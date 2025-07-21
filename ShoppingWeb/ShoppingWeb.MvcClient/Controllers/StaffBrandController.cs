using Microsoft.AspNetCore.Mvc;
using ShoppingWeb.MvcClient.Helper;

namespace ShoppingWeb.MvcClient.Controllers
{
    // Controller dành cho staff quản lý thương hiệu
    public class StaffBrandController : Controller
    {
        // Danh sách thương hiệu
        public IActionResult StaffBrandList()
        {
            var role = AuthHelper.GetRole(HttpContext);
            if (role != "STAFF")
                return RedirectToAction("Login", "Auth");
            return View(); // Trả về Views/StaffBrand/StaffBrandList.cshtml
        }

        // Form thêm thương hiệu
        public IActionResult StaffBrandCreate()
        {
            var role = AuthHelper.GetRole(HttpContext);
            if (role != "STAFF")
                return RedirectToAction("Login", "Auth");
            return View(); // Trả về Views/StaffBrand/StaffBrandCreate.cshtml
        }

        // Form sửa thương hiệu
        public IActionResult StaffBrandEdit(int id)
        {
            var role = AuthHelper.GetRole(HttpContext);
            if (role != "STAFF")
                return RedirectToAction("Login", "Auth");
            ViewBag.BrandId = id;
            return View(); // Trả về Views/StaffBrand/StaffBrandEdit.cshtml
        }

        // Xác nhận xóa thương hiệu
        public IActionResult StaffBrandDelete(int id)
        {
            var role = AuthHelper.GetRole(HttpContext);
            if (role != "STAFF")
                return RedirectToAction("Login", "Auth");
            ViewBag.BrandId = id;
            return View(); // Trả về Views/StaffBrand/StaffBrandDelete.cshtml
        }
    }
} 