using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.Http;

namespace QUANLYDICHVUDULICH.API.Controllers
{
    // Kế thừa BaseApiController để dùng hàm kết nối SQL
    public class AuthApiController : BaseApiController
    {
        // Class nhỏ để hứng dữ liệu gửi lên
        public class LoginRequest
        {
            public string Email { get; set; }
            public string MatKhau { get; set; }
        }

        [HttpPost]
        [Route("api/auth/login")]
        public IHttpActionResult Login([FromBody] LoginRequest loginData)
        {
            if (loginData == null || string.IsNullOrEmpty(loginData.Email))
            {
                return BadRequest("Vui lòng nhập Email và Mật khẩu");
            }

            try
            {
                // 1. Viết câu lệnh SQL kiểm tra User và Pass
                // Lưu ý: Chỉ cho phép VaiTro là 'Admin' đăng nhập vào trang này
                string query = @"SELECT MaND, HoTen, Email, VaiTro 
                                 FROM NguoiDung 
                                 WHERE Email = @Email 
                                 AND MatKhau = @Pass 
                                 AND VaiTro = N'Admin' 
                                 AND TrangThai = 1";

                SqlParameter[] paramsList = new SqlParameter[]
                {
                    new SqlParameter("@Email", loginData.Email),
                    new SqlParameter("@Pass", loginData.MatKhau)
                };

                // 2. Gọi hàm ExecuteQuery từ BaseApiController
                DataTable dt = ExecuteQuery(query, paramsList);

                // 3. Kiểm tra kết quả
                if (dt.Rows.Count > 0)
                {
                    // Đăng nhập thành công -> Trả về thông tin User
                    DataRow row = dt.Rows[0];
                    return Ok(new
                    {
                        MaND = row["MaND"],
                        HoTen = row["HoTen"].ToString(),
                        Email = row["Email"].ToString(),
                        VaiTro = row["VaiTro"].ToString()
                    });
                }
                else
                {
                    // Đăng nhập thất bại
                    return BadRequest("Sai tài khoản, mật khẩu hoặc bạn không có quyền Admin!");
                }
            }
            catch (Exception ex)
            {
                return InternalServerError(new Exception("Lỗi server: " + ex.Message));
            }
        }
    }
}