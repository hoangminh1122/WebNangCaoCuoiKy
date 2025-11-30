using System.Collections.Generic;
using System.Web.Mvc;
using QUANLYDICHVUDULICH.Admin.Controllers;
using QUANLYDICHVUDULICH.ADMIN.Models;

namespace QUANLYDICHVUDULICH.ADMIN.Controllers
{
    public class KhachHangController : BaseController
    {
        // 1. Hiển thị danh sách
        public ActionResult Index(string searchString, string statusFilter)
        {
            var list = GetFromApi<List<KhachHangViewModel>>("api/khachhang");
            if (list == null) list = new List<KhachHangViewModel>();

            // Lọc theo tên/email/sdt
            if (!string.IsNullOrEmpty(searchString))
            {
                searchString = searchString.ToLower();
                list = list.FindAll(s => s.HoTen.ToLower().Contains(searchString) ||
                                         s.Email.ToLower().Contains(searchString) ||
                                         s.SoDienThoai.Contains(searchString));
            }

            // Lọc theo trạng thái (Hoạt động / Bị khóa)
            if (!string.IsNullOrEmpty(statusFilter))
            {
                if (statusFilter == "active") list = list.FindAll(s => s.TrangThai == true);
                if (statusFilter == "locked") list = list.FindAll(s => s.TrangThai == false);
            }

            return View(list);
        }

        // 2. Xử lý khóa/mở khóa
        public ActionResult ToggleStatus(int id)
        {
            // Gọi API đổi trạng thái (Post rỗng vì ID đã truyền trên URL, hoặc dùng object nặc danh)
            PostToApi("api/khachhang/toggle-status?id=" + id, new object());
            return RedirectToAction("Index");
        }
    }
}