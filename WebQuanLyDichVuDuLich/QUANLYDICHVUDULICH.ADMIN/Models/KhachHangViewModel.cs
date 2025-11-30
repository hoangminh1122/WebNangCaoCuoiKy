using System;

namespace QUANLYDICHVUDULICH.ADMIN.Models
{
    public class KhachHangViewModel
    {
        public int MaND { get; set; }
        public string HoTen { get; set; }
        public string Email { get; set; }
        public string SoDienThoai { get; set; }
        public DateTime NgayTao { get; set; }
        public bool TrangThai { get; set; }
    }
}