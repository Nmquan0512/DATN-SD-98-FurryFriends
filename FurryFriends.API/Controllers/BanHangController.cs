using FurryFriends.API.Models.DTO.BanHang;
using FurryFriends.API.Models.DTO.BanHang.Requests;
using FurryFriends.API.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Threading.Tasks;

namespace FurryFriends.API.Controllers
{
    /// <summary>
    /// API quản lý các nghiệp vụ của chức năng bán hàng.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // Yêu cầu người dùng phải đăng nhập để sử dụng các API này
    public class BanHangController : ControllerBase
    {
        private readonly IBanHangService _banHangService;
        private readonly ILogger<BanHangController> _logger;

        public BanHangController(IBanHangService banHangService, ILogger<BanHangController> logger)
        {
            _banHangService = banHangService;
            _logger = logger;
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
                // Lấy NhanVienId từ token JWT của người dùng đang đăng nhập
                var nhanVienIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (nhanVienIdClaim == null || !Guid.TryParse(nhanVienIdClaim.Value, out var nhanVienId))
                {
                    return Unauthorized("Không thể xác định nhân viên từ token.");
                }
                request.NhanVienId = nhanVienId;

                var result = await _banHangService.TaoHoaDonAsync(request);
                return CreatedAtAction(nameof(GetHoaDonById), new { hoaDonId = result.HoaDonId }, result);
            }
            catch (ArgumentException ex) { return BadRequest(ex.Message); }
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
                var fullRequest = new ThemSanPhamVaoHoaDonRequest
                {
                    HoaDonId = hoaDonId,
                    SanPhamChiTietId = request.SanPhamChiTietId,
                    SoLuong = request.SoLuong
                };
                var result = await _banHangService.ThemSanPhamVaoHoaDonAsync(fullRequest);
                return Ok(result);
            }
            catch (KeyNotFoundException ex) { return NotFound(ex.Message); }
            catch (InvalidOperationException ex) { return BadRequest(ex.Message); }
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
        public async Task<IActionResult> GanKhachHang(Guid hoaDonId, [FromBody] GanKhachHangRequest request)
        {
            try
            {
                var result = await _banHangService.GanKhachHangAsync(hoaDonId, request.KhachHangId);
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
        /// <param name="keyword">Từ khóa tìm kiếm.</param>
        [HttpGet("tim-kiem/khach-hang")]
        [ProducesResponseType(typeof(IEnumerable<KhachHangDto>), 200)]
        public async Task<IActionResult> TimKiemKhachHang([FromQuery] string keyword)
        {
            var result = await _banHangService.TimKiemKhachHangAsync(keyword);
            return Ok(result);
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

        #endregion
    }

}