using System.Collections.Generic;

namespace QUANLYDICHVUDULICH.Admin.Models
{
    public class DashboardViewModel
    {
        public decimal TongDoanhThu { get; set; }
        public int TongLuotDat { get; set; }
        public int KhachHangMoi { get; set; }
        public int TourHoatDong { get; set; }
        public List<TopTourModel> TopTours { get; set; }
    }

    public class TopTourModel
    {
        public string TenTour { get; set; }
        public string HinhAnh { get; set; }
        public int SoLuotDat { get; set; }
        public decimal DoanhThu { get; set; }
    }
}