using System;
using System.Net.Http;
using System.Web.Mvc;
// Nhớ using Models để dùng class DashboardViewModel sau này nếu cần
using QUANLYDICHVUDULICH.Admin.Models;

namespace QUANLYDICHVUDULICH.Admin.Controllers
{
    // Kế thừa BaseController để tận dụng biến _client và cấu hình
    public class AccountController : BaseController
    {
        // GET: Trang đăng nhập
        public ActionResult Login()
        {
            // Nếu đã đăng nhập rồi thì đá sang trang chủ luôn
            if (Session["User"] != null)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        // POST: Xử lý đăng nhập khi bấm nút
        [HttpPost]
        public ActionResult Login(string username, string password)
        {
            try
            {
                // 1. Tạo dữ liệu để gửi sang API
                var loginData = new
                {
                    Email = username,
                    MatKhau = password
                };

                // 2. Gọi API Login (Sử dụng _client từ BaseController)
                // Đường dẫn API: api/auth/login
                var response = _client.PostAsJsonAsync("api/auth/login", loginData).Result;

                // 3. Kiểm tra kết quả trả về
                if (response.IsSuccessStatusCode)
                {
                    // A. Thành công: Đọc dữ liệu User trả về
                    var userResult = response.Content.ReadAsAsync<dynamic>().Result;

                    // B. Lưu vào Session (Để các trang khác kiểm tra)
                    Session["User"] = userResult.HoTen.ToString(); // Lưu Họ tên để hiển thị
                    Session["UserEmail"] = userResult.Email.ToString();
                    Session["Role"] = userResult.VaiTro.ToString();

                    // C. Chuyển hướng vào Dashboard
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    // D. Thất bại: Đọc thông báo lỗi từ API
                    string errorMsg = response.Content.ReadAsStringAsync().Result;
                    // API trả về dạng JSON lỗi, ta lọc lấy text (tùy chọn) hoặc hiện luôn
                    ViewBag.Error = "Đăng nhập thất bại: " + errorMsg.Replace("\"", "");
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Lỗi kết nối đến Server API: " + ex.Message;
            }

            // Nếu lỗi thì quay lại trang Login
            return View();
        }

        // Đăng xuất
        public ActionResult Logout()
        {
            Session.Clear(); // Xóa sạch session
            return RedirectToAction("Login");
        }
    }
}