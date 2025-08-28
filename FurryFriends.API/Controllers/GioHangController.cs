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
