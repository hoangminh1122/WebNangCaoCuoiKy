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
                // Sửa đường dẫn API: api/adminkhachhang
                var list = GetFromApi<List<KhachHangViewModel>>("api/adminkhachhang");
                if (list == null) list = new List<KhachHangViewModel>();

                // Lọc dữ liệu theo từ khóa
                if (!string.IsNullOrEmpty(searchString))
                {
                    searchString = searchString.ToLower();
                    list = list.FindAll(s => s.HoTen.ToLower().Contains(searchString) ||
                                             s.Email.ToLower().Contains(searchString) ||
                                             s.SoDienThoai.Contains(searchString));
                }

                // Lọc theo trạng thái
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

        // 3. Xử lý XÓA (AJAX) - Mới thêm vào
        [HttpPost]
        public ActionResult Delete(int id)
        {
            // Gọi API xóa: api/adminkhachhang/delete/{id}
            if (DeleteFromApi("api/adminkhachhang/delete/" + id))
            {
                return Json(new { success = true, message = "Xóa khách hàng thành công!" });
            }

            // Nếu API trả về BadRequest (do ràng buộc đơn hàng), hàm DeleteFromApi trả về false
            return Json(new { success = false, message = "Không thể xóa (Khách hàng này có thể đã đặt tour)." });
        }

        [HttpGet]
        public ActionResult GetJsonKhachHang(int id)
        {
            try
            {
                var kh = GetFromApi<KhachHangViewModel>("api/adminkhachhang/" + id);
                return Json(kh, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }
    }
}