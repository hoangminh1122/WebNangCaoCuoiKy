using System;

namespace QUANLYDICHVUDULICH.API.Models
{
    public class TinTucDTO
    {
        public int MaTin { get; set; }
        public string TieuDe { get; set; }
        public string TomTat { get; set; }
        public string NoiDung { get; set; }
        public string HinhAnh { get; set; }
        public string DanhMuc { get; set; } // Tương ứng cột LoaiTin trong DB
        public DateTime NgayDang { get; set; }
        public string TacGia { get; set; }  // Tên người đăng (Lấy từ bảng NguoiDung)
        public int MaND { get; set; }       // ID người đăng
        public string TrangThai { get; set; }
    }
}