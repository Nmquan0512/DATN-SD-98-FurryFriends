using FurryFriends.API.Models;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using FurryFriends.API.Models.DTO.BanHang;
using FurryFriends.API.Models.BanHang.Requests;
namespace FurryFriends.API.Repository.IRepository
{
    public interface IBanHangRepository
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
