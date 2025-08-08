using FurryFriends.API.Models.DTO.BanHang;
using FurryFriends.API.Models.DTO.BanHang.Requests;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FurryFriends.API.Repository.IRepository
{
    public interface IBanHangRepository
    {
        // Hóa đơn
        Task<IEnumerable<HoaDonBanHangDto>> GetAllHoaDonsAsync();
        Task<HoaDonBanHangDto> GetHoaDonByIdAsync(Guid id);
        Task<HoaDonBanHangDto> TaoHoaDonAsync(TaoHoaDonRequest request);
        Task<HoaDonBanHangDto> HuyHoaDonAsync(Guid hoaDonId); // Quan trọng: Để hủy đơn sai

        // Quản lý sản phẩm trong hóa đơn
        Task<HoaDonBanHangDto> ThemSanPhamVaoHoaDonAsync(ThemSanPhamVaoHoaDonRequest request);
        Task<HoaDonBanHangDto> XoaSanPhamKhoiHoaDonAsync(Guid hoaDonId, Guid sanPhamChiTietId);
        Task<HoaDonBanHangDto> CapNhatSoLuongSanPhamAsync(Guid hoaDonId, Guid sanPhamChiTietId, int soLuongMoi);

        // Voucher & Khách hàng
        Task<HoaDonBanHangDto> ApDungVoucherAsync(Guid hoaDonId, string maVoucher);
        Task<HoaDonBanHangDto> GoBoVoucherAsync(Guid hoaDonId);
        Task<HoaDonBanHangDto> GanKhachHangAsync(Guid hoaDonId, Guid khachHangId);

        // Thanh toán
        Task<HoaDonBanHangDto> ThanhToanHoaDonAsync(ThanhToanRequest request);

        // Tìm kiếm
        Task<IEnumerable<SanPhamBanHangDto>> TimKiemSanPhamAsync(string keyword);
        Task<IEnumerable<KhachHangDto>> TimKiemKhachHangAsync(string keyword);
        Task<IEnumerable<VoucherDto>> TimKiemVoucherHopLeAsync(Guid hoaDonId); // Tìm voucher áp dụng được cho hóa đơn

        // Khách hàng
        Task<KhachHangDto> TaoKhachHangMoiAsync(TaoKhachHangRequest request);
    }
}