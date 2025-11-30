using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QUANLYDICHVUDULICH.ADMIN.Models
{
    public class TourViewModel
    {
        public int MaTour { get; set; }
        public string TenTour { get; set; }
        public int MaDanhMuc { get; set; } // Khóa ngoại
        public decimal GiaGoc { get; set; }
        public int ThoiLuongNgay { get; set; }
        public string TrangThai { get; set; }
        public string HinhAnhDaiDien { get; set; }
    }

}