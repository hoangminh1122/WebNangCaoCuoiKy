using System;

namespace QUANLYDICHVUDULICH.ADMIN.Models
{
    public class DatTourViewModel
    {
        public int MaDatTour { get; set; }
        public string TenKhachHang { get; set; } // Tên người đặt
        public string TenTour { get; set; }      // Tên tour họ chọn
        public DateTime NgayKhoiHanh { get; set; } // Ngày đi
        public decimal TongTien { get; set; }
        public string TrangThai { get; set; }    // Chờ xác nhận, Đã xác nhận, Hủy...
    }
}