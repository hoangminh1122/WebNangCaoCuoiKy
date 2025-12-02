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
        public ActionResult Index(string searchString)
        {
            try
            {
                // 1. Gọi API lấy toàn bộ danh sách
                var tours = GetFromApi<List<TourViewModel>>("api/admintour");

                // 2. Nếu API trả về null (lỗi hoặc rỗng), khởi tạo list mới để tránh lỗi NullReference
                if (tours == null) tours = new List<TourViewModel>();

                // 3. XỬ LÝ TÌM KIẾM (Chuẩn hóa chuỗi)
                if (!string.IsNullOrEmpty(searchString))
                {
                    searchString = searchString.Trim().ToLower(); // Xóa khoảng trắng thừa, chuyển thường

                    // Lọc theo Tên Tour HOẶC Mã Tour
                    tours = tours.FindAll(t =>
                        (t.TenTour != null && t.TenTour.ToLower().Contains(searchString)) ||
                        t.MaTour.ToString().Contains(searchString)
                    );
                }

                // 4. Trả về View (Đảm bảo Model là IEnumerable<TourViewModel>)
                return View(tours);
            }
            catch
            {
                return View(new List<TourViewModel>());
            }
        }

        // 2. Trả về Form (Modal) - GIỮ NGUYÊN
        public ActionResult GetModal(int? id)
        {
            TourViewModel model = new TourViewModel();
            if (id.HasValue && id.Value > 0)
            {
                var tourFromApi = GetFromApi<TourViewModel>("api/admintour/" + id);
                if (tourFromApi != null) model = tourFromApi;
            }
            return PartialView("_FormTour", model);
        }

        // 3. LƯU DỮ LIỆU (CÓ XỬ LÝ UPLOAD ẢNH) - GIỮ NGUYÊN
        [HttpPost]
        public ActionResult Save(TourViewModel tour)
        {
            // --- XỬ LÝ FILE UPLOAD ---
            var file = Request.Files["ImageFile"];
            if (file != null && file.ContentLength > 0)
            {
                string fileName = "tour_" + Guid.NewGuid() + Path.GetExtension(file.FileName);
                string uploadPath = Server.MapPath("~/Content/Images/");

                if (!Directory.Exists(uploadPath))
                {
                    Directory.CreateDirectory(uploadPath);
                }

                file.SaveAs(Path.Combine(uploadPath, fileName));
                tour.HinhAnhDaiDien = "/Content/Images/" + fileName;
            }
            // --------------------------

            bool result = false;
            if (tour.MaTour > 0)
            {
                // Gọi API Sửa
                result = PutToApi("api/admintour", tour);
            }
            else
            {
                // Gọi API Thêm
                result = PostToApi("api/admintour", tour);
            }

            if (result)
                return Json(new { success = true, message = "Lưu thành công!" });
            else
                return Json(new { success = false, message = "Lỗi khi gọi API." });
        }

        // 4. Xóa Tour - SỬA LẠI ĐƯỜNG DẪN API CHO CHUẨN
        [HttpPost]
        public ActionResult Delete(int id)
        {
            // API mới đổi thành: api/admintour/delete/{id}
            bool result = DeleteFromApi("api/admintour/delete/" + id);

            if (result) return Json(new { success = true });

            // Trả về false kèm thông báo nếu xóa thất bại (do ràng buộc)
            return Json(new { success = false, message = "Không thể xóa tour này (Đã có đơn đặt hoặc lỗi ràng buộc)." });
        }
    }
}