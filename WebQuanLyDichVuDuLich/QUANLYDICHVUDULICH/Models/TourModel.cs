using System;

namespace QUANLYDICHVUDULICH.API.Models
{
    public class TourModel
    {
        public int MaTour { get; set; }
        public int MaDanhMuc { get; set; }
        public string TenTour { get; set; }
        public decimal GiaGoc { get; set; }
        public int ThoiLuongNgay { get; set; }
        public string TrangThai { get; set; }
        public string HinhAnhDaiDien { get; set; }
        public string MoTa { get; set; }
        public string LichTrinh { get; set; }
        public string DiemKhoiHanh { get; set; }
        public string DiemDen { get; set; }
        public string PhuongTien { get; set; }
    }
}