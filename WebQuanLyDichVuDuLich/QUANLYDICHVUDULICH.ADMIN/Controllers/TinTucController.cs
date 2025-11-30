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
            try
            {
                // SỬA ĐƯỜNG DẪN: api/tintuc -> api/admintintuc
                var list = GetFromApi<List<TinTucViewModel>>("api/tintuc");
                if (list == null) list = new List<TinTucViewModel>();

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

        // 2. Trang Thêm mới
        public ActionResult Create()
        {
            return View();
        }

        // 3. Xử lý Thêm mới
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Create(TinTucViewModel model)
        {
            model.NgayDang = DateTime.Now;
            model.TacGia = Session["User"] != null ? Session["User"].ToString() : "Admin";

            // SỬA ĐƯỜNG DẪN
            if (PostToApi("api/tintuc", model))
            {
                return RedirectToAction("Index");
            }
            ModelState.AddModelError("", "Lỗi khi thêm bài viết.");
            return View(model);
        }

        // 4. Trang Sửa
        public ActionResult Edit(int id)
        {
            // SỬA ĐƯỜNG DẪN
            var model = GetFromApi<TinTucViewModel>("api/tintuc/" + id);
            return View(model);
        }

        // 5. Xử lý Cập nhật
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(TinTucViewModel model)
        {
            // SỬA ĐƯỜNG DẪN
            if (PutToApi("api/tintuc", model))
            {
                return RedirectToAction("Index");
            }
            ModelState.AddModelError("", "Lỗi khi cập nhật.");
            return View(model);
        }

        // 6. Xử lý Xóa (QUAN TRỌNG NHẤT)
        [HttpPost]
        public ActionResult Delete(int id)
        {
            // API bên kia quy định là: api/admintintuc/delete/{id}
            // Nên ở đây phải gọi đúng y hệt
            bool result = DeleteFromApi("api/tintuc/delete/" + id);

            if (result)
            {
                return Json(new { success = true, message = "Xóa bài viết thành công" });
            }
            else
            {
                // Nếu lỗi, API có thể trả về BadRequest kèm thông báo lỗi
                // Tuy nhiên hàm DeleteFromApi hiện tại chỉ trả về bool
                // Nên ta báo lỗi chung chung, hoặc bạn cần debug xem API báo gì
                return Json(new { success = false, message = "Lỗi khi gọi API xóa (Kiểm tra lại kết nối hoặc dữ liệu ràng buộc)" });
            }
        }
    }
}