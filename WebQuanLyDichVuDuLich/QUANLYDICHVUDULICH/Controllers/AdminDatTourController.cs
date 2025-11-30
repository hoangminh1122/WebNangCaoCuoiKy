using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Web.Http;
using QUANLYDICHVUDULICH.API.Models;

namespace QUANLYDICHVUDULICH.API.Controllers
{
    // Dùng RoutePrefix cho chuẩn
    [RoutePrefix("api/admindattour")]
    public class AdminDatTourController : BaseApiController
    {
        // 1. GET: Lấy danh sách (api/admindattour)
        [HttpGet]
        [Route("")]
        public IHttpActionResult GetDatTours()
        {
            List<DatTourDTO> list = new List<DatTourDTO>();
            // Câu lệnh SQL giữ nguyên như cũ
            string sql = @"
                SELECT d.MaDatTour, u.HoTen AS TenKhachHang, t.TenTour, 
                       t.NgayBatDau AS NgayKhoiHanh, d.TongTien, d.TrangThai
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
            catch (Exception ex) { return BadRequest("Lỗi truy vấn: " + ex.Message); }
        }

        // 2. POST: Cập nhật trạng thái
        [HttpPost]
        [Route("status")]
        public IHttpActionResult UpdateStatus([FromBody] dynamic data)
        {
            try
            {
                int id = (int)data.MaDatTour;
                string status = (string)data.TrangThai;
                string sql = "UPDATE DatTour SET TrangThai = @TrangThai WHERE MaDatTour = @MaDatTour";

                ExecuteNonQuery(sql, new SqlParameter[] {
                    new SqlParameter("@TrangThai", status),
                    new SqlParameter("@MaDatTour", id)
                }, false);

                return Ok(new { message = "Cập nhật thành công" });
            }
            catch (Exception ex) { return BadRequest("Lỗi cập nhật: " + ex.Message); }
        }

        // 3. DELETE: Xóa đơn hàng (SỬA LOGIC XÓA)
        // Route: api/admindattour/delete/5
        [HttpDelete]
        [Route("delete/{id}")]
        public IHttpActionResult DeleteOrder(int id)
        {
            try
            {
                // BƯỚC 1: Xóa các bảng phụ trước (Ví dụ ThanhToan)
                ExecuteNonQuery("DELETE FROM ThanhToan WHERE MaDatTour = @id", new SqlParameter[] { new SqlParameter("@id", id) });

                // BƯỚC 2: Xóa bảng chính
                string sql = "DELETE FROM DatTour WHERE MaDatTour = @id";
                int rows = ExecuteNonQuery(sql, new SqlParameter[] { new SqlParameter("@id", id) }, false);

                if (rows > 0)
                    return Ok(new { message = "Đã xóa đơn hàng" });
                else
                    return BadRequest("Đơn hàng không tồn tại hoặc đã bị xóa.");
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("REFERENCE"))
                    return BadRequest("Không thể xóa đơn này vì dính khóa ngoại.");

                return BadRequest("Lỗi xóa: " + ex.Message);
            }
        }
    }
}