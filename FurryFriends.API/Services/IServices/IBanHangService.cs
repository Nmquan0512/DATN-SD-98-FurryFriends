using FurryFriends.API.Models.DTO.BanHang;
using FurryFriends.API.Models.DTO.BanHang.Requests;

namespace FurryFriends.API.Services.IServices
{
    public interface IBanHangService
    {
        Task<HoaDonBanHangDto> TaoHoaDon(TaoHoaDonRequest request);
        Task<HoaDonBanHangDto> ThemSanPhamVaoHoaDon(ThemSanPhamVaoHoaDonRequest request);
        Task<HoaDonBanHangDto> XoaSanPhamKhoiHoaDon(Guid hoaDonId, Guid sanPhamChiTietId);
        Task<HoaDonBanHangDto> CapNhatSoLuongSanPham(Guid hoaDonId, Guid sanPhamChiTietId, int soLuong);
        Task<HoaDonBanHangDto> ApDungVoucher(Guid hoaDonId, Guid voucherId);
        Task<HoaDonBanHangDto> ThanhToan(ThanhToanRequest request);
        Task<HoaDonBanHangDto> GetHoaDonById(Guid id);
        Task<IEnumerable<SanPhamBanHangDto>> TimKiemSanPham(string keyword);
        Task<IEnumerable<KhachHangDto>> TimKiemKhachHang(string keyword);
        Task<KhachHangDto> TaoKhachHang(TaoKhachHangRequest request);
    }
}