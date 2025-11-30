using System;
using System.Collections.Generic;
using System.Data;
using System.Web.Http;

namespace QUANLYDICHVUDULICH.API.Controllers
{
    public class DashboardApiController : BaseApiController
    {
        [HttpGet]
        [Route("api/dashboard/stats")]
        public IHttpActionResult GetStats()
        {
            // 1. Tính tổng doanh thu (Bảng ThanhToan hoặc DatTour)
            string sqlDoanhThu = "SELECT ISNULL(SUM(TongTien), 0) FROM DatTour WHERE TrangThai = N'Đã thanh toán'";
            decimal doanhThu = Convert.ToDecimal(ExecuteQuery(sqlDoanhThu).Rows[0][0]);

            // 2. Tổng lượt đặt
            string sqlLuotDat = "SELECT COUNT(*) FROM DatTour";
            int luotDat = Convert.ToInt32(ExecuteQuery(sqlLuotDat).Rows[0][0]);

            // 3. Khách hàng mới (Role = 'User')
            string sqlKhach = "SELECT COUNT(*) FROM NguoiDung WHERE VaiTro = N'User'";
            int khachMoi = Convert.ToInt32(ExecuteQuery(sqlKhach).Rows[0][0]);

            // 4. Tour đang hoạt động
            string sqlTour = "SELECT COUNT(*) FROM Tour WHERE TrangThai = N'Đang mở bán'";
            int tourActive = Convert.ToInt32(ExecuteQuery(sqlTour).Rows[0][0]);

            // 5. Top 4 Tour bán chạy nhất (Query phức tạp hơn xíu)
            string sqlTopTour = @"
                SELECT TOP 4 t.TenTour, t.HinhAnhDaiDien, COUNT(dt.MaDatTour) as SoLuotDat, SUM(dt.TongTien) as DoanhThu
                FROM Tour t
                LEFT JOIN DatTour dt ON t.MaTour = dt.MaTour
                WHERE dt.TrangThai = N'Đã thanh toán'
                GROUP BY t.TenTour, t.HinhAnhDaiDien
                ORDER BY SoLuotDat DESC";

            DataTable dtTop = ExecuteQuery(sqlTopTour);
            var topTours = new List<object>();
            foreach (DataRow row in dtTop.Rows)
            {
                topTours.Add(new
                {
                    TenTour = row["TenTour"].ToString(),
                    HinhAnh = row["HinhAnhDaiDien"].ToString(), // Nhớ update DB có link ảnh nhé
                    SoLuotDat = row["SoLuotDat"],
                    DoanhThu = row["DoanhThu"]
                });
            }

            // Trả về cục dữ liệu JSON tổng hợp
            return Ok(new
            {
                TongDoanhThu = doanhThu,
                TongLuotDat = luotDat,
                KhachHangMoi = khachMoi,
                TourHoatDong = tourActive,
                TopTours = topTours
            });
        }
    }
}