namespace FurryFriends.Web.ViewModels
{
    public class SanPhamChiTietViewModel
    {
        public Guid SanPhamChiTietId { get; set; }

        public string TenSanPham { get; set; } = "";
        public string MauSac { get; set; } = "";
        public string KichCo { get; set; } = "";
        public int SoLuongTon { get; set; }
        public decimal GiaBan { get; set; }
        public List<string> DanhSachAnh { get; set; } = new(); // Url ảnh để hiển thị

        // Thông tin giảm giá
        public decimal? PhanTramGiamGia { get; set; }
        public decimal? GiaSauGiam { get; set; }
        public bool CoGiamGia { get; set; } = false;

        public DateTime? NgayKetThucGiamGia { get; set; } // Ngày kết thúc giảm giá
        public bool TrangThai { get; set; } // Trạng thái sản phẩm (hoạt động/ngừng)
    }
}
