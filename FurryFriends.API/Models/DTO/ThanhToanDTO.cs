namespace FurryFriends.API.Models.DTO
{
    public class ThanhToanDTO
    {
        public Guid KhachHangId { get; set; }
        public Guid? VoucherId { get; set; }
        public Guid TaiKhoanId { get; set; }
        public Guid HinhThucThanhToanId { get; set; }
        public Guid? NhanVienId { get; set; }

        public string? TenCuaKhachHang { get; set; }
        public string? SdtCuaKhachHang { get; set; }
        public string? EmailCuaKhachHang { get; set; }

        public string? LoaiHoaDon { get; set; }
        public string? GhiChu { get; set; }
    }
}
