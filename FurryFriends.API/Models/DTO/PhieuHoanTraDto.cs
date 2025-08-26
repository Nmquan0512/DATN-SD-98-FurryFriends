namespace FurryFriends.API.Models.DTO
{
    public class PhieuHoanTraDto
    {
        public Guid PhieuHoanTraId { get; set; }
        public Guid HoaDonChiTietId { get; set; }
        public int SoLuongHoan { get; set; }
        public string LyDoHoanTra { get; set; }
        public DateTime NgayHoanTra { get; set; }
        public int TrangThai { get; set; }

        // ✅ Thông tin snapshot sản phẩm lúc mua
        public string TenSanPham { get; set; }
        public string MauSac { get; set; }
        public string KichCo { get; set; }
        public string AnhSanPham { get; set; }

        // ✅ Thông tin khách hàng gửi yêu cầu (mới thêm)
        public Guid KhachHangId { get; set; }
        public string HoTenKhach { get; set; }
        public string SdtKhach { get; set; }
        public string EmailKhach { get; set; }
        public string DiaChiNhanHang { get; set; }
        public int TongSoLuongMua { get; set; }    // từ HoaDonChiTiet.SoLuongSanPham
        public int SoLuongDaHoan { get; set; }     // tổng số lượng đã “giữ chỗ” hoàn (trạng thái != 2 Bị từ chối)

        // ✅ Tính sẵn cho tiện (không bắt buộc, nhưng hữu ích)
        public int SoLuongConLai => Math.Max(0, TongSoLuongMua - SoLuongDaHoan);
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
