using FurryFriends.API.Models;
using FurryFriends.API.Models.DTO;
using FurryFriends.API.Models.VNPay;
using FurryFriends.Web.Service.IService;
using FurryFriends.Web.Services;
using FurryFriends.Web.Services.IService;
using FurryFriends.Web.Services.IServices;
using FurryFriends.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Text;

namespace FurryFriends.Web.Controllers
{
    public class GioHangsController : Controller
    {
        private readonly IGioHangService _gioHangService;
        private readonly IVoucherService _voucherService;
        private readonly IKhachHangService _khachHangService;
        private readonly IHinhThucThanhToanService _hinhThucThanhToanService;
        private readonly IDiaChiKhachHangService _diaChiKhachHangService;
        private readonly IVnPayService _vnPayService;
        private readonly ILogger<GioHangsController> _logger;

        private Guid GetKhachHangId()
        {
            var khachHangIdString = HttpContext.Session.GetString("KhachHangId");
            if (string.IsNullOrEmpty(khachHangIdString) || !Guid.TryParse(khachHangIdString, out Guid khachHangId))
            {
                throw new InvalidOperationException("Không tìm thấy thông tin khách hàng.");
            }
            return khachHangId;
        }

        public GioHangsController(
            IGioHangService gioHangService, 
            IVoucherService voucherService, 
            IKhachHangService khachHangService, 
            IHinhThucThanhToanService hinhThucThanhToanService, 
            IDiaChiKhachHangService diaChiKhachHangService,
            IVnPayService vnPayService,
            ILogger<GioHangsController> logger)
        {
            _gioHangService = gioHangService;
            _voucherService = voucherService;
            _khachHangService = khachHangService;
            _hinhThucThanhToanService = hinhThucThanhToanService;
            _diaChiKhachHangService = diaChiKhachHangService;
            _vnPayService = vnPayService;
            _logger = logger;
        }

        public async Task<IActionResult> Index(Guid? voucherId = null)
        {
            Guid khachHangId;
            try
            {
                khachHangId = GetKhachHangId();
            }
            catch
            {
                return Redirect("/KhachHangLogin/DangNhap");
            }

            var gioHang = await _gioHangService.GetGioHangAsync(khachHangId);

            ViewBag.Vouchers = await _voucherService.GetAllAsync(); // Trả về List<VoucherDTO>
            ViewBag.VoucherId = voucherId;
            ViewBag.KhachHangId = khachHangId; // dùng cho link ThanhToan

            if (voucherId.HasValue)
            {
                // Xem trước để biết số tiền giảm thật sự theo API (đã áp dụng trần max)
                var preview = await _gioHangService.PreviewVoucherAsync(khachHangId, voucherId.Value);
                if (preview != null)
                {
                    ViewBag.TienSauGiam = preview.TienSauGiam;
                    ViewBag.GiamGia = preview.GiamGia;
                    ViewBag.PhiVanChuyen = preview.PhiVanChuyen;
                    ViewBag.TongDonHang = preview.TongDonHang;
                    ViewBag.TenVoucher = preview.TenVoucher;
                    ViewBag.MaVoucher = preview.MaVoucher;
                    
                    if (preview.GiamGia <= 0)
                    {
                        TempData["Warning"] = "Voucher không đủ điều kiện hoặc không áp dụng được.";
                        // Xóa voucherId khỏi URL nếu không áp dụng được
                        return RedirectToAction("Index");
                    }
                }
                else
                {
                    TempData["Warning"] = "Voucher không đủ điều kiện hoặc không áp dụng được.";
                    // Xóa voucherId khỏi URL nếu không áp dụng được
                    return RedirectToAction("Index");
                }
            }
            else
            {
                // Không có voucher, tính phí ship mặc định
                var tongTienHang = gioHang?.GioHangChiTiets?.Sum(ct => ct.ThanhTien) ?? 0;
                var phiVanChuyen = tongTienHang >= 500000 ? 0 : 30000;
                ViewBag.PhiVanChuyen = phiVanChuyen;
                ViewBag.GiamGia = 0; // Không có giảm giá
            }
            ViewBag.HinhThucThanhToanList = await _hinhThucThanhToanService.GetAllAsync();
            ViewBag.DiaChis = await _diaChiKhachHangService.GetByKhachHangIdAsync(khachHangId);
            return View(gioHang);
        }

        [HttpPost]
        public async Task<IActionResult> AddToCart(AddToCartViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                    {
                        return BadRequest(new { success = false, message = "Dữ liệu không hợp lệ" });
                    }
                    return RedirectToAction("Index", "SanPhamKhachHang");
                }

                var khachHangId = GetKhachHangId();

                var dto = new AddToCartDTO
                {
                    KhachHangId = khachHangId,
                    SanPhamChiTietId = model.SanPhamChiTietId,
                    SoLuong = model.SoLuong
                };

                await _gioHangService.AddToCartAsync(dto);

                // Nếu là AJAX (fetch), trả về JSON
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return Ok(new { success = true });
                }

                // Ngược lại điều hướng như cũ
                return RedirectToAction("Index", "GioHangs");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Lỗi khi thêm vào giỏ hàng: {ex.Message}");
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return StatusCode(500, new { success = false, message = ex.Message });
                }
                return RedirectToAction("Index", "SanPhamKhachHang");
            }
        }


        [HttpPost]
        public async Task<IActionResult> UpdateQuantity(Guid chiTietId, int soLuong, Guid? voucherId)
        {
            await _gioHangService.UpdateSoLuongAsync(chiTietId, soLuong);
            return RedirectToAction("Index", new { voucherId });
        }

        [HttpPost]
        public async Task<IActionResult> Remove(Guid chiTietId, Guid? voucherId)
        {
            await _gioHangService.RemoveAsync(chiTietId);
            return RedirectToAction("Index", new { voucherId }); // Truyền lại voucherId
        }


        [HttpGet]
        public async Task<IActionResult> ThanhToan(Guid khachHangId, Guid? voucherId)
        {
            if (khachHangId == Guid.Empty)
            {
                try { khachHangId = GetKhachHangId(); }
                catch { return Redirect("/KhachHangLogin/DangNhap"); }
            }
            var khachHang = await _khachHangService.GetByIdAsync(khachHangId);
            if (khachHang == null)
                return NotFound("Không tìm thấy khách hàng.");

            // Lấy danh sách hình thức thanh toán
            var hinhThucThanhToans = await _hinhThucThanhToanService.GetAllAsync();
            ViewBag.HinhThucThanhToanList = new SelectList(hinhThucThanhToans, "HinhThucThanhToanId", "TenHinhThuc");

            // Gửi DTO rỗng ban đầu để bind vào form
            var model = new ThanhToanDTO
            {
                KhachHangId = khachHangId,
                VoucherId = voucherId,
                TaiKhoanId = khachHang.TaiKhoanId ?? Guid.Empty,
                TenCuaKhachHang = khachHang.TenKhachHang,
                SdtCuaKhachHang = khachHang.SDT,
                EmailCuaKhachHang = khachHang.EmailCuaKhachHang,
                LoaiHoaDon = "Online",
                GhiChu = "Hóa đơn Online",
                NhanVienId = null
            };

            return View(model); // Trả về View có form để user chọn
        }

        [HttpPost]
        public async Task<IActionResult> ThanhToan(ThanhToanDTO dto)
        {
            // ✅ Validation: Kiểm tra địa chỉ giao hàng
            if (dto.DiaChiGiaoHangId == Guid.Empty)
            {
                ViewBag.HinhThucThanhToanList = await _hinhThucThanhToanService.GetAllAsync();
                ModelState.AddModelError("DiaChiGiaoHangId", "Vui lòng chọn địa chỉ giao hàng trước khi thanh toán!");
                return View(dto);
            }

            // Validate
            if (dto.HinhThucThanhToanId == Guid.Empty)
            {
                ViewBag.HinhThucThanhToanList = await _hinhThucThanhToanService.GetAllAsync();
                ModelState.AddModelError("HinhThucThanhToanId", "Vui lòng chọn hình thức thanh toán.");
                return View(dto);
            }

            // Bổ sung: nếu VoucherId không bind được từ form, thử lấy từ form/query thủ công
            if (!dto.VoucherId.HasValue)
            {
                var vFromForm = Request.Form["VoucherId"].FirstOrDefault();
                if (Guid.TryParse(vFromForm, out var vid))
                {
                    dto.VoucherId = vid;
                }
                else
                {
                    var vFromQuery = Request.Query["voucherId"].FirstOrDefault();
                    if (Guid.TryParse(vFromQuery, out var vid2))
                    {
                        dto.VoucherId = vid2;
                    }
                }
            }

            // 👉 Lấy thông tin khách hàng từ database
            var khachHang = await _khachHangService.GetByIdAsync(dto.KhachHangId);
            if (khachHang == null)
                return NotFound("Không tìm thấy khách hàng.");

            // 👉 Gán lại dữ liệu cho dto
            dto.TenCuaKhachHang = khachHang.TenKhachHang;
            dto.EmailCuaKhachHang = khachHang.EmailCuaKhachHang;
            dto.SdtCuaKhachHang = khachHang.SDT;
            var taiKhoanIdString = HttpContext.Session.GetString("TaiKhoanId");
            if (taiKhoanIdString == null || !Guid.TryParse(taiKhoanIdString, out Guid taiKhoanId))
            {
                TempData["Loi"] = "Bạn cần đăng nhập để thanh toán.";
                return RedirectToAction("Index", "DangNhap");
            }

            dto.TaiKhoanId = taiKhoanId;

            dto.LoaiHoaDon = "Online";
            dto.GhiChu = "Hóa đơn Online";
            dto.NhanVienId = null; // hoặc gán nhân viên nếu có logic khác

            // Kiểm tra hình thức thanh toán VNPay
            _logger.LogInformation($"HinhThucThanhToanId: {dto.HinhThucThanhToanId}");
            var hinhThuc = await _hinhThucThanhToanService.GetByIdAsync(dto.HinhThucThanhToanId);
            _logger.LogInformation($"HinhThuc: {hinhThuc?.TenHinhThuc ?? "NULL"}");
            _logger.LogInformation($"HinhThuc ID: {hinhThuc?.HinhThucThanhToanId}");
            
            // Kiểm tra nhiều cách gọi tên VNPay
            var isVnPay = hinhThuc != null && (
                hinhThuc.TenHinhThuc.Equals("Thanh toán VNPay", StringComparison.OrdinalIgnoreCase) ||
                hinhThuc.TenHinhThuc.Equals("VNPay", StringComparison.OrdinalIgnoreCase) ||
                hinhThuc.TenHinhThuc.Equals("VNPAY", StringComparison.OrdinalIgnoreCase) ||
                hinhThuc.TenHinhThuc.Contains("VNPay", StringComparison.OrdinalIgnoreCase)
            );
            
            _logger.LogInformation($"Is VNPay: {isVnPay}");
            
            // Lấy tổng tiền từ giỏ hàng để kiểm tra validation
            var gioHang = await _gioHangService.GetGioHangAsync(dto.KhachHangId);
            decimal tongTien = 0;
            if (dto.VoucherId.HasValue && dto.VoucherId != Guid.Empty)
            {
                tongTien = await _gioHangService.TinhTongTienSauVoucher(dto.KhachHangId, dto.VoucherId.Value);
            }
            else
            {
                tongTien = gioHang.GioHangChiTiets.Sum(x => x.ThanhTien);
            }
            
            _logger.LogInformation($"Tổng tiền: {tongTien}");
            
            // Validation: Không cho phép đặt hàng quá 5 triệu
            const decimal MAX_ORDER_AMOUNT = 5000000; // 5 triệu VNĐ
            if (tongTien > MAX_ORDER_AMOUNT)
            {
                ViewBag.HinhThucThanhToanList = await _hinhThucThanhToanService.GetAllAsync();
                ModelState.AddModelError("", $"Không thể đặt hàng với tổng tiền vượt quá {MAX_ORDER_AMOUNT:N0} VNĐ. Tổng tiền hiện tại: {tongTien:N0} VNĐ");
                return View(dto);
            }
            
            if (isVnPay)
            {
                _logger.LogInformation("Đang xử lý thanh toán VNPay...");

                // Lưu tạm DTO vào Session để callback xử lý
                HttpContext.Session.SetString("ThanhToanDTO", System.Text.Json.JsonSerializer.Serialize(dto));

                var paymentModel = new PaymentInformationModel
                {
                    Amount = (double)tongTien,
                    OrderDescription = $"Thanh toán đơn hàng cho {dto.TenCuaKhachHang}",
                    Name = dto.TenCuaKhachHang
                };

                _logger.LogInformation("Đang tạo URL thanh toán VNPay...");
                try
                {
                var url = _vnPayService.CreatePaymentUrl(paymentModel, HttpContext);
                    _logger.LogInformation($"URL VNPay: {url}");
                return Redirect(url);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Lỗi khi tạo URL VNPay");
                    TempData["Loi"] = "Không thể tạo URL thanh toán VNPay. Vui lòng thử lại.";
                    return RedirectToAction("Index");
                }
            }
            else
            {
                _logger.LogInformation($"Không phải VNPay. HinhThuc: {hinhThuc?.TenHinhThuc ?? "NULL"}, ID: {hinhThuc?.HinhThucThanhToanId}");
            }

            // Nếu không phải VNPay, xử lý thanh toán thông thường
            var result = await _gioHangService.ThanhToanAsync(dto);
            return View("KetQuaThanhToan", (ThanhToanResultViewModel)result);
        }

        public IActionResult CreatePaymentUrlVnpay(PaymentInformationModel model)
        {
            var url = _vnPayService.CreatePaymentUrl(model, HttpContext);
            return Redirect(url);
        }

        [HttpGet]
        public async Task<IActionResult> PaymentCallbackVnpay()
        {
            var response = _vnPayService.PaymentExecute(Request.Query);

            if (response.Success)
            {
                // Lấy lại DTO từ Session
                var dtoJson = HttpContext.Session.GetString("ThanhToanDTO");
                if (!string.IsNullOrEmpty(dtoJson))
                {
                    var dto = System.Text.Json.JsonSerializer.Deserialize<ThanhToanDTO>(dtoJson);
                    if (dto != null)
                    {
                        var result = await _gioHangService.ThanhToanAsync(dto);
                        HttpContext.Session.Remove("ThanhToanDTO"); // Xóa sau khi xử lý
                        return View("KetQuaThanhToan", (ThanhToanResultViewModel)result);
                    }
                }
            }

            return View("ThanhToanThatBai", response);
        }
    }
}
