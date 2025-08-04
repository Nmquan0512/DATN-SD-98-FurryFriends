using FurryFriends.API.Models.BanHang.Requests;
using FurryFriends.API.Models.DTO.BanHang;
using FurryFriends.API.Repository.IRepository;
using FurryFriends.API.Services.IServices;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FurryFriends.API.Services
{
    public class BanHangService : IBanHangService
    {
        private readonly IBanHangRepository _banHangRepository;

        public BanHangService(IBanHangRepository banHangRepository)
        {
            _banHangRepository = banHangRepository;
        }

        public async Task<HoaDonBanHangDto> TaoHoaDon(TaoHoaDonRequest request)
        {
            ValidateTaoHoaDonRequest(request);
            return await _banHangRepository.TaoHoaDon(request);
        }

        public async Task<HoaDonBanHangDto> ThemSanPhamVaoHoaDon(ThemSanPhamVaoHoaDonRequest request)
        {
            ValidateThemSanPhamRequest(request);
            return await _banHangRepository.ThemSanPhamVaoHoaDon(request);
        }

        public async Task<HoaDonBanHangDto> XoaSanPhamKhoiHoaDon(Guid hoaDonId, Guid sanPhamChiTietId)
        {
            if (hoaDonId == Guid.Empty || sanPhamChiTietId == Guid.Empty)
                throw new ArgumentException("ID không hợp lệ");

            return await _banHangRepository.XoaSanPhamKhoiHoaDon(hoaDonId, sanPhamChiTietId);
        }

        public async Task<HoaDonBanHangDto> CapNhatSoLuongSanPham(Guid hoaDonId, Guid sanPhamChiTietId, int soLuong)
        {
            if (hoaDonId == Guid.Empty || sanPhamChiTietId == Guid.Empty)
                throw new ArgumentException("ID không hợp lệ");

            if (soLuong <= 0)
                throw new ArgumentException("Số lượng phải lớn hơn 0");

            return await _banHangRepository.CapNhatSoLuongSanPham(hoaDonId, sanPhamChiTietId, soLuong);
        }

        public async Task<HoaDonBanHangDto> ApDungVoucher(Guid hoaDonId, Guid voucherId)
        {
            if (hoaDonId == Guid.Empty || voucherId == Guid.Empty)
                throw new ArgumentException("ID không hợp lệ");

            return await _banHangRepository.ApDungVoucher(hoaDonId, voucherId);
        }

        public async Task<HoaDonBanHangDto> ThanhToan(ThanhToanRequest request)
        {
            ValidateThanhToanRequest(request);
            return await _banHangRepository.ThanhToan(request);
        }

        public async Task<HoaDonBanHangDto> GetHoaDonById(Guid id)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("ID không hợp lệ");

            return await _banHangRepository.GetHoaDonById(id);
        }

        public async Task<IEnumerable<SanPhamBanHangDto>> TimKiemSanPham(string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
                throw new ArgumentException("Từ khóa tìm kiếm không được để trống");

            return await _banHangRepository.TimKiemSanPham(keyword);
        }

        public async Task<IEnumerable<KhachHangDto>> TimKiemKhachHang(string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
                throw new ArgumentException("Từ khóa tìm kiếm không được để trống");

            return await _banHangRepository.TimKiemKhachHang(keyword);
        }

        public async Task<KhachHangDto> TaoKhachHang(TaoKhachHangRequest request)
        {
            ValidateTaoKhachHangRequest(request);
            return await _banHangRepository.TaoKhachHang(request);
        }

        #region Private Methods
        private void ValidateTaoHoaDonRequest(TaoHoaDonRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            if (!request.LaKhachLe && !request.KhachHangId.HasValue)
                throw new ArgumentException("Phải chọn khách hàng hoặc chọn khách lẻ");
        }

        private void ValidateThemSanPhamRequest(ThemSanPhamVaoHoaDonRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            if (request.HoaDonId == Guid.Empty)
                throw new ArgumentException("ID hóa đơn không hợp lệ");

            if (request.SanPhamChiTietId == Guid.Empty)
                throw new ArgumentException("ID sản phẩm không hợp lệ");

            if (request.SoLuong <= 0)
                throw new ArgumentException("Số lượng phải lớn hơn 0");
        }

        private void ValidateThanhToanRequest(ThanhToanRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            if (request.HoaDonId == Guid.Empty)
                throw new ArgumentException("ID hóa đơn không hợp lệ");

            if (request.HinhThucThanhToanId == Guid.Empty)
                throw new ArgumentException("Hình thức thanh toán không hợp lệ");
        }

        private void ValidateTaoKhachHangRequest(TaoKhachHangRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            if (string.IsNullOrWhiteSpace(request.TenKhachHang))
                throw new ArgumentException("Tên khách hàng không được để trống");

            if (string.IsNullOrWhiteSpace(request.SDT))
                throw new ArgumentException("Số điện thoại không được để trống");

            if (request.SDT.Length < 10 || request.SDT.Length > 15)
                throw new ArgumentException("Số điện thoại không hợp lệ");
        }
        #endregion
    }
}