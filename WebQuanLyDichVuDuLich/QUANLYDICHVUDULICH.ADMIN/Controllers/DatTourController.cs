using QUANLYDICHVUDULICH.Admin.Controllers;
using QUANLYDICHVUDULICH.ADMIN.Models;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace QUANLYDICHVUDULICH.ADMIN.Controllers
{
    public class DatTourController : BaseController
    {
        // 1. Hiển thị danh sách & Tìm kiếm
        public ActionResult Index(string searchString, string status)
        {
            try
            {
                // Gọi API lấy danh sách (Route: api/admindattour)
                var listBooking = GetFromApi<List<DatTourViewModel>>("api/admindattour");
                if (listBooking == null) listBooking = new List<DatTourViewModel>();

                // Xử lý tìm kiếm (Lọc tại Client)
                if (!string.IsNullOrEmpty(searchString))
                {
                    searchString = searchString.ToLower();
                    listBooking = listBooking.FindAll(s => s.TenKhachHang.ToLower().Contains(searchString)
                                                        || s.MaDatTour.ToString().Contains(searchString)
                                                        || s.TenTour.ToLower().Contains(searchString));
                }

                // Xử lý lọc theo trạng thái
                if (!string.IsNullOrEmpty(status) && status != "Tất cả trạng thái")
                {
                    listBooking = listBooking.FindAll(s => s.TrangThai == status);
                }

                return View(listBooking);
            }
            catch
            {
                // Trả về list rỗng nếu lỗi kết nối API
                return View(new List<DatTourViewModel>());
            }
        }

        // 2. Chức năng DUYỆT ĐƠN (POST AJAX)
        [HttpPost]
        public ActionResult Approve(int id)
        {
            // Tạo dữ liệu cập nhật
            var data = new { MaDatTour = id, TrangThai = "Đã xác nhận" };

            // Gọi API Update Status
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
            var data = new { MaDatTour = id, TrangThai = "Đã hủy" }; // Hoặc "Hủy" tùy vào logic DB của bạn

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
            // Gọi API Xóa: api/admindattour/delete/{id}
            // (Hàm này bên API đã được nâng cấp để xóa cả bảng ThanhToan liên quan)
            if (DeleteFromApi("api/admindattour/delete/" + id))
            {
                return Json(new { success = true, message = "Xóa thành công!" });
            }

            return Json(new { success = false, message = "Không thể xóa đơn này (Lỗi server hoặc dữ liệu ràng buộc)." });
        }
    }
}