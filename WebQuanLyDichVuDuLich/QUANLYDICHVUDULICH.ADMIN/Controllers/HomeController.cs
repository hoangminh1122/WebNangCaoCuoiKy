using System.Web.Mvc;
using QUANLYDICHVUDULICH.Admin.Models;

namespace QUANLYDICHVUDULICH.Admin.Controllers
{
    // Kế thừa BaseController để dùng hàm gọi API
    public class HomeController : BaseController
    {
        public ActionResult Index()
        {
            // Kiểm tra đăng nhập (Nếu chưa đăng nhập thì đá về trang Login)
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // Gọi API lấy số liệu thống kê thật từ SQL
            var model = GetFromApi<DashboardViewModel>("api/dashboard/stats");

            // Nếu API lỗi hoặc chưa có dữ liệu thì khởi tạo mặc định để không lỗi trang web
            if (model == null) model = new DashboardViewModel();

            return View(model);
        }
    }
}