using QUANLYDICHVUDULICH.Admin.Controllers;
using QUANLYDICHVUDULICH.ADMIN.Models;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace QUANLYDICHVUDULICH.ADMIN.Controllers
{
    public class TinTucController : BaseController
    {
        // 1. Hiển thị danh sách
        public ActionResult Index(string searchString, string category)
        {
            // Có thể mở rộng API để hỗ trợ tìm kiếm: api/tintuc?search=...
            // Ở đây lấy tất cả về rồi lọc tạm (nếu list ít), hoặc gọi API chuẩn
            try
            {
                var list = GetFromApi<List<TinTucViewModel>>("api/tintuc");
                if (list == null) list = new List<TinTucViewModel>();

                // Logic lọc dữ liệu đơn giản tại Client (Frontend)
                if (!String.IsNullOrEmpty(searchString))
                {
                    list = list.FindAll(s => s.TieuDe.ToLower().Contains(searchString.ToLower()));
                }
                if (!String.IsNullOrEmpty(category))
                {
                    list = list.FindAll(s => s.DanhMuc == category);
                }

                return View(list);
            }
            catch
            {
                return View(new List<TinTucViewModel>());
            }
        }

        // 2. Trang THÊM MỚI
        public ActionResult Create()
        {
            return View();
        }

        // 3. Xử lý LƯU
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Create(TinTucViewModel model)
        {
            // Gán các giá trị mặc định nếu cần
            model.NgayDang = DateTime.Now;
            model.TacGia = Session["User"] != null ? Session["User"].ToString() : "Admin";

            if (PostToApi("api/tintuc", model))
            {
                return RedirectToAction("Index");
            }
            ModelState.AddModelError("", "Lỗi khi thêm bài viết.");
            return View(model);
        }

        // 4. Trang SỬA
        public ActionResult Edit(int id)
        {
            var model = GetFromApi<TinTucViewModel>("api/tintuc/" + id);
            return View(model);
        }

        // 5. Xử lý CẬP NHẬT
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(TinTucViewModel model)
        {
            if (PutToApi("api/tintuc", model))
            {
                return RedirectToAction("Index");
            }
            ModelState.AddModelError("", "Lỗi khi cập nhật.");
            return View(model);
        }

        // 6. Xử lý XÓA (QUAN TRỌNG: Đã sửa để trả về JSON cho AJAX)
        [HttpPost]
        public ActionResult Delete(int id)
        {
            // Gọi API xóa
            bool result = DeleteFromApi("api/tintuc/" + id);

            if (result)
            {
                // Trả về JSON để SweetAlert hiển thị "Thành công"
                return Json(new { success = true, message = "Xóa bài viết thành công" });
            }
            else
            {
                // Trả về JSON lỗi
                return Json(new { success = false, message = "Lỗi khi gọi API xóa" });
            }
        }
    }
}