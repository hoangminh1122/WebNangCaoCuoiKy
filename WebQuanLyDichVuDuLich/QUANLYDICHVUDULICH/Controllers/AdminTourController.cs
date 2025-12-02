using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Web.Http;
using QUANLYDICHVUDULICH.API.Models;

namespace QUANLYDICHVUDULICH.API.Controllers
{
    public class AdminTourController : BaseApiController
    {
        // 1. GET: Lấy danh sách Tour
        [HttpGet]
        [Route("api/admintour")] // Thêm Route rõ ràng
        public IHttpActionResult GetTours()
        {
            List<TourModel> list = new List<TourModel>();
            try
            {
                string sql = "SELECT * FROM Tour ORDER BY MaTour DESC";
                DataTable dt = ExecuteQuery(sql);

                foreach (DataRow row in dt.Rows)
                {
                    var tour = new TourModel();
                    tour.MaTour = Convert.ToInt32(row["MaTour"]);
                    tour.TenTour = row["TenTour"] != DBNull.Value ? row["TenTour"].ToString() : "Chưa đặt tên";
                    tour.MaDanhMuc = row["MaDanhMuc"] != DBNull.Value ? Convert.ToInt32(row["MaDanhMuc"]) : 0;
                    tour.GiaGoc = row["GiaGoc"] != DBNull.Value ? Convert.ToDecimal(row["GiaGoc"]) : 0;
                    tour.ThoiLuongNgay = row["ThoiLuongNgay"] != DBNull.Value ? Convert.ToInt32(row["ThoiLuongNgay"]) : 0;
                    tour.TrangThai = row["TrangThai"] != DBNull.Value ? row["TrangThai"].ToString() : "Đang mở bán";
                    tour.HinhAnhDaiDien = row["HinhAnhDaiDien"] != DBNull.Value ? row["HinhAnhDaiDien"].ToString() : "";

                    list.Add(tour);
                }
                return Ok(list);
            }
            catch (Exception ex)
            {
                return BadRequest("Lỗi lấy danh sách: " + ex.Message);
            }
        }

        // 2. POST: Thêm mới Tour
        [HttpPost]
        [Route("api/admintour")]
        public IHttpActionResult CreateTour([FromBody] TourModel tour)
        {
            if (tour == null) return BadRequest("Dữ liệu rỗng");
            try
            {
                // Nếu bạn chưa viết Stored Procedure sp_ThemTour, hãy đổi thành câu SQL INSERT thường nhé
                // Ở đây tôi giả định bạn dùng SQL thường cho an toàn nếu quên tạo SP
                string sql = @"INSERT INTO Tour (MaDanhMuc, TenTour, GiaGoc, ThoiLuongNgay, TrangThai) 
                               VALUES (@MaDanhMuc, @TenTour, @GiaGoc, @ThoiLuongNgay, @TrangThai)";

                SqlParameter[] param = new SqlParameter[]
                {
                    new SqlParameter("@MaDanhMuc", tour.MaDanhMuc),
                    new SqlParameter("@TenTour", tour.TenTour ?? (object)DBNull.Value),
                    new SqlParameter("@GiaGoc", tour.GiaGoc),
                    new SqlParameter("@ThoiLuongNgay", tour.ThoiLuongNgay),
                    new SqlParameter("@HinhAnhDaiDien", tour.HinhAnhDaiDien ?? (object)DBNull.Value),
new SqlParameter("@TrangThai", tour.TrangThai ?? "Đang mở bán")
                };

                // false vì là câu SQL thường
                ExecuteNonQuery(sql, param, false);
                return Ok("Thêm thành công");
            }
            catch (Exception ex)
            {
                return BadRequest("Lỗi thêm: " + ex.Message);
            }
        }

        // 3. PUT: Cập nhật Tour
        [HttpPut]
        [Route("api/admintour")] // Dùng chung đường dẫn, khác Method
        public IHttpActionResult UpdateTour([FromBody] TourModel tour)
        {
            try
            {
                string sql = @"UPDATE Tour 
                               SET TenTour=@TenTour, MaDanhMuc=@MaDanhMuc, GiaGoc=@GiaGoc, 
                                   ThoiLuongNgay=@ThoiLuongNgay, TrangThai=@TrangThai 
                               WHERE MaTour=@MaTour";

                SqlParameter[] param = new SqlParameter[]
                {
                    new SqlParameter("@MaTour", tour.MaTour),
                    new SqlParameter("@MaDanhMuc", tour.MaDanhMuc),
                    new SqlParameter("@TenTour", tour.TenTour ?? (object)DBNull.Value),
                    new SqlParameter("@GiaGoc", tour.GiaGoc),
                    new SqlParameter("@ThoiLuongNgay", tour.ThoiLuongNgay),
                    new SqlParameter("@HinhAnhDaiDien", tour.HinhAnhDaiDien ?? (object)DBNull.Value),
                    new SqlParameter("@TrangThai", tour.TrangThai ?? "Đang mở bán")
                };

                ExecuteNonQuery(sql, param, false);
                return Ok("Cập nhật thành công");
            }
            catch (Exception ex)
            {
                return BadRequest("Lỗi cập nhật: " + ex.Message);
            }
        }

        [HttpDelete]
        [Route("api/admintour/delete/{id}")] // Đổi thành delete/{id} cho rõ nghĩa và khớp với Frontend
        public IHttpActionResult DeleteTour(int id)
        {
            try
            {
                // BƯỚC 1: Xóa các dữ liệu phụ liên quan trước (để tránh lỗi Khóa ngoại)
                // Lưu ý: Nếu trong SQL bạn đã set ON DELETE CASCADE thì không cần bước này, nhưng viết ra cho chắc ăn

                // 1. Xóa trong bảng ThanhToan (liên quan đến DatTour)
                string sqlDelThanhToan = "DELETE FROM ThanhToan WHERE MaDatTour IN (SELECT MaDatTour FROM DatTour WHERE MaTour = @id)";
                ExecuteNonQuery(sqlDelThanhToan, new SqlParameter[] { new SqlParameter("@id", id) }, false);

                // 2. Xóa trong bảng DatTour (Những người đã đặt tour này)
                string sqlDelDatTour = "DELETE FROM DatTour WHERE MaTour = @id";
                ExecuteNonQuery(sqlDelDatTour, new SqlParameter[] { new SqlParameter("@id", id) }, false);

                // 3. Xóa Hình ảnh, Đánh giá, Dịch vụ đi kèm...
                ExecuteNonQuery("DELETE FROM TourHinhAnh WHERE MaTour = @id", new SqlParameter[] { new SqlParameter("@id", id) }, false);
                ExecuteNonQuery("DELETE FROM Tour_DichVu WHERE MaTour = @id", new SqlParameter[] { new SqlParameter("@id", id) }, false);
                ExecuteNonQuery("DELETE FROM Tour_KhuyenMai WHERE MaTour = @id", new SqlParameter[] { new SqlParameter("@id", id) }, false);
                ExecuteNonQuery("DELETE FROM DanhGia WHERE MaTour = @id", new SqlParameter[] { new SqlParameter("@id", id) }, false);

                // BƯỚC 2: Cuối cùng mới xóa Tour chính
                string sqlDelTour = "DELETE FROM Tour WHERE MaTour = @id";
                int rows = ExecuteNonQuery(sqlDelTour, new SqlParameter[] { new SqlParameter("@id", id) }, false);

                if (rows > 0)
                    return Ok("Đã xóa thành công tour và các dữ liệu liên quan.");
                else
                    return BadRequest("Tour không tồn tại hoặc đã bị xóa trước đó.");
            }
            catch (Exception ex)
            {
                // Trả về lỗi chi tiết
                return BadRequest("Lỗi SQL: " + ex.Message);
            }
        }

        // 5. GET: Lấy chi tiết 1 Tour (Đã sửa lỗi bảo mật)
        [HttpGet]
        [Route("api/admintour/{id}")]
        public IHttpActionResult GetTourById(int id)
        {
            // SỬA: Dùng tham số @id thay vì cộng chuỗi "MaTour = " + id
            string sql = "SELECT * FROM Tour WHERE MaTour = @id";
            SqlParameter[] param = { new SqlParameter("@id", id) };

            DataTable dt = ExecuteQuery(sql, param);
            if (dt.Rows.Count > 0)
            {
                DataRow row = dt.Rows[0];
                var tour = new TourModel
                {
                    MaTour = Convert.ToInt32(row["MaTour"]),
                    TenTour = row["TenTour"].ToString(),
                    MaDanhMuc = row["MaDanhMuc"] != DBNull.Value ? Convert.ToInt32(row["MaDanhMuc"]) : 0,
                    GiaGoc = row["GiaGoc"] != DBNull.Value ? Convert.ToDecimal(row["GiaGoc"]) : 0,
                    ThoiLuongNgay = row["ThoiLuongNgay"] != DBNull.Value ? Convert.ToInt32(row["ThoiLuongNgay"]) : 0,
                    TrangThai = row["TrangThai"].ToString()
                };
                return Ok(tour);
            }
            return NotFound();
        }
    }
}