using System;

namespace FurryFriends.API.Models.DTO.BanHang.Requests
{
    public class TaoHoaDonRequest
    {
        public Guid? KhachHangId { get; set; } // Null nếu là khách lẻ
        public bool LaKhachLe { get; set; } = true;
        public string GhiChu { get; set; }

        // Thêm trạng thái hóa đơn (tùy chọn)
        public bool GiaoHang { get; set; } = false; // Đánh dấu đây là đơn giao hàng
        public Guid? DiaChiGiaoHangId { get; set; }
        public int? TrangThai { get; set; } = 0; // 0: Chưa thanh toán, 1: Đã thanh toán
        public DiaChiMoiDto? DiaChiMoi { get; set; }
        public Guid NhanVienId { get; internal set; }
    }
    public class DiaChiMoiDto
    {
 
        public string TenNguoiNhan { get; set; }
     
        public string SoDienThoai { get; set; }
     
        public string ThanhPho { get; set; }

        public string PhuongXa { get; set; }
 
        public string TenDiaChi { get; set; } // Số nhà, tên đường...
    }
}