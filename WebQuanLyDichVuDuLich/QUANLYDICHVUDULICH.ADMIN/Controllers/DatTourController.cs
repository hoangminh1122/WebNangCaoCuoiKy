using QUANLYDICHVUDULICH.Admin.Controllers;
using QUANLYDICHVUDULICH.ADMIN.Models;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace QUANLYDICHVUDULICH.ADMIN.Controllers
{
    public class DatTourController : BaseController
    {
        // 1. Hiển thị danh sách đơn hàng
        public ActionResult Index(string searchString, string status)
        {
            try
            {
                // Gọi API lấy danh sách (Lưu ý: kiểm tra lại đường dẫn API của bạn là 'api/dattour' hay 'api/admindattour')
                var listBooking = GetFromApi<List<DatTourViewModel>>("api/admindattour");
                if (listBooking == null) listBooking = new List<DatTourViewModel>();

                // Xử lý tìm kiếm & Lọc tại Client
                if (!string.IsNullOrEmpty(searchString))
                {
                    listBooking = listBooking.FindAll(s => s.TenKhachHang.ToLower().Contains(searchString.ToLower())
                                                        || s.MaDatTour.ToString().Contains(searchString));
                }

                if (!string.IsNullOrEmpty(status) && status != "Tất cả trạng thái")
                {
                    listBooking = listBooking.FindAll(s => s.TrangThai == status);
                }

                return View(listBooking);
            }
            catch
            {
                return View(new List<DatTourViewModel>());
            }
        }

        // 2. Chức năng DUYỆT ĐƠN (POST AJAX)
        [HttpPost]
        public ActionResult Approve(int id)
        {
            // Tạo đối tượng dữ liệu gửi lên API
            var data = new { MaDatTour = id, TrangThai = "Đã xác nhận" };

            // Gọi API cập nhật (Giả sử bạn dùng API Update chung hoặc API riêng)
            // Lưu ý: Đường dẫn API phải khớp với bên Project API
            if (PostToApi("api/admindattour/status", data))
            {
                return Json(new { success = true, message = "Duyệt đơn hàng thành công!" });
            }

            return Json(new { success = false, message = "Lỗi khi gọi API Duyệt đơn." });
        }

        // 3. Chức năng HỦY ĐƠN (POST AJAX)
        [HttpPost]
        public ActionResult Cancel(int id)
        {
            var data = new { MaDatTour = id, TrangThai = "Đã hủy" };

            if (PostToApi("api/admindattour/status", data))
            {
                return Json(new { success = true, message = "Đã hủy đơn hàng!" });
            }

            return Json(new { success = false, message = "Lỗi khi gọi API Hủy đơn." });
        }

        // 4. Chức năng XÓA (POST AJAX)
        [HttpPost]
        public ActionResult Delete(int id)
        {
            // Gọi API xóa
            if (DeleteFromApi("api/admindattour/" + id))
            {
                return Json(new { success = true, message = "Xóa thành công!" });
            }

            return Json(new { success = false, message = "Không thể xóa đơn này (Do ràng buộc dữ liệu hoặc lỗi API)." });
        }
    }
}