using QUANLYDICHVUDULICH.Admin.Controllers;
using QUANLYDICHVUDULICH.ADMIN.Models;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace QUANLYDICHVUDULICH.ADMIN.Controllers
{
    public class TinTucController : BaseController
    {
        // 1. Hiển thị danh sách (ĐÃ SỬA: Thêm tìm kiếm)
        public ActionResult Index(string searchString, string category)
        {
            try
            {
                // SỬA ĐƯỜNG DẪN CHUẨN: api/admintintuc
                var list = GetFromApi<List<TinTucViewModel>>("api/admintintuc");
                if (list == null) list = new List<TinTucViewModel>();

                // XỬ LÝ TÌM KIẾM (Mới thêm)
                if (!String.IsNullOrEmpty(searchString))
                {
                    searchString = searchString.ToLower().Trim();
                    list = list.FindAll(s => s.TieuDe.ToLower().Contains(searchString));
                }

                // XỬ LÝ LỌC DANH MỤC
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

            // SỬA ĐƯỜNG DẪN CHUẨN
            if (PostToApi("api/admintintuc", model))
            {
                return RedirectToAction("Index");
            }
            ModelState.AddModelError("", "Lỗi khi thêm bài viết.");
            return View(model);
        }

        // 4. Trang Sửa
        public ActionResult Edit(int id)
        {
            // SỬA ĐƯỜNG DẪN CHUẨN
            var model = GetFromApi<TinTucViewModel>("api/admintintuc/" + id);
            return View(model);
        }

        // 5. Xử lý Cập nhật
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(TinTucViewModel model)
        {
            // SỬA ĐƯỜNG DẪN CHUẨN
            if (PutToApi("api/admintintuc", model))
            {
                return RedirectToAction("Index");
            }
            ModelState.AddModelError("", "Lỗi khi cập nhật.");
            return View(model);
        }

        // 6. Xử lý Xóa (Giữ nguyên logic JSON cho AJAX)
        [HttpPost]
        public ActionResult Delete(int id)
        {
            // SỬA ĐƯỜNG DẪN CHUẨN: api/admintintuc/delete/{id}
            bool result = DeleteFromApi("api/admintintuc/delete/" + id);

            if (result)
            {
                return Json(new { success = true, message = "Xóa bài viết thành công" });
            }
            else
            {
                return Json(new { success = false, message = "Lỗi khi gọi API xóa (Kiểm tra lại kết nối hoặc dữ liệu ràng buộc)" });
            }
        }
    }
}