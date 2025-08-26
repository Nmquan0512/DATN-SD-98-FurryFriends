using FurryFriends.API.Models.DTO.BanHang;
using FurryFriends.API.Models.DTO.BanHang.Requests;
using FurryFriends.API.Services.IServices;
using FurryFriends.API.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Linq;

namespace FurryFriends.API.Controllers
{
    /// <summary>
    /// API quản lý các nghiệp vụ của chức năng bán hàng.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
   /* [Authorize]*/ // Yêu cầu người dùng phải đăng nhập để sử dụng các API này
    public class BanHangController : ControllerBase
    {
        private readonly IBanHangService _banHangService;
        private readonly ILogger<BanHangController> _logger;
        private readonly AppDbContext _context;

        public BanHangController(IBanHangService banHangService, ILogger<BanHangController> logger, AppDbContext context)
        {
            _banHangService = banHangService;
            _logger = logger;
            _context = context;
        }

        #region Hóa Đơn (Hành động chính)

        /// <summary>
        /// Lấy danh sách tất cả hóa đơn (sử dụng cho trang lịch sử).
        /// </summary>
        /// <response code="200">Trả về danh sách hóa đơn.</response>
        [HttpGet("hoa-don")]
        [ProducesResponseType(typeof(IEnumerable<HoaDonBanHangDto>), 200)]
        public async Task<IActionResult> GetAllHoaDons()
        {
            var result = await _banHangService.GetAllHoaDonsAsync();
            return Ok(result);
        }

        /// <summary>
        /// Lấy chi tiết một hóa đơn bằng ID.
        /// </summary>
        /// <param name="hoaDonId">ID của hóa đơn cần lấy.</param>
        /// <response code="200">Trả về chi tiết hóa đơn.</response>
        /// <response code="404">Không tìm thấy hóa đơn với ID đã cho.</response>
        [HttpGet("hoa-don/{hoaDonId}")]
        [ProducesResponseType(typeof(HoaDonBanHangDto), 200)]
        [ProducesResponseType(typeof(string), 404)]
        public async Task<IActionResult> GetHoaDonById(Guid hoaDonId)
        {
            try
            {
                var result = await _banHangService.GetHoaDonByIdAsync(hoaDonId);
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        /// <summary>
        /// Tạo một hóa đơn mới (hóa đơn chờ).
        /// </summary>
        /// <remarks>
        /// ID Nhân viên sẽ được tự động lấy từ token của người dùng đang đăng nhập.
        /// </remarks>
        /// <param name="request">Thông tin để tạo hóa đơn.</param>
        /// <response code="201">Tạo thành công và trả về hóa đơn mới.</response>
        /// <response code="400">Dữ liệu đầu vào không hợp lệ.</response>
        [HttpPost("hoa-don")]
        [ProducesResponseType(typeof(HoaDonBanHangDto), 201)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<IActionResult> TaoHoaDon([FromBody] TaoHoaDonRequest request)
        {
            try
            {
                _logger.LogInformation("API: Bắt đầu tạo hóa đơn mới. Request: {@Request}", request);
                
                if (request == null)
                {
                    _logger.LogWarning("API: Request tạo hóa đơn null");
                    return BadRequest("Dữ liệu yêu cầu không hợp lệ.");
                }
                
                // Toàn bộ logic kiểm tra token và nhân viên đã được XÓA BỎ
                var result = await _banHangService.TaoHoaDonAsync(request);
                
                _logger.LogInformation("API: Tạo hóa đơn thành công. HoaDonId: {HoaDonId}", result.HoaDonId);
                
                return CreatedAtAction(nameof(GetHoaDonById), new { hoaDonId = result.HoaDonId }, result);
            }
            catch (ArgumentException ex) 
            { 
                _logger.LogWarning("API: ArgumentException khi tạo hóa đơn: {Message}", ex.Message);
                return BadRequest(ex.Message); 
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "API: Lỗi không xác định khi tạo hóa đơn");
                return StatusCode(500, $"Lỗi nội bộ server khi tạo hóa đơn: {ex.Message}");
            }
        }
        /// <summary>
        /// Hủy một hóa đơn đang ở trạng thái chờ.
        /// </summary>
        /// <param name="hoaDonId">ID của hóa đơn cần hủy.</param>
        /// <response code="200">Hủy thành công, trả về hóa đơn đã được cập nhật trạng thái.</response>
        /// <response code="400">Hóa đơn không thể hủy (đã thanh toán hoặc đã hủy trước đó).</response>
        /// <response code="404">Không tìm thấy hóa đơn.</response>
        [HttpPost("hoa-don/{hoaDonId}/huy")]
        [ProducesResponseType(typeof(HoaDonBanHangDto), 200)]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(typeof(string), 404)]
        public async Task<IActionResult> HuyHoaDon(Guid hoaDonId)
        {
            try
            {
                var result = await _banHangService.HuyHoaDonAsync(hoaDonId);
                return Ok(result);
            }
            catch (KeyNotFoundException ex) { return NotFound(ex.Message); }
            catch (InvalidOperationException ex) { return BadRequest(ex.Message); }
        }

        #endregion

        #region Quản lý Chi tiết Hóa đơn (Items)

        /// <summary>
        /// Thêm sản phẩm vào hóa đơn.
        /// </summary>
        /// <param name="hoaDonId">ID của hóa đơn cần thêm sản phẩm vào.</param>
        /// <param name="request">Thông tin sản phẩm và số lượng cần thêm.</param>
        [HttpPost("hoa-don/{hoaDonId}/items")]
        [ProducesResponseType(typeof(HoaDonBanHangDto), 200)]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(typeof(string), 404)]
        public async Task<IActionResult> ThemSanPhamVaoHoaDon(Guid hoaDonId, [FromBody] ThemSanPhamRequest request)
        {
            try
            {
                _logger.LogInformation("API: Bắt đầu thêm sản phẩm vào hóa đơn. HoaDonId: {HoaDonId}, Request: {@Request}", hoaDonId, request);
                
                if (request == null)
                {
                    _logger.LogWarning("API: Request null");
                    return BadRequest("Dữ liệu yêu cầu không hợp lệ.");
                }

                if (request.SanPhamChiTietId == Guid.Empty)
                {
                    _logger.LogWarning("API: SanPhamChiTietId empty");
                    return BadRequest("ID sản phẩm chi tiết không hợp lệ.");
                }

                if (request.SoLuong <= 0)
                {
                    _logger.LogWarning("API: SoLuong <= 0: {SoLuong}", request.SoLuong);
                    return BadRequest("Số lượng phải lớn hơn 0.");
                }

                // Kiểm tra xem hóa đơn có tồn tại không
                try
                {
                    var existingHoaDon = await _banHangService.GetHoaDonByIdAsync(hoaDonId);
                    _logger.LogInformation("API: Hóa đơn tồn tại: {@HoaDon}", existingHoaDon);
                }
                catch (KeyNotFoundException ex)
                {
                    _logger.LogError("API: Hóa đơn không tồn tại: {HoaDonId}, Error: {Message}", hoaDonId, ex.Message);
                    return NotFound($"Hóa đơn với ID {hoaDonId} không tồn tại.");
                }

                var fullRequest = new ThemSanPhamVaoHoaDonRequest
                {
                    HoaDonId = hoaDonId,
                    SanPhamChiTietId = request.SanPhamChiTietId,
                    SoLuong = request.SoLuong
                };
                
                _logger.LogInformation("API: Gọi service với fullRequest: {@FullRequest}", fullRequest);
                
                var result = await _banHangService.ThemSanPhamVaoHoaDonAsync(fullRequest);
                
                _logger.LogInformation("API: Thêm sản phẩm thành công. Kết quả: {@Result}", result);
                
                return Ok(result);
            }
            catch (KeyNotFoundException ex) 
            { 
                _logger.LogWarning("API: KeyNotFoundException: {Message}", ex.Message);
                return NotFound(ex.Message); 
            }
            catch (InvalidOperationException ex) 
            { 
                _logger.LogWarning("API: InvalidOperationException: {Message}", ex.Message);
                return BadRequest(ex.Message); 
            }
            catch (ArgumentException ex) 
            { 
                _logger.LogWarning("API: ArgumentException: {Message}", ex.Message);
                return BadRequest(ex.Message); 
            }
            catch (Exception ex) 
            { 
                _logger.LogError(ex, "API: Lỗi không xác định khi thêm sản phẩm vào hóa đơn {HoaDonId}", hoaDonId);
                return StatusCode(500, $"Lỗi nội bộ server khi thêm sản phẩm: {ex.Message}");
            }
        }

        /// <summary>
        /// Cập nhật số lượng sản phẩm trong hóa đơn.
        /// </summary>
        /// <param name="hoaDonId">ID của hóa đơn.</param>
        /// <param name="sanPhamChiTietId">ID của sản phẩm chi tiết cần cập nhật.</param>
        /// <param name="request">Body chứa số lượng mới.</param>
        [HttpPut("hoa-don/{hoaDonId}/items/{sanPhamChiTietId}")]
        [ProducesResponseType(typeof(HoaDonBanHangDto), 200)]
        public async Task<IActionResult> CapNhatSoLuong(Guid hoaDonId, Guid sanPhamChiTietId, [FromBody] CapNhatSoLuongRequest request)
        {
            try
            {
                var result = await _banHangService.CapNhatSoLuongSanPhamAsync(hoaDonId, sanPhamChiTietId, request.SoLuongMoi);
                return Ok(result);
            }
            catch (KeyNotFoundException ex) { return NotFound(ex.Message); }
            catch (InvalidOperationException ex) { return BadRequest(ex.Message); }
        }

        /// <summary>
        /// Xóa một sản phẩm khỏi hóa đơn.
        /// </summary>
        /// <param name="hoaDonId">ID của hóa đơn.</param>
        /// <param name="sanPhamChiTietId">ID của sản phẩm chi tiết cần xóa.</param>
        [HttpDelete("hoa-don/{hoaDonId}/items/{sanPhamChiTietId}")]
        [ProducesResponseType(typeof(HoaDonBanHangDto), 200)]
        public async Task<IActionResult> XoaSanPham(Guid hoaDonId, Guid sanPhamChiTietId)
        {
            try
            {
                var result = await _banHangService.XoaSanPhamKhoiHoaDonAsync(hoaDonId, sanPhamChiTietId);
                return Ok(result);
            }
            catch (KeyNotFoundException ex) { return NotFound(ex.Message); }
            catch (InvalidOperationException ex) { return BadRequest(ex.Message); }
        }

        #endregion

        #region Voucher & Khách hàng

        /// <summary>
        /// Gán khách hàng vào hóa đơn.
        /// </summary>
        /// <param name="hoaDonId">ID của hóa đơn cần gán.</param>
        /// <param name="request">Body chứa ID khách hàng.</param>
        [HttpPut("hoa-don/{hoaDonId}/khach-hang")]
        [ProducesResponseType(typeof(HoaDonBanHangDto), 200)]
        public async Task<IActionResult> GanKhachHang(Guid hoaDonId, [FromBody] GanKhachHangRequest? request)
        {
            try
            {
                Guid? khachHangId = request?.KhachHangId;
                var result = await _banHangService.GanKhachHangAsync(hoaDonId, khachHangId);
                return Ok(result);
            }
            catch (KeyNotFoundException ex) { return NotFound(ex.Message); }
            catch (InvalidOperationException ex) { return BadRequest(ex.Message); }
        }

        /// <summary>
        /// Áp dụng voucher bằng mã.
        /// </summary>
        /// <param name="hoaDonId">ID của hóa đơn cần áp dụng.</param>
        /// <param name="request">Body chứa mã voucher.</param>
        [HttpPost("hoa-don/{hoaDonId}/voucher")]
        [ProducesResponseType(typeof(HoaDonBanHangDto), 200)]
        public async Task<IActionResult> ApDungVoucher(Guid hoaDonId, [FromBody] ApDungVoucherRequest request)
        {
            try
            {
                var result = await _banHangService.ApDungVoucherAsync(hoaDonId, request.MaVoucher);
                return Ok(result);
            }
            catch (KeyNotFoundException ex) { return NotFound(ex.Message); }
            catch (InvalidOperationException ex) { return BadRequest(ex.Message); }
        }

        /// <summary>
        /// Áp dụng voucher bằng mã sử dụng logic của GioHang.
        /// </summary>
        /// <param name="hoaDonId">ID của hóa đơn cần áp dụng.</param>
        /// <param name="request">Body chứa mã voucher.</param>
        [HttpPost("{hoaDonId}/ap-dung-voucher-by-code")]
        [ProducesResponseType(typeof(object), 200)]
        public async Task<IActionResult> ApDungVoucherByCode([FromBody] ApDungVoucherByCodeRequest request, Guid hoaDonId)
        {
            try
            {
                _logger.LogInformation($"🎫 Applying voucher by code: {request.MaVoucher} for invoice: {hoaDonId}");
                
                // ✅ Lấy hóa đơn để có KhachHangId
                var hoaDon = await _banHangService.GetHoaDonByIdAsync(hoaDonId);
                if (hoaDon?.KhachHang?.KhachHangId == null)
                {
                    _logger.LogWarning($"🎫 Invoice {hoaDonId} has no customer assigned");
                    return BadRequest(new { success = false, message = "Hóa đơn chưa có khách hàng" });
                }
                
                var khachHangId = hoaDon.KhachHang.KhachHangId;
                _logger.LogInformation($"🎫 Using customer: {khachHangId}");

                // ✅ Sử dụng logic của GioHang: tìm voucher theo mã và áp dụng theo KhachHangId
                var vouchers = await _banHangService.TimKiemVoucherHopLeAsync(hoaDonId);
                var voucher = vouchers.FirstOrDefault(v => v.MaVoucher == request.MaVoucher);
                
                if (voucher == null)
                {
                    _logger.LogWarning($"🎫 Voucher {request.MaVoucher} not found in valid vouchers");
                    return BadRequest(new { success = false, message = $"Mã voucher '{request.MaVoucher}' không tồn tại hoặc không hợp lệ" });
                }
                
                _logger.LogInformation($"🎫 Found voucher: {voucher.MaVoucher} - {voucher.TenVoucher}");

                // ✅ Gọi API của GioHang trực tiếp để đảm bảo logic giống hệt
                var result = await _banHangService.ApDungVoucherGioHangAsync(khachHangId, voucher.VoucherId);
                _logger.LogInformation($"🎫 Voucher applied successfully: {result}");
                
                return Ok(new { success = true, data = result });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"🎫 Error applying voucher {request.MaVoucher}: {ex.Message}");
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Gỡ bỏ voucher khỏi hóa đơn.
        /// </summary>
        /// <param name="hoaDonId">ID của hóa đơn cần gỡ voucher.</param>
        [HttpDelete("hoa-don/{hoaDonId}/voucher")]
        [ProducesResponseType(typeof(HoaDonBanHangDto), 200)]
        public async Task<IActionResult> GoBoVoucher(Guid hoaDonId)
        {
            try
            {
                var result = await _banHangService.GoBoVoucherAsync(hoaDonId);
                return Ok(result);
            }
            catch (KeyNotFoundException ex) { return NotFound(ex.Message); }
            catch (InvalidOperationException ex) { return BadRequest(ex.Message); }
        }
        #endregion

        #region Cập nhật thông tin địa chỉ giao hàng

        /// <summary>
        /// Cập nhật thông tin địa chỉ giao hàng cho hóa đơn.
        /// </summary>
        /// <param name="hoaDonId">ID của hóa đơn cần cập nhật.</param>
        /// <param name="request">Thông tin địa chỉ giao hàng mới.</param>
        [HttpPut("hoa-don/{hoaDonId}/dia-chi-giao-hang")]
        [ProducesResponseType(typeof(HoaDonBanHangDto), 200)]
        public async Task<IActionResult> CapNhatDiaChiGiaoHang(Guid hoaDonId, [FromBody] DiaChiMoiDto request)
        {
            try
            {
                var result = await _banHangService.CapNhatDiaChiGiaoHangAsync(hoaDonId, request);
                return Ok(result);
            }
            catch (KeyNotFoundException ex) { return NotFound(ex.Message); }
            catch (InvalidOperationException ex) { return BadRequest(ex.Message); }
        }

        #endregion

        #region Thanh toán

        /// <summary>
        /// Thực hiện thanh toán hóa đơn.
        /// </summary>
        /// <param name="hoaDonId">ID hóa đơn cần thanh toán.</param>
        /// <param name="request">Thông tin thanh toán.</param>
        [HttpPost("hoa-don/{hoaDonId}/thanh-toan")]
        [ProducesResponseType(typeof(HoaDonBanHangDto), 200)]
        public async Task<IActionResult> ThanhToan(Guid hoaDonId, [FromBody] ThanhToanRequest request)
        {
            try
            {
                if (request == null)
                {
                    return BadRequest("Dữ liệu thanh toán không hợp lệ.");
                }
                
                // ✅ Kiểm tra hình thức thanh toán cho BanHang (Tiền mặt và Chuyển khoản)
                if (request.HinhThucThanhToanId == Guid.Empty)
                {
                    return BadRequest("Vui lòng chọn hình thức thanh toán.");
                }
                
                request.HoaDonId = hoaDonId;
                var result = await _banHangService.ThanhToanHoaDonAsync(request);
                return Ok(result);
            }
            catch (KeyNotFoundException ex) { return NotFound(ex.Message); }
            catch (InvalidOperationException ex) { return BadRequest(ex.Message); }
        }
        #endregion

        #region Tìm kiếm

        /// <summary>
        /// Tìm kiếm sản phẩm theo từ khóa (tên, mã...).
        /// </summary>
        /// <param name="keyword">Từ khóa tìm kiếm.</param>
        [HttpGet("tim-kiem/san-pham")]
        [ProducesResponseType(typeof(IEnumerable<SanPhamBanHangDto>), 200)]
        public async Task<IActionResult> TimKiemSanPham([FromQuery] string keyword)
        {
            var result = await _banHangService.TimKiemSanPhamAsync(keyword);
            return Ok(result);
        }

        /// <summary>
        /// Tìm kiếm khách hàng theo từ khóa (tên, SĐT...).
        /// </summary>
        /// <param name="keyword">Từ khóa tìm kiếm (optional).</param>
        [HttpGet("tim-kiem/khach-hang")]
        [ProducesResponseType(typeof(IEnumerable<KhachHangDto>), 200)]
        public async Task<IActionResult> TimKiemKhachHang([FromQuery] string? keyword = null)
        {
            try
            {
                _logger.LogInformation("Tìm kiếm khách hàng với từ khóa: '{Keyword}'", keyword ?? "null");
            var result = await _banHangService.TimKiemKhachHangAsync(keyword);
                _logger.LogInformation("Tìm thấy {Count} khách hàng", result?.Count() ?? 0);
            return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tìm kiếm khách hàng với từ khóa: '{Keyword}'", keyword ?? "null");
                return BadRequest($"Lỗi khi tìm kiếm khách hàng: {ex.Message}");
            }
        }

        /// <summary>
        /// Tìm các voucher hợp lệ có thể dùng cho một hóa đơn.
        /// </summary>
        /// <param name="hoaDonId">ID của hóa đơn đang thao tác.</param>
        [HttpGet("hoa-don/{hoaDonId}/vouchers-hop-le")]
        [ProducesResponseType(typeof(IEnumerable<VoucherDto>), 200)]
        public async Task<IActionResult> TimKiemVoucherHopLe(Guid hoaDonId)
        {
            var result = await _banHangService.TimKiemVoucherHopLeAsync(hoaDonId);
            return Ok(result);
        }

        #endregion

        #region Khách hàng (Tạo nhanh)

        /// <summary>
        /// Tạo nhanh một khách hàng mới trong quá trình bán hàng.
        /// </summary>
        /// <param name="request">Thông tin khách hàng mới.</param>
        [HttpPost("khach-hang")]
        [ProducesResponseType(typeof(KhachHangDto), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<IActionResult> TaoKhachHangMoi([FromBody] TaoKhachHangRequest request)
        {
            try
            {
                var result = await _banHangService.TaoKhachHangMoiAsync(request);
                return Ok(result);
            }
            catch (ValidationException ex) { return BadRequest(ex.Message); }
            catch (InvalidOperationException ex) { return BadRequest(ex.Message); }
        }
        [HttpGet("tim-kiem/san-pham-goi-y")]
        [ProducesResponseType(typeof(IEnumerable<SanPhamBanHangDto>), 200)]
        public async Task<IActionResult> LaySanPhamGoiY()
        {
            try
            {
                // Bạn có thể thay đổi số lượng sản phẩm gợi ý ở đây, ví dụ 15 sản phẩm.
                var result = await _banHangService.TimKiemSanPhamAsync(null); // Gọi với keyword là null/rỗng
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy danh sách sản phẩm gợi ý.");
                // Trả về lỗi 500 nếu có vấn đề ở server
                return StatusCode(500, "Đã xảy ra lỗi hệ thống khi lấy sản phẩm gợi ý.");
            }
        }

        /// <summary>
        /// Sửa dữ liệu hóa đơn bị lỗi (tổng tiền = 0)
        /// </summary>
        /// <response code="200">Sửa thành công.</response>
        /// <response code="500">Lỗi server.</response>
        [HttpPost("fix-invoice-data")]
        [ProducesResponseType(typeof(string), 200)]
        [ProducesResponseType(typeof(string), 500)]
        public async Task<IActionResult> FixInvoiceData()
        {
            try
            {
                await _banHangService.FixInvoiceDataAsync();
                return Ok("Đã sửa dữ liệu hóa đơn thành công");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi sửa dữ liệu hóa đơn");
                return StatusCode(500, "Lỗi khi sửa dữ liệu hóa đơn");
            }
        }

        /// <summary>
        /// Lấy QR code chuyển khoản cho hóa đơn
        /// </summary>
        /// <param name="hoaDonId">ID của hóa đơn</param>
        /// <response code="200">Trả về thông tin QR code</response>
        /// <response code="404">Không tìm thấy hóa đơn</response>
        [HttpGet("hoa-don/{hoaDonId}/qr-code")]
        [ProducesResponseType(typeof(object), 200)]
        [ProducesResponseType(typeof(string), 404)]
        public async Task<IActionResult> GetQRCode(Guid hoaDonId)
        {
            try
            {
                var hoaDon = await _banHangService.GetHoaDonByIdAsync(hoaDonId);
                if (hoaDon == null)
                {
                    return NotFound("Không tìm thấy hóa đơn");
                }

                // Tạo QR code URL với thông tin hóa đơn
                var qrCodeUrl = $"https://img.vietqr.io/image/acb-40070087-compact2.jpg?amount={hoaDon.ThanhTien}&addInfo=Chuyen%20tien%20mua%20hang%20FurryFriends&accountName=Nguyen%20Minh%20Quan";
                
                var result = new
                {
                    qrCodeUrl = qrCodeUrl,
                    amount = hoaDon.ThanhTien,
                    hoaDonId = hoaDon.HoaDonId,
                    maHoaDon = hoaDon.MaHoaDon
                };

                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tạo QR code cho hóa đơn {HoaDonId}", hoaDonId);
                return StatusCode(500, "Lỗi khi tạo QR code");
            }
        }

        /// <summary>
        /// Áp dụng voucher theo hóa đơn (tổng tiền hóa đơn).
        /// </summary>
        /// <param name="hoaDonId">ID của hóa đơn.</param>
        /// <param name="request">Body chứa mã voucher.</param>
        [HttpPost("{hoaDonId}/ap-dung-voucher")]
        [ProducesResponseType(typeof(object), 200)]
        public async Task<IActionResult> ApDungVoucherHoaDon(Guid hoaDonId, [FromBody] ApDungVoucherByCodeRequest request)
        {
            try
            {
                _logger.LogInformation($"🎫 Applying voucher: {request.MaVoucher} for invoice: {hoaDonId}");
                
                // ✅ Lấy hóa đơn để có tổng tiền
                var hoaDon = await _banHangService.GetHoaDonByIdAsync(hoaDonId);
                if (hoaDon == null)
                {
                    _logger.LogWarning($"🎫 Invoice {hoaDonId} not found");
                    return BadRequest(new { success = false, message = "Hóa đơn không tồn tại" });
                }
                
                _logger.LogInformation($"🎫 Invoice total: {hoaDon.TongTien:N0} VNĐ");

                // ✅ Tìm voucher theo mã
                var voucher = await _context.Vouchers
                    .AsNoTracking()
                    .FirstOrDefaultAsync(v => v.MaVoucher == request.MaVoucher && v.TrangThai == 1);
                
                if (voucher == null)
                {
                    _logger.LogWarning($"🎫 Voucher {request.MaVoucher} not found or invalid");
                    
                    // ✅ Debug: Kiểm tra tất cả voucher trong database
                    var allVouchers = await _context.Vouchers
                        .AsNoTracking()
                        .Select(v => new { v.MaVoucher, v.TenVoucher, v.TrangThai, v.SoLuong })
                        .ToListAsync();
                    
                    _logger.LogWarning($"🎫 All vouchers in DB: {string.Join(", ", allVouchers.Select(v => $"{v.MaVoucher}({v.TrangThai})"))}");
                    
                    return BadRequest(new { success = false, message = $"Mã voucher '{request.MaVoucher}' không tồn tại hoặc không hợp lệ" });
                }
                
                _logger.LogInformation($"🎫 Found voucher: {voucher.MaVoucher} - {voucher.TenVoucher} - Status: {voucher.TrangThai} - Quantity: {voucher.SoLuong}");

                // ✅ Tính toán voucher theo tổng tiền hóa đơn
                var tongTienHang = hoaDon.TongTien;
                var phiVanChuyen = 0m; // BanHang không có phí ship
                var tongDonHang = tongTienHang + phiVanChuyen;
                
                _logger.LogInformation($"🎫 Total order amount: {tongDonHang:N0} VNĐ");
                
                // ✅ Kiểm tra điều kiện voucher
                var now = DateTime.Now;
                if (voucher.NgayBatDau > now || voucher.NgayKetThuc < now)
                {
                    return BadRequest(new { success = false, message = "Voucher đã hết hạn hoặc chưa có hiệu lực" });
                }
                
                if (voucher.SoLuong <= 0)
                {
                    return BadRequest(new { success = false, message = "Voucher đã hết lượt sử dụng" });
                }
                
                if (tongDonHang < voucher.SoTienApDungToiThieu)
                {
                    return BadRequest(new { success = false, message = $"Đơn hàng tối thiểu {voucher.SoTienApDungToiThieu:N0} VNĐ để áp dụng voucher" });
                }
                
                // ✅ Tính số tiền giảm
                var soTienGiam = 0m;
                if (voucher.PhanTramGiam > 0)
                {
                    soTienGiam = tongDonHang * voucher.PhanTramGiam / 100;
                    if (voucher.GiaTriGiamToiDa.HasValue && soTienGiam > voucher.GiaTriGiamToiDa.Value)
                    {
                        soTienGiam = voucher.GiaTriGiamToiDa.Value;
                    }
                }
                // Voucher chỉ có phần trăm giảm, không có số tiền giảm cố định
                
                var tongTienSauGiam = tongDonHang - soTienGiam;
                
                _logger.LogInformation($"🎫 Discount amount: {soTienGiam:N0} VNĐ");
                _logger.LogInformation($"🎫 Final amount: {tongTienSauGiam:N0} VNĐ");
                
                // ✅ Lưu thông tin voucher vào hóa đơn
                var hoaDonEntity = await _context.HoaDons.FindAsync(hoaDonId);
                if (hoaDonEntity != null)
                {
                    hoaDonEntity.VoucherId = voucher.VoucherId;
                    hoaDonEntity.TongTienSauKhiGiam = tongTienSauGiam; // Sử dụng TongTienSauKhiGiam thay vì ThanhTien
                    hoaDonEntity.ThoiGianThayDoiTrangThai = DateTime.Now; // Sử dụng ThoiGianThayDoiTrangThai thay vì NgayCapNhat
                    
                    // ✅ Giảm số lượng voucher
                    voucher.SoLuong -= 1;
                    _context.Vouchers.Update(voucher);
                    
                    await _context.SaveChangesAsync();
                    _logger.LogInformation($"🎫 Saved voucher to invoice: VoucherId={voucher.VoucherId}, TienGiam={soTienGiam}, VoucherSoLuongConLai={voucher.SoLuong}");
                }
                else
                {
                    _logger.LogWarning($"🎫 Could not find invoice entity to save voucher: {hoaDonId}");
                }
                
                // ✅ Trả về kết quả
                var result = new
                {
                    tenVoucher = voucher.TenVoucher,
                    maVoucher = voucher.MaVoucher,
                    soTienGiam = soTienGiam,
                    soTienApDungToiThieu = voucher.SoTienApDungToiThieu,
                    tongTienHang = tongTienHang,
                    tongTienSauGiam = tongTienSauGiam
                };
                
                return Ok(new { success = true, data = result });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"🎫 Error applying voucher {request.MaVoucher}: {ex.Message}");
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

       

        #endregion
    }
}