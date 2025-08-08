using FurryFriends.API.Models.DTO.BanHang;
using FurryFriends.API.Models.DTO.BanHang.Requests;
using FurryFriends.API.Repository.IRepository;
using FurryFriends.API.Services.IServices;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace FurryFriends.API.Services
{
    public class BanHangService : IBanHangService
    {
        private readonly IBanHangRepository _banHangRepository;
        private readonly ILogger<BanHangService> _logger;

        public BanHangService(IBanHangRepository banHangRepository, ILogger<BanHangService> logger)
        {
            _banHangRepository = banHangRepository;
            _logger = logger;
        }

        #region Hóa Đơn
        public async Task<IEnumerable<HoaDonBanHangDto>> GetAllHoaDonsAsync()
        {
            _logger.LogInformation("Bắt đầu lấy danh sách tất cả hóa đơn.");
            return await _banHangRepository.GetAllHoaDonsAsync();
        }

        public async Task<HoaDonBanHangDto> GetHoaDonByIdAsync(Guid id)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("ID hóa đơn không hợp lệ.", nameof(id));
            _logger.LogInformation($"Bắt đầu lấy hóa đơn ID: {id}.");
            return await _banHangRepository.GetHoaDonByIdAsync(id);
        }

        public async Task<HoaDonBanHangDto> TaoHoaDonAsync(TaoHoaDonRequest request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));
            if (!request.LaKhachLe && !request.KhachHangId.HasValue)
                throw new ArgumentException("Phải cung cấp ID khách hàng hoặc đánh dấu là khách lẻ.", nameof(request.KhachHangId));

            _logger.LogInformation("Bắt đầu tạo hóa đơn mới.");
            return await _banHangRepository.TaoHoaDonAsync(request);
        }

        public async Task<HoaDonBanHangDto> HuyHoaDonAsync(Guid hoaDonId)
        {
            if (hoaDonId == Guid.Empty)
                throw new ArgumentException("ID hóa đơn không hợp lệ.", nameof(hoaDonId));

            _logger.LogWarning($"Yêu cầu hủy hóa đơn ID: {hoaDonId}.");
            return await _banHangRepository.HuyHoaDonAsync(hoaDonId);
        }
        #endregion

        #region Quản lý sản phẩm
        public async Task<HoaDonBanHangDto> ThemSanPhamVaoHoaDonAsync(ThemSanPhamVaoHoaDonRequest request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));
            if (request.HoaDonId == Guid.Empty) throw new ArgumentException("ID hóa đơn không hợp lệ.", nameof(request.HoaDonId));
            if (request.SanPhamChiTietId == Guid.Empty) throw new ArgumentException("ID sản phẩm chi tiết không hợp lệ.", nameof(request.SanPhamChiTietId));
            if (request.SoLuong <= 0) throw new ArgumentOutOfRangeException(nameof(request.SoLuong), "Số lượng phải lớn hơn 0.");

            _logger.LogInformation($"Thêm {request.SoLuong} sản phẩm {request.SanPhamChiTietId} vào hóa đơn {request.HoaDonId}.");
            return await _banHangRepository.ThemSanPhamVaoHoaDonAsync(request);
        }

        public async Task<HoaDonBanHangDto> CapNhatSoLuongSanPhamAsync(Guid hoaDonId, Guid sanPhamChiTietId, int soLuongMoi)
        {
            if (hoaDonId == Guid.Empty) throw new ArgumentException("ID hóa đơn không hợp lệ.", nameof(hoaDonId));
            if (sanPhamChiTietId == Guid.Empty) throw new ArgumentException("ID sản phẩm chi tiết không hợp lệ.", nameof(sanPhamChiTietId));
            if (soLuongMoi < 0) throw new ArgumentOutOfRangeException(nameof(soLuongMoi), "Số lượng không được âm.");

            _logger.LogInformation($"Cập nhật số lượng sản phẩm {sanPhamChiTietId} trong hóa đơn {hoaDonId} thành {soLuongMoi}.");
            return await _banHangRepository.CapNhatSoLuongSanPhamAsync(hoaDonId, sanPhamChiTietId, soLuongMoi);
        }

        public async Task<HoaDonBanHangDto> XoaSanPhamKhoiHoaDonAsync(Guid hoaDonId, Guid sanPhamChiTietId)
        {
            _logger.LogInformation($"Xóa sản phẩm {sanPhamChiTietId} khỏi hóa đơn {hoaDonId}.");
            // Xóa thực chất là cập nhật số lượng về 0
            return await _banHangRepository.CapNhatSoLuongSanPhamAsync(hoaDonId, sanPhamChiTietId, 0);
        }
        #endregion

        #region Voucher & Khách hàng
        public async Task<HoaDonBanHangDto> ApDungVoucherAsync(Guid hoaDonId, string maVoucher)
        {
            if (hoaDonId == Guid.Empty) throw new ArgumentException("ID hóa đơn không hợp lệ.", nameof(hoaDonId));
            if (string.IsNullOrWhiteSpace(maVoucher)) throw new ArgumentException("Mã voucher không được để trống.", nameof(maVoucher));

            _logger.LogInformation($"Áp dụng voucher '{maVoucher}' cho hóa đơn {hoaDonId}.");
            return await _banHangRepository.ApDungVoucherAsync(hoaDonId, maVoucher);
        }

        public async Task<HoaDonBanHangDto> GoBoVoucherAsync(Guid hoaDonId)
        {
            if (hoaDonId == Guid.Empty) throw new ArgumentException("ID hóa đơn không hợp lệ.", nameof(hoaDonId));
            _logger.LogInformation($"Gỡ bỏ voucher khỏi hóa đơn {hoaDonId}.");
            return await _banHangRepository.GoBoVoucherAsync(hoaDonId);
        }

        public async Task<HoaDonBanHangDto> GanKhachHangAsync(Guid hoaDonId, Guid khachHangId)
        {
            if (hoaDonId == Guid.Empty) throw new ArgumentException("ID hóa đơn không hợp lệ.", nameof(hoaDonId));
            if (khachHangId == Guid.Empty) throw new ArgumentException("ID khách hàng không hợp lệ.", nameof(khachHangId));

            _logger.LogInformation($"Gán khách hàng {khachHangId} cho hóa đơn {hoaDonId}.");
            return await _banHangRepository.GanKhachHangAsync(hoaDonId, khachHangId);
        }
        #endregion

        #region Thanh toán
        public async Task<HoaDonBanHangDto> ThanhToanHoaDonAsync(ThanhToanRequest request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));
            if (request.HoaDonId == Guid.Empty) throw new ArgumentException("ID hóa đơn không hợp lệ.", nameof(request.HoaDonId));
            if (request.HinhThucThanhToanId == Guid.Empty) throw new ArgumentException("Phải chọn hình thức thanh toán.", nameof(request.HinhThucThanhToanId));

            _logger.LogInformation($"Yêu cầu thanh toán cho hóa đơn {request.HoaDonId}.");
            return await _banHangRepository.ThanhToanHoaDonAsync(request);
        }
        #endregion

        #region Tìm kiếm
        public async Task<IEnumerable<SanPhamBanHangDto>> TimKiemSanPhamAsync(string keyword)
        {
            // Cho phép tìm kiếm với keyword rỗng để lấy danh sách mặc định
            _logger.LogInformation($"Tìm kiếm sản phẩm với từ khóa: '{keyword}'.");
            return await _banHangRepository.TimKiemSanPhamAsync(keyword);
        }

        public async Task<IEnumerable<KhachHangDto>> TimKiemKhachHangAsync(string keyword)
        {
            _logger.LogInformation($"Tìm kiếm khách hàng với từ khóa: '{keyword}'.");
            return await _banHangRepository.TimKiemKhachHangAsync(keyword);
        }

        public async Task<IEnumerable<VoucherDto>> TimKiemVoucherHopLeAsync(Guid hoaDonId)
        {
            if (hoaDonId == Guid.Empty) throw new ArgumentException("ID hóa đơn không hợp lệ.", nameof(hoaDonId));
            _logger.LogInformation($"Tìm kiếm voucher hợp lệ cho hóa đơn {hoaDonId}.");
            return await _banHangRepository.TimKiemVoucherHopLeAsync(hoaDonId);
        }
        #endregion

        #region Khách hàng
        public async Task<KhachHangDto> TaoKhachHangMoiAsync(TaoKhachHangRequest request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));
            var validationContext = new ValidationContext(request);
            var validationResults = new List<ValidationResult>();
            if (!Validator.TryValidateObject(request, validationContext, validationResults, true))
            {
                throw new ValidationException(validationResults.First().ErrorMessage);
            }

            _logger.LogInformation($"Tạo khách hàng mới với SĐT: {request.SDT}.");
            return await _banHangRepository.TaoKhachHangMoiAsync(request);
        }
        #endregion
    }
}