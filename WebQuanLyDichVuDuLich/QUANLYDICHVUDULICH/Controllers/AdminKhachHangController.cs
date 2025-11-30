using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Web.Http;
using QUANLYDICHVUDULICH.API.Models;

namespace QUANLYDICHVUDULICH.API.Controllers
{
    public class AdminKhachHangController : BaseApiController
    {
        // 1. LẤY DANH SÁCH KHÁCH HÀNG (Chỉ lấy Role 'User')
        [HttpGet]
        [Route("api/khachhang")]
        public IHttpActionResult GetKhachHangs()
        {
            List<KhachHangDTO> list = new List<KhachHangDTO>();
            // SQL: Chỉ lấy người dùng thường, không lấy Admin
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
                        // Trong DB: 1 là Active, 0 là Locked
                        TrangThai = Convert.ToBoolean(row["TrangThai"])
                    });
                }
                return Ok(list);
            }
            catch (Exception ex) { return BadRequest(ex.Message); }
        }

        // 2. KHÓA / MỞ KHÓA TÀI KHOẢN
        [HttpPost]
        [Route("api/khachhang/toggle-status")]
        public IHttpActionResult ToggleStatus(int id)
        {
            try
            {
                // Logic: Đảo ngược trạng thái hiện tại (Đang 1 thành 0, đang 0 thành 1)
                string sql = "UPDATE NguoiDung SET TrangThai = CASE WHEN TrangThai = 1 THEN 0 ELSE 1 END WHERE MaND = @MaND";
                SqlParameter[] param = { new SqlParameter("@MaND", id) };

                ExecuteNonQuery(sql, param, false);
                return Ok("Đổi trạng thái thành công");
            }
            catch (Exception ex) { return BadRequest(ex.Message); }
        }
    }
}