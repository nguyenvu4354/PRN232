using Microsoft.AspNetCore.Mvc;
using ShoppingWeb.MvcClient.Helper;

namespace ShoppingWeb.MvcClient.Controllers
{
    // Controller dành cho staff quản lý danh mục
    public class StaffCategoryController : Controller
    {
        // Danh sách danh mục
        public IActionResult StaffCategoryList()
        {
            var role = AuthHelper.GetRole(HttpContext);
            if (role != "STAFF")
                return RedirectToAction("Login", "Auth");
            return View(); // Trả về Views/StaffCategory/StaffCategoryList.cshtml
        }

        // Form thêm danh mục
        public IActionResult StaffCategoryCreate()
        {
            var role = AuthHelper.GetRole(HttpContext);
            if (role != "STAFF")
                return RedirectToAction("Login", "Auth");
            return View(); // Trả về Views/StaffCategory/StaffCategoryCreate.cshtml
        }

        // Form sửa danh mục
        public IActionResult StaffCategoryEdit(int id)
        {
            var role = AuthHelper.GetRole(HttpContext);
            if (role != "STAFF")
                return RedirectToAction("Login", "Auth");
            ViewBag.CategoryId = id;
            return View(); // Trả về Views/StaffCategory/StaffCategoryEdit.cshtml
        }

        // Xác nhận xóa danh mục
        public IActionResult StaffCategoryDelete(int id)
        {
            var role = AuthHelper.GetRole(HttpContext);
            if (role != "STAFF")
                return RedirectToAction("Login", "Auth");
            ViewBag.CategoryId = id;
            return View(); // Trả về Views/StaffCategory/StaffCategoryDelete.cshtml
        }
    }
} 