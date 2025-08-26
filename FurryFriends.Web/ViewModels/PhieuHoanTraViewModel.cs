namespace FurryFriends.Web.ViewModels
{
    public class PhieuHoanTraViewModel
    {
        public Guid PhieuHoanTraId { get; set; }
        public Guid HoaDonChiTietId { get; set; }
        public int SoLuongHoan { get; set; }
        public string LyDoHoanTra { get; set; }
        public DateTime NgayHoanTra { get; set; }
        public int TrangThai { get; set; }
        public int TongSoLuongMua { get; set; }      // tổng số lượng sản phẩm KH đã mua (trong CTHĐ)
        public int SoLuongDaHoan { get; set; }       // tổng số lượng đã yêu cầu/hoàn thành
        public int SoLuongConLai => TongSoLuongMua - SoLuongDaHoan;


        // Sản phẩm
        public string TenSanPham { get; set; }
        public string MauSac { get; set; }
        public string KichCo { get; set; }
        public string AnhSanPham { get; set; }

        // ✅ Khách hàng gửi yêu cầu
        public Guid KhachHangId { get; set; }
        public string HoTenKhach { get; set; }
        public string SdtKhach { get; set; }
        public string EmailKhach { get; set; }
        public string DiaChiNhanHang { get; set; } // nếu API trả snapshot địa chỉ lúc mua
    }
    public class PhieuHoanTraCreateRequest
    {
        public Guid HoaDonChiTietId { get; set; }
        public int SoLuongHoan { get; set; }
        public string LyDoHoanTra { get; set; }
    }

    public class PhieuHoanTraUpdateRequest
    {
        public int SoLuongHoan { get; set; }
        public string LyDoHoanTra { get; set; }
        public int TrangThai { get; set; }
    }
}
