using FurryFriends.API.Data;
using FurryFriends.API.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FurryFriends.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VoucherValidationController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly VoucherCalculationService _voucherService;

        public VoucherValidationController(AppDbContext context, VoucherCalculationService voucherService)
        {
            _context = context;
            _voucherService = voucherService;
        }

        [HttpPost("validate")]
        public async Task<IActionResult> ValidateVoucher([FromBody] ValidateVoucherRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.VoucherCode))
                {
                    return BadRequest(new { success = false, message = "Mã voucher không được để trống" });
                }

                // Tìm voucher theo mã (ưu tiên), fallback theo tên cũ
                var code = (request.VoucherCode ?? string.Empty).Trim().ToUpper();
                var voucher = await _context.Vouchers
                    .FirstOrDefaultAsync(v => v.MaVoucher == code || v.TenVoucher.ToLower() == request.VoucherCode.ToLower());

                if (voucher == null)
                {
                    return Ok(new { success = false, message = "Mã voucher không tồn tại" });
                }

                // Tính phí vận chuyển: trên 500k thì freeship, dưới 500k thì tính ship 30k
                var phiVanChuyen = _voucherService.CalculateShippingFee(request.TongTienHang, 30000, 500000);
                var tongDonHang = request.TongTienHang + phiVanChuyen;

                // Kiểm tra voucher với tổng đơn hàng bao gồm phí ship
                var result = _voucherService.GetVoucherApplication(voucher, request.TongTienHang, phiVanChuyen);

                if (result.IsValid)
                {
                    return Ok(new
                    {
                        success = true,
                        voucherId = voucher.VoucherId,
                        maVoucher = voucher.MaVoucher,
                        tenVoucher = voucher.TenVoucher,
                        phanTramGiam = voucher.PhanTramGiam,
                        soTienGiam = result.SoTienGiam,
                        giaTriGiamToiDa = voucher.GiaTriGiamToiDa,
                        soTienApDungToiThieu = voucher.SoTienApDungToiThieu,
                        phiVanChuyen = phiVanChuyen,
                        tongDonHang = tongDonHang,
                        message = $"Áp dụng thành công! Giảm {result.SoTienGiam:N0} VNĐ"
                    });
                }
                else
                {
                    return Ok(new
                    {
                        success = false,
                        message = result.LyDoKhongHopLe,
                        tenVoucher = voucher.TenVoucher,
                        maVoucher = voucher.MaVoucher,
                        phanTramGiam = voucher.PhanTramGiam,
                        giaTriGiamToiDa = voucher.GiaTriGiamToiDa,
                        soTienApDungToiThieu = voucher.SoTienApDungToiThieu,
                        phiVanChuyen = phiVanChuyen,
                        tongDonHang = tongDonHang
                    });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Có lỗi xảy ra khi kiểm tra voucher" });
            }
        }

        [HttpGet("available/{khachHangId}")]
        public async Task<IActionResult> GetAvailableVouchers(Guid khachHangId, [FromQuery] decimal tongTienHang = 0)
        {
            try
            {
                var now = DateTime.Now;
                var vouchers = await _context.Vouchers
                    .Where(v => v.TrangThai == 1 && 
                               v.NgayBatDau <= now && 
                               v.NgayKetThuc >= now && 
                               v.SoLuong > 0)
                    .ToListAsync();

                // Tính phí vận chuyển: trên 500k thì freeship, dưới 500k thì tính ship 30k
                var phiVanChuyen = _voucherService.CalculateShippingFee(tongTienHang, 30000, 500000);
                var tongDonHang = tongTienHang + phiVanChuyen;

                var availableVouchers = vouchers
                    .Where(v => !v.SoTienApDungToiThieu.HasValue || tongDonHang >= v.SoTienApDungToiThieu.Value)
                    .Select(v => new
                    {
                        voucherId = v.VoucherId,
                        maVoucher = v.MaVoucher,
                        tenVoucher = v.TenVoucher,
                        phanTramGiam = v.PhanTramGiam,
                        giaTriGiamToiDa = v.GiaTriGiamToiDa,
                        soTienApDungToiThieu = v.SoTienApDungToiThieu,
                        phiVanChuyen = phiVanChuyen,
                        tongDonHang = tongDonHang,
                        soTienGiam = _voucherService.CalculateVoucherDiscount(v, tongTienHang, phiVanChuyen)
                    })
                    .ToList();

                return Ok(new { success = true, vouchers = availableVouchers });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Có lỗi xảy ra khi lấy danh sách voucher" });
            }
        }
    }

    public class ValidateVoucherRequest
    {
        public string VoucherCode { get; set; } = "";
        public decimal TongTienHang { get; set; }
    }
}
