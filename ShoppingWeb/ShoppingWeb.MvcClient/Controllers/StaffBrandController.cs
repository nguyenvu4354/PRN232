using Microsoft.AspNetCore.Mvc;

namespace ShoppingWeb.MvcClient.Controllers
{
    // Controller dành cho staff quản lý thương hiệu
    public class StaffBrandController : Controller
    {
        // Danh sách thương hiệu
        public IActionResult StaffBrandList()
        {
            return View(); // Trả về Views/StaffBrand/StaffBrandList.cshtml
        }

        // Form thêm thương hiệu
        public IActionResult StaffBrandCreate()
        {
            return View(); // Trả về Views/StaffBrand/StaffBrandCreate.cshtml
        }

        // Form sửa thương hiệu
        public IActionResult StaffBrandEdit(int id)
        {
            ViewBag.BrandId = id;
            return View(); // Trả về Views/StaffBrand/StaffBrandEdit.cshtml
        }

        // Xác nhận xóa thương hiệu
        public IActionResult StaffBrandDelete(int id)
        {
            ViewBag.BrandId = id;
            return View(); // Trả về Views/StaffBrand/StaffBrandDelete.cshtml
        }
    }
} 