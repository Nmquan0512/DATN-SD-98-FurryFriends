namespace FurryFriends.Web.ViewModels
{
    public class DonHangKhachHangViewModel
    {
        public Guid HoaDonId { get; set; }
        public DateTime NgayTao { get; set; }
        public DateTime? NgayNhanHang { get; set; }
        public decimal TongTien { get; set; }
        public decimal TongTienSauKhiGiam { get; set; }
        public int TrangThai { get; set; }
        public string TrangThaiText { get; set; } = "";
        public string GhiChu { get; set; } = "";
        public string HinhThucThanhToan { get; set; } = "";
        public string DiaChiGiaoHang { get; set; } = "";
        public List<DonHangChiTietViewModel> ChiTiets { get; set; } = new();
        
        // Voucher info (snapshot)
        public string? VoucherCode { get; set; }
        public decimal? VoucherDiscount { get; set; }
        public string? ThongTinVoucherLucMua { get; set; }
    }

    public class DonHangChiTietViewModel
    {
        public Guid HoaDonChiTietId { get; set; }
        
        // ✅ Snapshot data - thông tin lúc mua
        public string TenSanPham { get; set; } = "";
        public string AnhSanPham { get; set; } = "";
        public string MauSac { get; set; } = "";
        public string KichCo { get; set; } = "";
        public string ThuongHieu { get; set; } = "";
        public string MoTa { get; set; } = "";
        
        // ✅ Thêm các thuộc tính snapshot từ HoaDonChiTiet
        public string? TenSanPhamLucMua { get; set; }
        public string? AnhSanPhamLucMua { get; set; }
        public string? MauSacLucMua { get; set; }
        public string? KichCoLucMua { get; set; }
        public string? ThuongHieuLucMua { get; set; }
        public string? MoTaSanPhamLucMua { get; set; }
        public decimal? GiaLucMua { get; set; }
        
        // Số lượng và giá
        public int SoLuong { get; set; }
        public decimal Gia { get; set; }
        public decimal ThanhTien { get; set; }
        
        // Thông tin giá gốc và giảm giá (để hiển thị)
        public decimal? GiaGoc { get; set; }
        public decimal? PhanTramGiam { get; set; }
    }

    public enum TrangThaiDonHang
    {
        ChoDuyet = 0,
        DaDuyet = 1,
        DangGiao = 2,
        DaGiao = 3,
        DaHuy = 4
    }
}
