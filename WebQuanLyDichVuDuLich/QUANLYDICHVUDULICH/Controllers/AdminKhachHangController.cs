using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Web.Http;
using QUANLYDICHVUDULICH.API.Models;

namespace QUANLYDICHVUDULICH.API.Controllers
{
    // ĐỔI ROUTE CHO CHUẨN
    [RoutePrefix("api/adminkhachhang")]
    public class AdminKhachHangController : BaseApiController
    {
        // 1. LẤY DANH SÁCH KHÁCH HÀNG (GET api/adminkhachhang)
        [HttpGet]
        [Route("")]
        public IHttpActionResult GetKhachHangs()
        {
            List<KhachHangDTO> list = new List<KhachHangDTO>();
            string sql = @"SELECT MaND, HoTen, Email, SoDienThoai, NgayTao, TrangThai 
                           FROM NguoiDung 
                           WHERE VaiTro = 'User' 
                           ORDER BY MaND DESC";

            try
            {
                DataTable dt = ExecuteQuery(sql);
                foreach (DataRow row in dt.Rows)
                {
                    list.Add(new KhachHangDTO
                    {
                        MaND = Convert.ToInt32(row["MaND"]),
                        HoTen = row["HoTen"].ToString(),
                        Email = row["Email"].ToString(),
                        SoDienThoai = row["SoDienThoai"] != DBNull.Value ? row["SoDienThoai"].ToString() : "",
                        NgayTao = Convert.ToDateTime(row["NgayTao"]),
                        TrangThai = Convert.ToBoolean(row["TrangThai"])
                    });
                }
                return Ok(list);
            }
            catch (Exception ex) { return BadRequest(ex.Message); }
        }

        // 2. KHÓA / MỞ KHÓA TÀI KHOẢN (POST api/adminkhachhang/toggle-status)
        // Nhận JSON: { "MaND": 5 }
        [HttpPost]
        [Route("toggle-status")]
        public IHttpActionResult ToggleStatus([FromBody] dynamic data)
        {
            try
            {
                if (data == null) return BadRequest("Dữ liệu rỗng");

                int id = (int)data.MaND;

                // Logic: Đảo ngược trạng thái hiện tại (Đang 1 thành 0, đang 0 thành 1)
                string sql = "UPDATE NguoiDung SET TrangThai = CASE WHEN TrangThai = 1 THEN 0 ELSE 1 END WHERE MaND = @MaND";
                SqlParameter[] param = { new SqlParameter("@MaND", id) };

                int rows = ExecuteNonQuery(sql, param, false);

                if (rows > 0)
                    return Ok(new { message = "Đổi trạng thái thành công" });
                else
                    return BadRequest("Không tìm thấy khách hàng này.");
            }
            catch (Exception ex) { return BadRequest(ex.Message); }
        }
        [HttpGet]
        [Route("{id}")]
        public IHttpActionResult GetKhachHangById(int id)
        {
            try
            {
                string sql = "SELECT * FROM NguoiDung WHERE MaND = @id";
                SqlParameter[] param = { new SqlParameter("@id", id) };
                DataTable dt = ExecuteQuery(sql, param);

                if (dt.Rows.Count > 0)
                {
                    DataRow row = dt.Rows[0];
                    var kh = new KhachHangDTO
                    {
                        MaND = Convert.ToInt32(row["MaND"]),
                        HoTen = row["HoTen"].ToString(),
                        Email = row["Email"].ToString(),
                        SoDienThoai = row["SoDienThoai"] != DBNull.Value ? row["SoDienThoai"].ToString() : "Chưa cập nhật",
                        NgayTao = Convert.ToDateTime(row["NgayTao"]),
                        TrangThai = Convert.ToBoolean(row["TrangThai"])
                    };
                    return Ok(kh);
                }
                return NotFound();
            }
            catch (Exception ex) { return BadRequest(ex.Message); }
        }
    }
}