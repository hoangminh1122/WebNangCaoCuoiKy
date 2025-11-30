using System;

namespace QUANLYDICHVUDULICH.ADMIN.Models
{
    public class TinTucViewModel
    {
        public int MaTin { get; set; }
        public string TieuDe { get; set; }
        public string DanhMuc { get; set; }
        public string TacGia { get; set; }
        public DateTime NgayDang { get; set; }
        public string TrangThai { get; set; }

        // --- BỔ SUNG 3 DÒNG NÀY VÀO LÀ HẾT LỖI ---
        public string TomTat { get; set; }
        public string NoiDung { get; set; }
        public string HinhAnh { get; set; }
        // ----------------------------------------
    }
}