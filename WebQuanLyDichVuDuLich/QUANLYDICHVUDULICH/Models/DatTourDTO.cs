using System;

namespace QUANLYDICHVUDULICH.API.Models
{
    public class DatTourDTO
    {
        public int MaDatTour { get; set; }
        public string TenKhachHang { get; set; }
        public string TenTour { get; set; }
        public DateTime NgayKhoiHanh { get; set; }
        public decimal TongTien { get; set; }
        public string TrangThai { get; set; }
    }
}