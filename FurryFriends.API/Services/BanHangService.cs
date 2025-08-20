using FurryFriends.API.Models.DTO.BanHang;
using FurryFriends.API.Models.DTO.BanHang.Requests;
using FurryFriends.API.Repository.IRepository;
using FurryFriends.API.Services.IServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
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
            try
            {
                _logger.LogInformation("Service: Bắt đầu thêm sản phẩm vào hóa đơn. Request: {@Request}", request);
                
                if (request == null) 
                {
                    _logger.LogError("Service: Request null");
                    throw new ArgumentNullException(nameof(request));
                }
                
                if (request.HoaDonId == Guid.Empty) 
                {
                    _logger.LogError("Service: HoaDonId empty");
                    throw new ArgumentException("ID hóa đơn không hợp lệ.", nameof(request.HoaDonId));
                }
                
                if (request.SanPhamChiTietId == Guid.Empty) 
                {
                    _logger.LogError("Service: SanPhamChiTietId empty");
                    throw new ArgumentException("ID sản phẩm chi tiết không hợp lệ.", nameof(request.SanPhamChiTietId));
                }
                
                if (request.SoLuong <= 0) 
                {
                    _logger.LogError("Service: SoLuong <= 0: {SoLuong}", request.SoLuong);
                    throw new ArgumentOutOfRangeException(nameof(request.SoLuong), "Số lượng phải lớn hơn 0.");
                }

                _logger.LogInformation("Service: Gọi repository với request hợp lệ");
                var result = await _banHangRepository.ThemSanPhamVaoHoaDonAsync(request);
                
                _logger.LogInformation("Service: Thêm sản phẩm thành công. Kết quả: {@Result}", result);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Service: Lỗi khi thêm sản phẩm {SanPhamChiTietId} vào hóa đơn {HoaDonId}", 
                    request?.SanPhamChiTietId, request?.HoaDonId);
                throw;
            }
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
            // Gọi hàm xóa sản phẩm riêng biệt
            return await _banHangRepository.XoaSanPhamKhoiHoaDonAsync(hoaDonId, sanPhamChiTietId);
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

        public async Task<object> ApDungVoucherGioHangAsync(Guid khachHangId, Guid voucherId)
        {
            if (khachHangId == Guid.Empty) throw new ArgumentException("ID khách hàng không hợp lệ.", nameof(khachHangId));
            if (voucherId == Guid.Empty) throw new ArgumentException("ID voucher không hợp lệ.", nameof(voucherId));

            _logger.LogInformation($"Áp dụng voucher GioHang '{voucherId}' cho khách hàng {khachHangId}.");
            
            // Sử dụng logic của GioHang để tính toán voucher
            var dto = new { KhachHangId = khachHangId, VoucherId = voucherId };
            
            // Gọi API của GioHang để tính toán
            using (var httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new Uri("https://localhost:7289/");
                var response = await httpClient.PostAsJsonAsync("/api/GioHang/ap-dung-voucher", dto);

                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    throw new Exception($"Không thể áp dụng voucher: {error}");
                }

                var responseBody = await response.Content.ReadAsStringAsync();
                return Newtonsoft.Json.JsonConvert.DeserializeObject(responseBody);
            }
        }

        public async Task<HoaDonBanHangDto> GoBoVoucherAsync(Guid hoaDonId)
        {
            if (hoaDonId == Guid.Empty) throw new ArgumentException("ID hóa đơn không hợp lệ.", nameof(hoaDonId));
            _logger.LogInformation($"Gỡ bỏ voucher khỏi hóa đơn {hoaDonId}.");
            return await _banHangRepository.GoBoVoucherAsync(hoaDonId);
        }

        public async Task<HoaDonBanHangDto> GanKhachHangAsync(Guid hoaDonId, Guid? khachHangId)
        {
            if (hoaDonId == Guid.Empty) throw new ArgumentException("ID hóa đơn không hợp lệ.", nameof(hoaDonId));
            
            if (khachHangId.HasValue && khachHangId.Value == Guid.Empty) 
                throw new ArgumentException("ID khách hàng không hợp lệ.", nameof(khachHangId));

            if (khachHangId.HasValue)
            {
                _logger.LogInformation($"Gán khách hàng {khachHangId.Value} cho hóa đơn {hoaDonId}.");
            }
            else
            {
                _logger.LogInformation($"Chuyển hóa đơn {hoaDonId} về khách lẻ.");
            }
            
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

        public async Task<IEnumerable<KhachHangDto>> TimKiemKhachHangAsync(string? keyword)
        {
            _logger.LogInformation($"Tìm kiếm khách hàng với từ khóa: '{keyword ?? "null"}'.");
            return await _banHangRepository.TimKiemKhachHangAsync(keyword);
        }

        public async Task<IEnumerable<VoucherDto>> TimKiemVoucherHopLeAsync(Guid hoaDonId)
        {
            if (hoaDonId == Guid.Empty) throw new ArgumentException("ID hóa đơn không hợp lệ.", nameof(hoaDonId));
            _logger.LogInformation($"Tìm kiếm voucher hợp lệ cho hóa đơn {hoaDonId}.");
            return await _banHangRepository.TimKiemVoucherHopLeAsync(hoaDonId);
        }

        public async Task<HoaDonBanHangDto> CapNhatDiaChiGiaoHangAsync(Guid hoaDonId, DiaChiMoiDto diaChiMoi)
        {
            if (diaChiMoi == null) throw new ArgumentNullException(nameof(diaChiMoi));
            if (string.IsNullOrWhiteSpace(diaChiMoi.TenNguoiNhan))
                throw new ArgumentException("Tên người nhận không được để trống.", nameof(diaChiMoi.TenNguoiNhan));

            _logger.LogInformation("Bắt đầu cập nhật địa chỉ giao hàng cho hóa đơn: {HoaDonId}", hoaDonId);
            return await _banHangRepository.CapNhatDiaChiGiaoHangAsync(hoaDonId, diaChiMoi);
        }
        #endregion

        public async Task<IEnumerable<SanPhamBanHangDto>> GetSuggestedProductsAsync(int count)
        {
            _logger.LogInformation($"Lấy {count} sản phẩm gợi ý.");
            if (count <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(count), "Số lượng sản phẩm gợi ý phải lớn hơn 0.");
            }
            return await _banHangRepository.GetSuggestedProductsAsync(count);
        }

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

        public async Task FixInvoiceDataAsync()
        {
            await _banHangRepository.FixInvoiceDataAsync();
        }
        #endregion
    }
}