using FurryFriends.API.Data;
using FurryFriends.API.Models.DTO;
using FurryFriends.API.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FurryFriends.API.Services;

namespace FurryFriends.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GioHangController : ControllerBase
    {
        private readonly IGioHangRepository _repo;
        private readonly AppDbContext _context;
        private readonly VoucherCalculationService _voucherCalc;
        public GioHangController(IGioHangRepository repo, AppDbContext context, VoucherCalculationService voucherCalc)
        {
            _repo = repo;
            _context = context;
            _voucherCalc = voucherCalc;
        }

        [HttpGet("{khachHangId}")]
        public async Task<IActionResult> GetGioHang(Guid khachHangId)
        {
            var gioHang = await _repo.GetGioHangByKhachHangIdAsync(khachHangId);
            return Ok(gioHang);
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddToCart([FromBody] AddToCartDTO model)
        {
            try
            {
                var chiTiet = await _repo.GetSanPhamChiTietByIdAsync(model.SanPhamChiTietId);
                if (chiTiet == null)
                {
                    return NotFound("Không tìm thấy chi tiết sản phẩm.");
                }

                var khachHangExists = await _context.KhachHangs.AnyAsync(kh => kh.KhachHangId == model.KhachHangId);
                if (!khachHangExists)
                {
                    return BadRequest("Khách hàng không tồn tại.");
                }

                if (chiTiet.SanPhamId == Guid.Empty)
                {
                    return BadRequest("Chi tiết sản phẩm không có ID sản phẩm.");
                }

                var result = await _repo.AddSanPhamVaoGioAsync(
                    model.KhachHangId,
                    model.SanPhamChiTietId,
                    model.SoLuong
                );

                var dto = await _repo.ConvertToDTOAsync(result);                // ✅ Convert sang DTO có TenSanPham

                return Ok(dto);
            }
            catch (Exception ex)
            {
                //Console.WriteLine("❌ Lỗi khi thêm vào giỏ hàng: " + ex);
                //return StatusCode(500, new { message = ex.Message, detail = ex.InnerException?.Message });
                return BadRequest(ex.Message);
            }
        }



        [HttpPut("update/{gioHangChiTietId}")]
        public async Task<IActionResult> UpdateSoLuong(Guid gioHangChiTietId, [FromBody] int soLuong)
        {
            try
            {
                var result = await _repo.UpdateSoLuongAsync(gioHangChiTietId, soLuong);
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("delete/{gioHangChiTietId}")]
        public async Task<IActionResult> Delete(Guid gioHangChiTietId)
        {
            var result = await _repo.RemoveSanPhamKhoiGioAsync(gioHangChiTietId);
            return Ok(new { success = result });
        }

        // ✅ Method test database lock
        [HttpGet("test-lock/{sanPhamChiTietId}")]
        public async Task<IActionResult> TestDatabaseLock(Guid sanPhamChiTietId)
        {
            try
            {
                var result = await _repo.TestDatabaseLockAsync(sanPhamChiTietId);
                return Ok(new { message = result });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("test-voucher-lock/{voucherId}")]
        public async Task<IActionResult> TestVoucherLock(Guid voucherId)
        {
            try
            {
                var result = await _repo.TestVoucherLockAsync(voucherId);
                return Ok(new { message = result });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("kiem-tra-don-trung-lap/{khachHangId}")]
        public async Task<IActionResult> KiemTraDonTrungLap(Guid khachHangId)
        {
            try
            {
                var result = await _repo.KiemTraVaXoaDonTrungLapAsync(khachHangId);
                return Ok(new { 
                    success = true, 
                    message = result 
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { 
                    success = false, 
                    message = ex.Message 
                });
            }
        }



        [HttpPost("ap-dung-voucher")]
        public async Task<IActionResult> ApDungVoucher([FromBody] GioHangVoucherDTO dto)
        {
            Console.WriteLine($"👉 Nhận được yêu cầu áp dụng voucher với KhachHangId = {dto.KhachHangId}, VoucherId = {dto.VoucherId}");

            var gioHang = await _repo.GetGioHangEntityByKhachHangIdAsync(dto.KhachHangId);
            if (gioHang == null)
            {
                Console.WriteLine("❌ Không tìm thấy giỏ hàng.");
                return NotFound("Không tìm thấy giỏ hàng hoặc giỏ hàng trống.");
            }

            if (gioHang.GioHangChiTiets == null || !gioHang.GioHangChiTiets.Any())
            {
                Console.WriteLine("❌ Giỏ hàng không có sản phẩm nào.");
                return NotFound("Không tìm thấy giỏ hàng hoặc giỏ hàng trống.");
            }

            var voucher = await _context.Vouchers
                .FirstOrDefaultAsync(v => v.VoucherId == dto.VoucherId && v.TrangThai == 1);

            if (voucher == null)
            {
                Console.WriteLine("❌ Không tìm thấy voucher hoặc voucher bị khóa.");
                return BadRequest("Voucher không hợp lệ hoặc đã hết hạn.");
            }

            Console.WriteLine($"🔎 Voucher tìm thấy: {voucher.TenVoucher}, Phần trăm giảm: {voucher.PhanTramGiam}, Số lượng: {voucher.SoLuong}, Bắt đầu: {voucher.NgayBatDau}, Kết thúc: {voucher.NgayKetThuc}");

            var tongTienHang = gioHang.GioHangChiTiets.Sum(ct => ct.ThanhTien);
            
            Console.WriteLine($"🔍 [Controller] Chi tiết giỏ hàng:");
            foreach (var item in gioHang.GioHangChiTiets)
            {
                Console.WriteLine($"  - Sản phẩm: {item.SanPhamChiTiet?.SanPham?.TenSanPham}, Số lượng: {item.SoLuong}, Đơn giá: {item.DonGia:N0}, Thành tiền: {item.ThanhTien:N0}");
            }
            Console.WriteLine($"🔍 [Controller] Tổng tiền hàng (Sum): {tongTienHang:N0}");
            
            // Kiểm tra tính toán thủ công
            var tongTienHangTinhLai = 0m;
            foreach (var item in gioHang.GioHangChiTiets)
            {
                var thanhTienTinhLai = item.DonGia * item.SoLuong;
                tongTienHangTinhLai += thanhTienTinhLai;
                Console.WriteLine($"  - Kiểm tra: {item.DonGia:N0} × {item.SoLuong} = {thanhTienTinhLai:N0}");
            }
            Console.WriteLine($"🔍 [Controller] Tổng tiền hàng tính lại: {tongTienHangTinhLai:N0}");
            Console.WriteLine($"🔍 [Controller] Chênh lệch: {tongTienHang - tongTienHangTinhLai:N0}");
            
            // Tính phí vận chuyển: trên 500k thì freeship, dưới 500k thì tính ship 30k
            var phiVanChuyen = _voucherCalc.CalculateShippingFee(tongTienHang, 30000, 500000);
            var tongDonHang = tongTienHang + phiVanChuyen;
            
            Console.WriteLine($"🔍 [Controller] Phí vận chuyển: {phiVanChuyen:N0}");
            Console.WriteLine($"🔍 [Controller] Tổng đơn hàng: {tongDonHang:N0}");

            Console.WriteLine($"💰 Tổng tiền hàng: {tongTienHang:N0} VNĐ");
            Console.WriteLine($"🚚 Phí vận chuyển: {phiVanChuyen:N0} VNĐ");
            Console.WriteLine($"💳 Tổng đơn hàng: {tongDonHang:N0} VNĐ");

            // Tính giảm với giới hạn tối đa (dựa trên tổng đơn hàng bao gồm phí ship)
            var apply = _voucherCalc.GetVoucherApplication(voucher, tongTienHang, phiVanChuyen);
            
            if (!apply.IsValid)
            {
                Console.WriteLine($"❌ Voucher không hợp lệ: {apply.LyDoKhongHopLe}");
                return BadRequest(apply.LyDoKhongHopLe);
            }

            Console.WriteLine($"✅ Voucher hợp lệ - tính toán giảm giá");

            // Tính giảm với giới hạn tối đa (dựa trên tổng đơn hàng bao gồm phí ship)
            var soTienGiam = apply.SoTienGiam;
            var tongTienSauGiam = tongDonHang - soTienGiam;
            
            Console.WriteLine($"🔍 [Controller] Số tiền giảm: {soTienGiam:N0}");
            Console.WriteLine($"🔍 [Controller] Tổng sau giảm: {tongTienSauGiam:N0}");
            
            // Kiểm tra tính toán cuối cùng
            Console.WriteLine($"🔍 [Controller] Kiểm tra tính toán cuối cùng:");
            Console.WriteLine($"  - Tổng tiền hàng: {tongTienHang:N0}");
            Console.WriteLine($"  - Phí vận chuyển: {phiVanChuyen:N0}");
            Console.WriteLine($"  - Tổng trước giảm: {tongDonHang:N0}");
            Console.WriteLine($"  - Số tiền giảm: {soTienGiam:N0}");
            Console.WriteLine($"  - Tổng sau giảm: {tongTienSauGiam:N0}");
            Console.WriteLine($"  - Kiểm tra: {tongDonHang:N0} - {soTienGiam:N0} = {tongTienSauGiam:N0}");

            Console.WriteLine($"🎫 Phần trăm giảm: {voucher.PhanTramGiam}%");
            Console.WriteLine($"💸 Số tiền giảm: {soTienGiam:N0} VNĐ");
            Console.WriteLine($"💳 Tổng sau giảm: {tongTienSauGiam:N0} VNĐ");

            return Ok(new
            {
                TongTienHang = tongTienHang,
                PhiVanChuyen = phiVanChuyen,
                TongDonHang = tongDonHang,
                GiamGia = soTienGiam,
                TienSauGiam = tongTienSauGiam,
                PhanTramGiam = voucher.PhanTramGiam,
                TenVoucher = voucher.TenVoucher,
                MaVoucher = voucher.MaVoucher
            });
        }

        [HttpPost("thanh-toan")]
        public async Task<IActionResult> ThanhToan([FromBody] ThanhToanDTO dto)
        {
            try
            {
                // ✅ Validation: Kiểm tra địa chỉ giao hàng
                if (dto.DiaChiGiaoHangId == Guid.Empty)
                {
                    return BadRequest(new { 
                        success = false, 
                        message = "Vui lòng chọn địa chỉ giao hàng trước khi thanh toán!" 
                    });
                }

                var result = await _repo.ThanhToanAsync(dto);
                Console.WriteLine($"[Controller] Kết quả thanh toán: {System.Text.Json.JsonSerializer.Serialize(result)}");

                return Ok(result);
            }
            catch (Exception ex)
            {
                // ✅ Xử lý exception và trả về thông báo lỗi thân thiện
                Console.WriteLine($"[Controller] Lỗi thanh toán: {ex.Message}");
                
                return BadRequest(new { 
                    success = false, 
                    message = ex.Message 
                });
            }
        }

        [HttpGet("cho-duyet-count/{khachHangId}")]
        public async Task<IActionResult> GetChoDuyetCount(Guid khachHangId)
        {
            // NOTE: Nếu bạn lưu trạng thái dạng int/enum,
            // hãy đổi điều kiện tương ứng (vd: hd.TrangThai == (int)TrangThaiHoaDon.ChoDuyet)
            var count = await _context.HoaDons
                .Where(hd => hd.KhachHangId == khachHangId && hd.TrangThai == 0)
                .CountAsync();

            return Ok(new { count });
        }


    }

    public class AddToCartDTO
    {
        public Guid KhachHangId { get; set; }
        public Guid SanPhamChiTietId { get; set; }
        public int SoLuong { get; set; }
        public Guid? VoucherId { get; set; }
    }

}
