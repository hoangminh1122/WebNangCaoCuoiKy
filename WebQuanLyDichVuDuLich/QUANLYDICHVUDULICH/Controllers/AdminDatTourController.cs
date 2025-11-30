using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Web.Http;
using QUANLYDICHVUDULICH.API.Models;

namespace QUANLYDICHVUDULICH.API.Controllers
{
    // ĐỔI ROUTE CHÍNH CHO KHỚP VỚI FRONTEND
    [RoutePrefix("api/admindattour")]
    public class AdminDatTourController : BaseApiController
    {
        // 1. GET: Lấy danh sách (api/admindattour)
        [HttpGet]
        [Route("")]
        public IHttpActionResult GetDatTours()
        {
            List<DatTourDTO> list = new List<DatTourDTO>();

            // Query lấy thông tin chi tiết
            string sql = @"
                SELECT 
                    d.MaDatTour, 
                    u.HoTen AS TenKhachHang, 
                    t.TenTour, 
                    t.NgayBatDau AS NgayKhoiHanh, 
                    d.TongTien, 
                    d.TrangThai
                FROM DatTour d
                LEFT JOIN NguoiDung u ON d.MaND = u.MaND
                LEFT JOIN Tour t ON d.MaTour = t.MaTour
                ORDER BY d.MaDatTour DESC";

            try
            {
                DataTable dt = ExecuteQuery(sql);

                foreach (DataRow row in dt.Rows)
                {
                    list.Add(new DatTourDTO
                    {
                        MaDatTour = Convert.ToInt32(row["MaDatTour"]),
                        TenKhachHang = row["TenKhachHang"] != DBNull.Value ? row["TenKhachHang"].ToString() : "Khách vãng lai",
                        TenTour = row["TenTour"] != DBNull.Value ? row["TenTour"].ToString() : "Tour đã xóa",
                        NgayKhoiHanh = row["NgayKhoiHanh"] != DBNull.Value ? Convert.ToDateTime(row["NgayKhoiHanh"]) : DateTime.Now,
                        TongTien = row["TongTien"] != DBNull.Value ? Convert.ToDecimal(row["TongTien"]) : 0,
                        TrangThai = row["TrangThai"].ToString()
                    });
                }

                return Ok(list);
            }
            catch (Exception ex)
            {
                return BadRequest("Lỗi truy vấn: " + ex.Message);
            }
        }

        // 2. POST: Cập nhật trạng thái (Duyệt / Hủy)
        // Frontend gửi JSON: { "MaDatTour": 123, "TrangThai": "Đã xác nhận" }
        [HttpPost]
        [Route("status")]
        public IHttpActionResult UpdateStatus([FromBody] dynamic data)
        {
            if (data == null) return BadRequest("Dữ liệu rỗng");

            try
            {
                int id = (int)data.MaDatTour;
                string status = (string)data.TrangThai;

                string sql = "UPDATE DatTour SET TrangThai = @TrangThai WHERE MaDatTour = @MaDatTour";

                SqlParameter[] param = new SqlParameter[]
                {
                    new SqlParameter("@TrangThai", status),
                    new SqlParameter("@MaDatTour", id)
                };

                int rows = ExecuteNonQuery(sql, param, false);

                if (rows > 0)
                    return Ok(new { message = "Cập nhật thành công" });
                else
                    return BadRequest("Không tìm thấy đơn hàng cần cập nhật.");
            }
            catch (Exception ex)
            {
                return BadRequest("Lỗi cập nhật: " + ex.Message);
            }
        }

        // 3. DELETE: Xóa đơn hàng
        // Gọi: DELETE api/admindattour/5
        [HttpDelete]
        [Route("{id}")]
        public IHttpActionResult DeleteOrder(int id)
        {
            try
            {
                string sql = "DELETE FROM Tour WHERE MaDatTour = @id";
                ExecuteNonQuery(sql, new SqlParameter[] { new SqlParameter("@id", id) }, false);
                return Ok("Đã xóa");
            }
            catch (Exception ex)
            {
                // Bắt lỗi ràng buộc khóa ngoại (Foreign Key)
                if (ex.Message.Contains("REFERENCE"))
                    return BadRequest("Không thể xóa Đơn đặt Tour này!");

                return BadRequest("Lỗi xóa: " + ex.Message);
            }
        }
    }
}