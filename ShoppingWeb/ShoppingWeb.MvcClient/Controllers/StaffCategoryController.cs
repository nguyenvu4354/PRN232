using Microsoft.AspNetCore.Mvc;

namespace ShoppingWeb.MvcClient.Controllers
{
    // Controller dành cho staff quản lý danh mục
    public class StaffCategoryController : Controller
    {
        // Danh sách danh mục
        public IActionResult StaffCategoryList()
        {
            return View(); // Trả về Views/StaffCategory/StaffCategoryList.cshtml
        }

        // Form thêm danh mục
        public IActionResult StaffCategoryCreate()
        {
            return View(); // Trả về Views/StaffCategory/StaffCategoryCreate.cshtml
        }

        // Form sửa danh mục
        public IActionResult StaffCategoryEdit(int id)
        {
            ViewBag.CategoryId = id;
            return View(); // Trả về Views/StaffCategory/StaffCategoryEdit.cshtml
        }

        // Xác nhận xóa danh mục
        public IActionResult StaffCategoryDelete(int id)
        {
            ViewBag.CategoryId = id;
            return View(); // Trả về Views/StaffCategory/StaffCategoryDelete.cshtml
        }
    }
} 