using QUANLYDICHVUDULICH.Admin.Controllers;
using QUANLYDICHVUDULICH.ADMIN.Models;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace QUANLYDICHVUDULICH.ADMIN.Controllers
{
    public class TinTucController : BaseController
    {
        // 1. Hiển thị danh sách & Tìm kiếm
        public ActionResult Index(string searchString, string category)
        {
            try
            {
                var list = GetFromApi<List<TinTucViewModel>>("api/admintintuc");
                if (list == null) list = new List<TinTucViewModel>();

                // Tìm kiếm
                if (!String.IsNullOrEmpty(searchString))
                {
                    searchString = searchString.ToLower().Trim();
                    list = list.FindAll(s => s.TieuDe.ToLower().Contains(searchString));
                }

                // Lọc danh mục
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

        // 2. Hàm lấy chi tiết Tin tức (JSON) -> DÙNG CHO MODAL XEM CHI TIẾT
        [HttpGet]
        public ActionResult GetJsonTinTuc(int id)
        {
            try
            {
                var tin = GetFromApi<TinTucViewModel>("api/admintintuc/" + id);
                return Json(tin, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }

        // 3. Trang Thêm mới
        public ActionResult Create()
        {
            return View();
        }

        // 4. Xử lý Thêm mới
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Create(TinTucViewModel model)
        {
            model.NgayDang = DateTime.Now;
            model.TacGia = Session["User"] != null ? Session["User"].ToString() : "Admin";

            if (PostToApi("api/admintintuc", model))
            {
                return RedirectToAction("Index");
            }
            ModelState.AddModelError("", "Lỗi khi thêm bài viết.");
            return View(model);
        }

        // 5. Trang Sửa
        public ActionResult Edit(int id)
        {
            var model = GetFromApi<TinTucViewModel>("api/admintintuc/" + id);
            return View(model);
        }

        // 6. Xử lý Cập nhật
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(TinTucViewModel model)
        {
            if (PutToApi("api/admintintuc", model))
            {
                return RedirectToAction("Index");
            }
            ModelState.AddModelError("", "Lỗi khi cập nhật.");
            return View(model);
        }

        // 7. Xử lý Xóa (AJAX)
        [HttpPost]
        public ActionResult Delete(int id)
        {
            bool result = DeleteFromApi("api/admintintuc/delete/" + id);

            if (result)
            {
                return Json(new { success = true, message = "Xóa bài viết thành công" });
            }
            else
            {
                return Json(new { success = false, message = "Lỗi khi gọi API xóa" });
            }
        }
    }
}