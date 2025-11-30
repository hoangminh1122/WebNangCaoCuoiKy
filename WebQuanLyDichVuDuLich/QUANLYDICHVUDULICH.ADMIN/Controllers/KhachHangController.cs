using QUANLYDICHVUDULICH.Admin.Controllers;
using QUANLYDICHVUDULICH.ADMIN.Models;
using System.Collections.Generic;
using System.Web.Mvc;

namespace QUANLYDICHVUDULICH.ADMIN.Controllers
{
    public class KhachHangController : BaseController
    {
        // 1. Hiển thị danh sách
        public ActionResult Index(string searchString, string statusFilter)
        {
            try
            {
                // Sửa đường dẫn API
                var list = GetFromApi<List<KhachHangViewModel>>("api/adminkhachhang");
                if (list == null) list = new List<KhachHangViewModel>();

                // Lọc dữ liệu
                if (!string.IsNullOrEmpty(searchString))
                {
                    searchString = searchString.ToLower();
                    list = list.FindAll(s => s.HoTen.ToLower().Contains(searchString) ||
                                             s.Email.ToLower().Contains(searchString) ||
                                             s.SoDienThoai.Contains(searchString));
                }

                if (!string.IsNullOrEmpty(statusFilter))
                {
                    if (statusFilter == "active") list = list.FindAll(s => s.TrangThai == true);
                    if (statusFilter == "locked") list = list.FindAll(s => s.TrangThai == false);
                }

                return View(list);
            }
            catch
            {
                return View(new List<KhachHangViewModel>());
            }
        }

        // 2. Xử lý KHÓA/MỞ KHÓA (AJAX)
        [HttpPost]
        public ActionResult ToggleStatus(int id)
        {
            // Gửi ID dưới dạng JSON body: { "MaND": ... }
            var data = new { MaND = id };

            // Gọi API đổi trạng thái
            if (PostToApi("api/adminkhachhang/toggle-status", data))
            {
                return Json(new { success = true, message = "Cập nhật trạng thái thành công!" });
            }

            return Json(new { success = false, message = "Lỗi khi gọi API." });
        }
    }
}