using QUANLYDICHVUDULICH.Admin.Controllers;
using QUANLYDICHVUDULICH.ADMIN.Models;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace QUANLYDICHVUDULICH.ADMIN.Controllers
{
    public class DatTourController : BaseController
    {
        // 1. Hiển thị danh sách
        public ActionResult Index(string searchString, string status)
        {
            try
            {
                // Gọi API lấy danh sách
                var listBooking = GetFromApi<List<DatTourViewModel>>("api/admindattour");
                if (listBooking == null) listBooking = new List<DatTourViewModel>();

                // Lọc dữ liệu
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

        // 2. Duyệt đơn
        [HttpPost]
        public ActionResult Approve(int id)
        {
            var data = new { MaDatTour = id, TrangThai = "Đã xác nhận" };

            if (PostToApi("api/admindattour/status", data))
            {
                return Json(new { success = true, message = "Duyệt đơn hàng thành công!" });
            }
            return Json(new { success = false, message = "Lỗi khi gọi API Duyệt đơn." });
        }

        // 3. Hủy đơn
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

        // 4. Xóa đơn (SỬA ĐƯỜNG DẪN KHỚP API)
        [HttpPost]
        public ActionResult Delete(int id)
        {
            // API: api/admindattour/delete/{id}
            bool result = DeleteFromApi("api/admindattour/delete/" + id);

            if (result)
            {
                return Json(new { success = true, message = "Xóa thành công!" });
            }

            // Nếu API trả về BadRequest, hàm DeleteFromApi trả về false
            // Có thể nâng cấp BaseController để lấy message lỗi chi tiết, nhưng tạm thời báo chung chung
            return Json(new { success = false, message = "Không thể xóa đơn này (Lỗi server hoặc dữ liệu ràng buộc)." });
        }
    }
}