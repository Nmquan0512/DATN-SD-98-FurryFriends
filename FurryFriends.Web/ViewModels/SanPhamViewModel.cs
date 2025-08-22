namespace FurryFriends.Web.ViewModels
{
    public class SanPhamViewModel
    {
        public Guid SanPhamChiTietId { get; set; } //sửa ơ đây thêm guid cho sp chi tiết
        public Guid SanPhamId { get; set; }
        public string TenSanPham { get; set; } = "";
        public string MoTa { get; set; } = "";
        public bool TrangThai { get; set; }
        public decimal GiaBan { get; set; }
        public int SoLuongTon { get; set; }

        public string? AnhDaiDienUrl { get; set; } // ảnh đầu tiên của chi tiết

        // Thông tin thương hiệu
        public string? TenThuongHieu { get; set; }
        public Guid? ThuongHieuId { get; set; }

        // Thông tin giảm giá
        public decimal? PhanTramGiamGia { get; set; }
        public decimal? GiaSauGiam { get; set; }
        public bool CoGiamGia { get; set; } = false;
        public DateTime? NgayKetThucGiamGia { get; set; } // Thêm ngày kết thúc giảm giá

        public List<SanPhamChiTietViewModel> ChiTietList { get; set; } = new();
    }
}
