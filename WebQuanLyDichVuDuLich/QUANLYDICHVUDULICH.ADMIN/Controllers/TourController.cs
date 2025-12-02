using QUANLYDICHVUDULICH.Admin.Controllers;
using QUANLYDICHVUDULICH.ADMIN.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Web.Mvc;

namespace QUANLYDICHVUDULICH.ADMIN.Controllers
{
    public class TourController : BaseController
    {
        // 1. LẤY DANH SÁCH TOUR (Kèm chức năng tìm kiếm)
        public ActionResult Index(string searchString)
        {
            try
            {
                // Gọi API lấy toàn bộ danh sách
                var tours = GetFromApi<List<TourViewModel>>("api/admintour");
                if (tours == null) tours = new List<TourViewModel>();

                // XỬ LÝ TÌM KIẾM
                if (!string.IsNullOrEmpty(searchString))
                {
                    searchString = searchString.Trim().ToLower();
                    // Lọc theo Tên Tour hoặc Mã Tour
                    tours = tours.FindAll(t =>
                        (t.TenTour != null && t.TenTour.ToLower().Contains(searchString)) ||
                        t.MaTour.ToString().Contains(searchString)
                    );
                }

                return View(tours);
            }
            catch
            {
                // Trả về list rỗng nếu lỗi kết nối để không chết trang web
                return View(new List<TourViewModel>());
            }
        }

        // 2. LẤY CHI TIẾT 1 TOUR (Trả về JSON để điền vào Form Sửa)
        // Hàm này thay thế cho GetModal cũ
        [HttpGet]
        public ActionResult GetJsonTour(int id)
        {
            try
            {
                var tour = GetFromApi<TourViewModel>("api/admintour/" + id);
                if (tour != null)
                {
                    // Trả về JSON cho Javascript đọc
                    return Json(tour, JsonRequestBehavior.AllowGet);
                }
                return Json(null, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }

        // 3. LƯU DỮ LIỆU (Thêm mới hoặc Cập nhật)
        [HttpPost]
        // ValidateInput(false) để cho phép lưu nội dung HTML (nếu có dùng CKEditor cho mô tả)
        [ValidateInput(false)]
        public ActionResult Save(TourViewModel tour)
        {
            try
            {
                // --- BƯỚC 1: XỬ LÝ UPLOAD ẢNH ---
                var file = Request.Files["ImageFile"];
                if (file != null && file.ContentLength > 0)
                {
                    // Tạo tên file ngẫu nhiên để tránh trùng
                    string fileName = "tour_" + Guid.NewGuid() + Path.GetExtension(file.FileName);

                    // Đường dẫn lưu file trên server Admin
                    string uploadPath = Server.MapPath("~/Content/Images/");

                    // Tạo thư mục nếu chưa có
                    if (!Directory.Exists(uploadPath))
                    {
                        Directory.CreateDirectory(uploadPath);
                    }

                    // Lưu file vật lý
                    file.SaveAs(Path.Combine(uploadPath, fileName));

                    // Cập nhật đường dẫn ảnh vào Model để lưu xuống DB
                    tour.HinhAnhDaiDien = "/Content/Images/" + fileName;
                }
                // Nếu không chọn ảnh mới, tour.HinhAnhDaiDien vẫn giữ giá trị cũ (do input hidden trong View)
                // ---------------------------------

                // --- BƯỚC 2: GỌI API LƯU DỮ LIỆU ---
                bool result = false;
                if (tour.MaTour > 0)
                {
                    // Có ID -> Gọi API Sửa (PUT)
                    result = PutToApi("api/admintour", tour);
                }
                else
                {
                    // Không ID -> Gọi API Thêm (POST)
                    result = PostToApi("api/admintour", tour);
                }

                if (result)
                    return Json(new { success = true, message = "Lưu dữ liệu thành công!" });
                else
                    return Json(new { success = false, message = "Lỗi khi gọi API (Kiểm tra server API)." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi server: " + ex.Message });
            }
        }

        // 4. XÓA TOUR
        [HttpPost]
        public ActionResult Delete(int id)
        {
            // Gọi API Xóa: api/admintour/delete/{id}
            bool result = DeleteFromApi("api/admintour/delete/" + id);

            if (result) return Json(new { success = true });

            return Json(new { success = false, message = "Không thể xóa tour này (Đang có đơn đặt hàng hoặc lỗi ràng buộc)." });
        }
    }
}