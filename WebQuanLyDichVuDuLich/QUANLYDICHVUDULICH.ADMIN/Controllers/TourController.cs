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
        // 1. Lấy danh sách Tour
        public ActionResult Index()
        {
            try
            {
                var tours = GetFromApi<List<TourViewModel>>("api/admintour");
                if (tours == null) tours = new List<TourViewModel>();
                return View(tours);
            }
            catch
            {
                return View(new List<TourViewModel>());
            }
        }

        // 2. Trả về Form (Modal)
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

        // 3. LƯU DỮ LIỆU (CÓ XỬ LÝ UPLOAD ẢNH)
        [HttpPost]
        public ActionResult Save(TourViewModel tour)
        {
            // --- XỬ LÝ FILE UPLOAD ---
            var file = Request.Files["ImageFile"];
            if (file != null && file.ContentLength > 0)
            {
                // 1. Đặt tên file (dùng Guid để không trùng)
                string fileName = "tour_" + Guid.NewGuid() + Path.GetExtension(file.FileName);

                // 2. Định nghĩa đường dẫn lưu (Thư mục /Content/Images/)
                string uploadPath = Server.MapPath("~/Content/Images/");

                // 3. Tạo thư mục nếu chưa có
                if (!Directory.Exists(uploadPath))
                {
                    Directory.CreateDirectory(uploadPath);
                }

                // 4. Lưu file vào server
                file.SaveAs(Path.Combine(uploadPath, fileName));

                // 5. Gán đường dẫn vào Model để gửi sang API
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

        // 4. Xóa Tour
        [HttpPost]
        public ActionResult Delete(int id)
        {
            bool result = DeleteFromApi("api/admintour/" + id);
            if (result) return Json(new { success = true });
            return Json(new { success = false });
        }
    }
}