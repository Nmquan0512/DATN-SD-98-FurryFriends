using FurryFriends.API.Data;
using FurryFriends.API.Models.DTO;
using FurryFriends.API.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FurryFriends.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GioHangController : ControllerBase
    {
        private readonly IGioHangRepository _repo;
        private readonly AppDbContext _context;
        public GioHangController(IGioHangRepository repo, AppDbContext context)
        {
            _repo = repo;
            _context = context;
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
                Console.WriteLine("❌ Lỗi khi thêm vào giỏ hàng: " + ex.Message);
                return StatusCode(500, "Đã có lỗi xảy ra khi thêm sản phẩm vào giỏ hàng.");
            }
        }



        [HttpPut("update/{gioHangChiTietId}")]
        public async Task<IActionResult> UpdateSoLuong(Guid gioHangChiTietId, [FromBody] int soLuong)
        {
            var result = await _repo.UpdateSoLuongAsync(gioHangChiTietId, soLuong);
            return Ok(result);
        }

        [HttpDelete("delete/{gioHangChiTietId}")]
        public async Task<IActionResult> Delete(Guid gioHangChiTietId)
        {
            var result = await _repo.RemoveSanPhamKhoiGioAsync(gioHangChiTietId);
            return Ok(new { success = result });
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

            if (voucher.NgayBatDau > DateTime.Now)
            {
                Console.WriteLine("❌ Voucher chưa bắt đầu.");
                return BadRequest("Voucher chưa được áp dụng.");
            }

            if (voucher.NgayKetThuc < DateTime.Now)
            {
                Console.WriteLine("❌ Voucher đã hết hạn.");
                return BadRequest("Voucher đã hết hạn.");
            }

            if (voucher.SoLuong <= 0)
            {
                Console.WriteLine("❌ Voucher đã hết lượt sử dụng.");
                return BadRequest("Voucher đã hết lượt sử dụng.");
            }

            var tongTien = gioHang.GioHangChiTiets.Sum(ct => ct.ThanhTien);
            var phanTramGiam = (decimal)voucher.PhanTramGiam / 100m;
            var soTienGiam = tongTien * phanTramGiam;

            // Áp dụng giới hạn giảm tối đa (nếu có)
            if (voucher.GiaTriGiamToiDa.HasValue && soTienGiam > voucher.GiaTriGiamToiDa.Value)
            {
                soTienGiam = voucher.GiaTriGiamToiDa.Value;
            }

            var tongTienSauGiam = tongTien - soTienGiam;

            Console.WriteLine($"Tổng tiền: {tongTien}");
            Console.WriteLine($"Phần trăm giảm: {voucher.PhanTramGiam}"); // phải là 20
            Console.WriteLine($"Số tiền giảm: {soTienGiam}");
            Console.WriteLine($"Tổng sau giảm: {tongTienSauGiam}");


            return Ok(new
            {
                TongTien = tongTien,
                GiamGia = soTienGiam,
                TienSauGiam = tongTienSauGiam,
                PhanTramGiam = voucher.PhanTramGiam,
                TenVoucher = voucher.TenVoucher
            });
        }

        [HttpPost("thanh-toan")]
        public async Task<IActionResult> ThanhToan([FromBody] ThanhToanDTO dto)
        {
            var result = await _repo.ThanhToanAsync(dto);
            Console.WriteLine($"[Controller] Kết quả thanh toán: {System.Text.Json.JsonSerializer.Serialize(result)}");
            return Ok(result);
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
