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
        // 1. GET: Lấy danh sách Tour (Lấy đầy đủ thông tin)
        [HttpGet]
        [Route("api/admintour")]
        public IHttpActionResult GetTours()
        {
            List<TourModel> list = new List<TourModel>();
            try
            {
                // Truy vấn lấy tất cả các cột cần thiết
                string sql = "SELECT * FROM Tour ORDER BY MaTour DESC";
                DataTable dt = ExecuteQuery(sql);

                foreach (DataRow row in dt.Rows)
                {
                    var tour = new TourModel();
                    // Mapping dữ liệu an toàn với DBNull
                    tour.MaTour = Convert.ToInt32(row["MaTour"]);
                    tour.TenTour = row["TenTour"] != DBNull.Value ? row["TenTour"].ToString() : "";
                    tour.MaDanhMuc = row["MaDanhMuc"] != DBNull.Value ? Convert.ToInt32(row["MaDanhMuc"]) : 0;

                    // Xử lý các trường có thể null hoặc số
                    tour.GiaGoc = row["GiaGoc"] != DBNull.Value ? Convert.ToDecimal(row["GiaGoc"]) : 0;
                    tour.GiaKhuyenMai = row["GiaKhuyenMai"] != DBNull.Value ? Convert.ToDecimal(row["GiaKhuyenMai"]) : (decimal?)null;

                    tour.ThoiLuongNgay = row["ThoiLuongNgay"] != DBNull.Value ? Convert.ToInt32(row["ThoiLuongNgay"]) : 0;
                    tour.SoNguoiToiDa = row["SoNguoiToiDa"] != DBNull.Value ? Convert.ToInt32(row["SoNguoiToiDa"]) : (int?)null;
                    tour.SoChoConLai = row["SoChoConLai"] != DBNull.Value ? Convert.ToInt32(row["SoChoConLai"]) : (int?)null;

                    // Xử lý ngày tháng
                    if (row["NgayBatDau"] != DBNull.Value) tour.NgayBatDau = Convert.ToDateTime(row["NgayBatDau"]);
                    if (row["NgayKetThuc"] != DBNull.Value) tour.NgayKetThuc = Convert.ToDateTime(row["NgayKetThuc"]);

                    // Xử lý chuỗi
                    tour.TrangThai = row["TrangThai"] != DBNull.Value ? row["TrangThai"].ToString() : "Đang mở bán";
                    tour.HinhAnhDaiDien = row["HinhAnhDaiDien"] != DBNull.Value ? row["HinhAnhDaiDien"].ToString() : "";
                    tour.MoTa = row["MoTa"] != DBNull.Value ? row["MoTa"].ToString() : "";
                    tour.LichTrinh = row["LichTrinh"] != DBNull.Value ? row["LichTrinh"].ToString() : "";
                    tour.DiemKhoiHanh = row["DiemKhoiHanh"] != DBNull.Value ? row["DiemKhoiHanh"].ToString() : "";
                    tour.DiemDen = row["DiemDen"] != DBNull.Value ? row["DiemDen"].ToString() : "";
                    tour.PhuongTien = row["PhuongTien"] != DBNull.Value ? row["PhuongTien"].ToString() : "";

                    list.Add(tour);
                }
                return Ok(list);
            }
            catch (Exception ex)
            {
                return BadRequest("Lỗi lấy danh sách: " + ex.Message);
            }
        }

        // 2. POST: Thêm mới Tour (Nhận full thông tin từ Admin)
        [HttpPost]
        [Route("api/admintour")]
        public IHttpActionResult CreateTour([FromBody] TourModel tour)
        {
            if (tour == null) return BadRequest("Dữ liệu rỗng");
            try
            {
                // Câu lệnh SQL INSERT đầy đủ các trường
                string sql = @"INSERT INTO Tour 
                (MaDanhMuc, TenTour, MoTa, LichTrinh, ThoiLuongNgay, DiemKhoiHanh, DiemDen, PhuongTien, 
                 GiaGoc, GiaKhuyenMai, NgayBatDau, NgayKetThuc, SoNguoiToiDa, SoChoConLai, HinhAnhDaiDien, TrangThai) 
                VALUES 
                (@MaDanhMuc, @TenTour, @MoTa, @LichTrinh, @ThoiLuongNgay, @DiemKhoiHanh, @DiemDen, @PhuongTien, 
                 @GiaGoc, @GiaKhuyenMai, @NgayBatDau, @NgayKetThuc, @SoNguoiToiDa, @SoChoConLai, @HinhAnhDaiDien, @TrangThai)";

                // Tạo tham số (dùng hàm phụ trợ cho gọn hoặc viết trực tiếp)
                SqlParameter[] param = new SqlParameter[]
                {
                    new SqlParameter("@MaDanhMuc", tour.MaDanhMuc),
                    new SqlParameter("@TenTour", tour.TenTour ?? (object)DBNull.Value),
                    new SqlParameter("@MoTa", tour.MoTa ?? (object)DBNull.Value),
                    new SqlParameter("@LichTrinh", tour.LichTrinh ?? (object)DBNull.Value),
                    new SqlParameter("@ThoiLuongNgay", tour.ThoiLuongNgay), // Int mặc định 0 nếu không set
                    new SqlParameter("@DiemKhoiHanh", tour.DiemKhoiHanh ?? (object)DBNull.Value),
                    new SqlParameter("@DiemDen", tour.DiemDen ?? (object)DBNull.Value),
                    new SqlParameter("@PhuongTien", tour.PhuongTien ?? (object)DBNull.Value),
                    new SqlParameter("@GiaGoc", tour.GiaGoc),
                    new SqlParameter("@GiaKhuyenMai", tour.GiaKhuyenMai ?? (object)DBNull.Value),
                    new SqlParameter("@NgayBatDau", tour.NgayBatDau ?? (object)DBNull.Value),
                    new SqlParameter("@NgayKetThuc", tour.NgayKetThuc ?? (object)DBNull.Value),
                    new SqlParameter("@SoNguoiToiDa", tour.SoNguoiToiDa ?? (object)DBNull.Value),
                    new SqlParameter("@SoChoConLai", tour.SoChoConLai ?? (object)DBNull.Value), // Mặc định có thể set bằng SoNguoiToiDa nếu null
                    new SqlParameter("@HinhAnhDaiDien", tour.HinhAnhDaiDien ?? (object)DBNull.Value),
                    new SqlParameter("@TrangThai", tour.TrangThai ?? "Đang mở bán")
                };

                ExecuteNonQuery(sql, param, false);
                return Ok("Thêm thành công");
            }
            catch (Exception ex)
            {
                return BadRequest("Lỗi thêm: " + ex.Message);
            }
        }

        // 3. PUT: Cập nhật Tour (Nhận full thông tin)
        [HttpPut]
        [Route("api/admintour")]
        public IHttpActionResult UpdateTour([FromBody] TourModel tour)
        {
            try
            {
                string sql = @"UPDATE Tour SET 
                    MaDanhMuc=@MaDanhMuc, TenTour=@TenTour, MoTa=@MoTa, LichTrinh=@LichTrinh, 
                    ThoiLuongNgay=@ThoiLuongNgay, DiemKhoiHanh=@DiemKhoiHanh, DiemDen=@DiemDen, PhuongTien=@PhuongTien,
                    GiaGoc=@GiaGoc, GiaKhuyenMai=@GiaKhuyenMai, NgayBatDau=@NgayBatDau, NgayKetThuc=@NgayKetThuc,
                    SoNguoiToiDa=@SoNguoiToiDa, SoChoConLai=@SoChoConLai, HinhAnhDaiDien=@HinhAnhDaiDien, TrangThai=@TrangThai
                    WHERE MaTour=@MaTour";

                SqlParameter[] param = new SqlParameter[]
                {
                    new SqlParameter("@MaTour", tour.MaTour),
                    new SqlParameter("@MaDanhMuc", tour.MaDanhMuc),
                    new SqlParameter("@TenTour", tour.TenTour ?? (object)DBNull.Value),
                    new SqlParameter("@MoTa", tour.MoTa ?? (object)DBNull.Value),
                    new SqlParameter("@LichTrinh", tour.LichTrinh ?? (object)DBNull.Value),
                    new SqlParameter("@ThoiLuongNgay", tour.ThoiLuongNgay),
                    new SqlParameter("@DiemKhoiHanh", tour.DiemKhoiHanh ?? (object)DBNull.Value),
                    new SqlParameter("@DiemDen", tour.DiemDen ?? (object)DBNull.Value),
                    new SqlParameter("@PhuongTien", tour.PhuongTien ?? (object)DBNull.Value),
                    new SqlParameter("@GiaGoc", tour.GiaGoc),
                    new SqlParameter("@GiaKhuyenMai", tour.GiaKhuyenMai ?? (object)DBNull.Value),
                    new SqlParameter("@NgayBatDau", tour.NgayBatDau ?? (object)DBNull.Value),
                    new SqlParameter("@NgayKetThuc", tour.NgayKetThuc ?? (object)DBNull.Value),
                    new SqlParameter("@SoNguoiToiDa", tour.SoNguoiToiDa ?? (object)DBNull.Value),
                    new SqlParameter("@SoChoConLai", tour.SoChoConLai ?? (object)DBNull.Value),
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

        // 4. DELETE: Xóa Tour (Xử lý xóa sạch sẽ các bảng con)
        [HttpDelete]
        [Route("api/admintour/delete/{id}")]
        public IHttpActionResult DeleteTour(int id)
        {
            try
            {
                // BƯỚC 1: Xóa dữ liệu liên quan ở các bảng phụ trước

                // 1.1. Xóa ThanhToan (liên quan DatTour)
                string sqlDelThanhToan = "DELETE FROM ThanhToan WHERE MaDatTour IN (SELECT MaDatTour FROM DatTour WHERE MaTour = @id)";
                ExecuteNonQuery(sqlDelThanhToan, new SqlParameter[] { new SqlParameter("@id", id) }, false);

                // 1.2. Xóa DatTour
                string sqlDelDatTour = "DELETE FROM DatTour WHERE MaTour = @id";
                ExecuteNonQuery(sqlDelDatTour, new SqlParameter[] { new SqlParameter("@id", id) }, false);

                // 1.3. Xóa các bảng chi tiết khác
                ExecuteNonQuery("DELETE FROM TourHinhAnh WHERE MaTour = @id", new SqlParameter[] { new SqlParameter("@id", id) }, false);
                ExecuteNonQuery("DELETE FROM Tour_DichVu WHERE MaTour = @id", new SqlParameter[] { new SqlParameter("@id", id) }, false);
                ExecuteNonQuery("DELETE FROM Tour_KhuyenMai WHERE MaTour = @id", new SqlParameter[] { new SqlParameter("@id", id) }, false);
                ExecuteNonQuery("DELETE FROM DanhGia WHERE MaTour = @id", new SqlParameter[] { new SqlParameter("@id", id) }, false);

                // BƯỚC 2: Xóa Tour chính
                string sqlDelTour = "DELETE FROM Tour WHERE MaTour = @id";
                int rows = ExecuteNonQuery(sqlDelTour, new SqlParameter[] { new SqlParameter("@id", id) }, false);

                if (rows > 0)
                    return Ok("Đã xóa thành công tour và các dữ liệu liên quan.");
                else
                    return BadRequest("Tour không tồn tại hoặc đã bị xóa trước đó.");
            }
            catch (Exception ex)
            {
                return BadRequest("Lỗi SQL: " + ex.Message);
            }
        }

        // 5. GET: Lấy chi tiết 1 Tour (Full thông tin để Binding lên form Sửa)
        [HttpGet]
        [Route("api/admintour/{id}")]
        public IHttpActionResult GetTourById(int id)
        {
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
                    GiaKhuyenMai = row["GiaKhuyenMai"] != DBNull.Value ? Convert.ToDecimal(row["GiaKhuyenMai"]) : (decimal?)null,

                    ThoiLuongNgay = row["ThoiLuongNgay"] != DBNull.Value ? Convert.ToInt32(row["ThoiLuongNgay"]) : 0,
                    SoNguoiToiDa = row["SoNguoiToiDa"] != DBNull.Value ? Convert.ToInt32(row["SoNguoiToiDa"]) : (int?)null,
                    SoChoConLai = row["SoChoConLai"] != DBNull.Value ? Convert.ToInt32(row["SoChoConLai"]) : (int?)null,

                    TrangThai = row["TrangThai"] != DBNull.Value ? row["TrangThai"].ToString() : "Đang mở bán",
                    HinhAnhDaiDien = row["HinhAnhDaiDien"] != DBNull.Value ? row["HinhAnhDaiDien"].ToString() : "",

                    MoTa = row["MoTa"] != DBNull.Value ? row["MoTa"].ToString() : "",
                    LichTrinh = row["LichTrinh"] != DBNull.Value ? row["LichTrinh"].ToString() : "",
                    DiemKhoiHanh = row["DiemKhoiHanh"] != DBNull.Value ? row["DiemKhoiHanh"].ToString() : "",
                    DiemDen = row["DiemDen"] != DBNull.Value ? row["DiemDen"].ToString() : "",
                    PhuongTien = row["PhuongTien"] != DBNull.Value ? row["PhuongTien"].ToString() : ""
                };

                // Xử lý ngày tháng riêng để tránh lỗi
                if (row["NgayBatDau"] != DBNull.Value) tour.NgayBatDau = Convert.ToDateTime(row["NgayBatDau"]);
                if (row["NgayKetThuc"] != DBNull.Value) tour.NgayKetThuc = Convert.ToDateTime(row["NgayKetThuc"]);

                return Ok(tour);
            }
            return NotFound();
        }
    }
}