using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Web.Http;
using QUANLYDICHVUDULICH.API.Models;

namespace QUANLYDICHVUDULICH.API.Controllers
{
    // ĐỔI ROUTE PREFIX CHO ĐỒNG BỘ (Khuyên dùng: api/admintintuc)
    // Nếu bạn muốn giữ api/tintuc thì sửa ở đây và cả bên Frontend
    [RoutePrefix("api/admintintuc")]
    public class AdminTinTucController : BaseApiController
    {
        // 1. LẤY DANH SÁCH (GET api/admintintuc)
        [HttpGet]
        [Route("")]
        public IHttpActionResult GetTinTucs()
        {
            List<TinTucDTO> list = new List<TinTucDTO>();
            try
            {
                string sql = @"SELECT t.MaTin, t.TieuDe, t.TomTat, t.HinhAnh, t.NgayDang, t.TrangThai, u.HoTen 
                               FROM TinTuc t LEFT JOIN NguoiDung u ON t.MaND = u.MaND ORDER BY t.MaTin DESC";
                DataTable dt = ExecuteQuery(sql);
                foreach (DataRow row in dt.Rows)
                {
                    list.Add(new TinTucDTO
                    {
                        MaTin = Convert.ToInt32(row["MaTin"]),
                        TieuDe = row["TieuDe"].ToString(),
                        TomTat = row["TomTat"] != DBNull.Value ? row["TomTat"].ToString() : "",
                        HinhAnh = row["HinhAnh"] != DBNull.Value ? row["HinhAnh"].ToString() : "",
                        DanhMuc = "Tin tức", // Mặc định
                        NgayDang = row["NgayDang"] != DBNull.Value ? Convert.ToDateTime(row["NgayDang"]) : DateTime.Now,
                        TrangThai = row["TrangThai"] != DBNull.Value ? row["TrangThai"].ToString() : "Đăng",
                        TacGia = row["HoTen"] != DBNull.Value ? row["HoTen"].ToString() : "Admin"
                    });
                }
                return Ok(list);
            }
            catch (Exception ex) { return BadRequest("Lỗi lấy danh sách: " + ex.Message); }
        }

        // 2. LẤY CHI TIẾT (GET api/admintintuc/5)
        [HttpGet]
        [Route("{id}")]
        public IHttpActionResult GetChiTiet(int id)
        {
            try
            {
                string sql = "SELECT * FROM TinTuc WHERE MaTin = @MaTin";
                SqlParameter[] param = { new SqlParameter("@MaTin", id) };
                DataTable dt = ExecuteQuery(sql, param);

                if (dt.Rows.Count > 0)
                {
                    DataRow row = dt.Rows[0];
                    var tin = new TinTucDTO
                    {
                        MaTin = Convert.ToInt32(row["MaTin"]),
                        TieuDe = row["TieuDe"].ToString(),
                        TomTat = row["TomTat"] != DBNull.Value ? row["TomTat"].ToString() : "",
                        NoiDung = row["NoiDung"] != DBNull.Value ? row["NoiDung"].ToString() : "",
                        HinhAnh = row["HinhAnh"] != DBNull.Value ? row["HinhAnh"].ToString() : "",
                        TrangThai = row["TrangThai"] != DBNull.Value ? row["TrangThai"].ToString() : "Đăng",
                        DanhMuc = "Tin tức"
                    };
                    return Ok(tin);
                }
                return NotFound();
            }
            catch (Exception ex) { return BadRequest(ex.Message); }
        }

        // 3. THÊM MỚI (POST api/admintintuc)
        [HttpPost]
        [Route("")]
        public IHttpActionResult Create([FromBody] TinTucDTO model)
        {
            try
            {
                if (model == null) return BadRequest("Dữ liệu rỗng");

                string sql = @"INSERT INTO TinTuc (TieuDe, TomTat, NoiDung, HinhAnh, NgayDang, TrangThai, MaND)
                               VALUES (@TieuDe, @TomTat, @NoiDung, @HinhAnh, GETDATE(), @TrangThai, 1)";

                SqlParameter[] param = {
                    new SqlParameter("@TieuDe", model.TieuDe ?? (object)DBNull.Value),
                    new SqlParameter("@TomTat", model.TomTat ?? (object)DBNull.Value),
                    new SqlParameter("@NoiDung", model.NoiDung ?? (object)DBNull.Value),
                    new SqlParameter("@HinhAnh", model.HinhAnh ?? (object)DBNull.Value),
                    new SqlParameter("@TrangThai", model.TrangThai ?? "Đăng")
                };

                ExecuteNonQuery(sql, param, false);
                return Ok("Thêm thành công");
            }
            catch (Exception ex) { return BadRequest("Lỗi thêm mới: " + ex.Message); }
        }

        // 4. CẬP NHẬT (PUT api/admintintuc)
        [HttpPut]
        [Route("")]
        public IHttpActionResult Update([FromBody] TinTucDTO model)
        {
            try
            {
                string sql = @"UPDATE TinTuc SET TieuDe=@TieuDe, TomTat=@TomTat, NoiDung=@NoiDung, 
                               HinhAnh=@HinhAnh, TrangThai=@TrangThai WHERE MaTin=@MaTin";

                SqlParameter[] param = {
                    new SqlParameter("@TieuDe", model.TieuDe),
                    new SqlParameter("@TomTat", model.TomTat ?? (object)DBNull.Value),
                    new SqlParameter("@NoiDung", model.NoiDung ?? (object)DBNull.Value),
                    new SqlParameter("@HinhAnh", model.HinhAnh ?? (object)DBNull.Value),
                    new SqlParameter("@TrangThai", model.TrangThai ?? "Đăng"),
                    new SqlParameter("@MaTin", model.MaTin)
                };
                ExecuteNonQuery(sql, param, false);
                return Ok("Cập nhật thành công");
            }
            catch (Exception ex) { return BadRequest("Lỗi cập nhật: " + ex.Message); }
        }

        // 5. XÓA (DELETE api/admintintuc/delete/5)
        // Đã nâng cấp để xử lý lỗi ràng buộc
        [HttpDelete]
        [Route("delete/{id}")]
        public IHttpActionResult Delete(int id)
        {
            try
            {
                // BƯỚC 1: Xóa các dữ liệu phụ liên quan nếu có (Ví dụ comment, tag...)
                // ExecuteNonQuery("DELETE FROM CommentTinTuc WHERE MaTin = @id", new SqlParameter[] { new SqlParameter("@id", id) });

                // BƯỚC 2: Xóa bài viết chính
                string sql = "DELETE FROM TinTuc WHERE MaTin = @MaTin";
                SqlParameter[] param = { new SqlParameter("@MaTin", id) };

                int rows = ExecuteNonQuery(sql, param, false);

                if (rows > 0)
                    return Ok(new { message = "Đã xóa bài viết thành công" });
                else
                    return BadRequest("Bài viết không tồn tại hoặc đã bị xóa.");
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("REFERENCE")) // Bắt lỗi khóa ngoại SQL
                    return BadRequest("Không thể xóa bài viết này vì đang được sử dụng ở nơi khác.");

                return BadRequest("Lỗi xóa: " + ex.Message);
            }
        }
    }
}