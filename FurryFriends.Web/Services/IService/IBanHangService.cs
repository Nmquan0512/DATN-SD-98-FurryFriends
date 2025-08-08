using FurryFriends.API.Models.DTO.BanHang;
using FurryFriends.API.Models.DTO.BanHang.Requests;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FurryFriends.Web.Services.IService
{
    public interface IBanHangService
    {
        // Hóa Đơn
        Task<IEnumerable<HoaDonBanHangDto>> GetAllHoaDonsAsync();
        Task<HoaDonBanHangDto> GetHoaDonByIdAsync(Guid hoaDonId);
        Task<HoaDonBanHangDto> TaoHoaDonAsync(TaoHoaDonRequest request);
        Task<HoaDonBanHangDto> HuyHoaDonAsync(Guid hoaDonId);

        // Quản lý Chi tiết Hóa đơn (Items)
        Task<HoaDonBanHangDto> ThemSanPhamVaoHoaDonAsync(Guid hoaDonId, ThemSanPhamRequest request);
        Task<HoaDonBanHangDto> CapNhatSoLuongSanPhamAsync(Guid hoaDonId, Guid sanPhamChiTietId, CapNhatSoLuongRequest request);
        Task<HoaDonBanHangDto> XoaSanPhamKhoiHoaDonAsync(Guid hoaDonId, Guid sanPhamChiTietId);

        // Voucher & Khách hàng
        Task<HoaDonBanHangDto> GanKhachHangAsync(Guid hoaDonId, GanKhachHangRequest request);
        Task<HoaDonBanHangDto> ApDungVoucherAsync(Guid hoaDonId, ApDungVoucherRequest request);
        Task<HoaDonBanHangDto> GoBoVoucherAsync(Guid hoaDonId);

        // Thanh toán
        Task<HoaDonBanHangDto> ThanhToanHoaDonAsync(Guid hoaDonId, ThanhToanRequest request);

        // Tìm kiếm
        Task<IEnumerable<SanPhamBanHangDto>> TimKiemSanPhamAsync(string keyword);
        Task<IEnumerable<KhachHangDto>> TimKiemKhachHangAsync(string keyword);
        Task<IEnumerable<VoucherDto>> TimKiemVoucherHopLeAsync(Guid hoaDonId);

        // Khách hàng
        Task<KhachHangDto> TaoKhachHangMoiAsync(TaoKhachHangRequest request);
    }
}