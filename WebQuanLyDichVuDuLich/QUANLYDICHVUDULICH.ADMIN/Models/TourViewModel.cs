using System;

namespace QUANLYDICHVUDULICH.ADMIN.Models // Nhớ đổi namespace tương ứng cho từng Project
{
    public class TourViewModel
    {
        public int MaTour { get; set; }
        public string TenTour { get; set; }
        public int MaDanhMuc { get; set; }

        // --- CÁC TRƯỜNG BỔ SUNG CHO ĐỦ SQL ---
        public string MoTa { get; set; }
        public string LichTrinh { get; set; }
        public int ThoiLuongNgay { get; set; }
        public string DiemKhoiHanh { get; set; }
        public string DiemDen { get; set; }
        public string PhuongTien { get; set; }
        public decimal GiaGoc { get; set; }
        public decimal? GiaKhuyenMai { get; set; }
        public DateTime? NgayBatDau { get; set; }
        public DateTime? NgayKetThuc { get; set; }
        public int? SoNguoiToiDa { get; set; }
        public int? SoChoConLai { get; set; }
        public string HinhAnhDaiDien { get; set; }
        public string TrangThai { get; set; }
    }
}