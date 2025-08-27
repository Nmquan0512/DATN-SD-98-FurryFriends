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
using FurryFriends.Web.Filter;

namespace FurryFriends.Web.Controllers
{
    [ServiceFilter(typeof(AccountStatusFilter))]
    public class GioHangsController : Controller
    {
        private readonly IGioHangService _gioHangService;
        private readonly IVoucherService _voucherService;
        private readonly IKhachHangService _khachHangService;
        private readonly IHinhThucThanhToanService _hinhThucThanhToanService;
        private readonly IDiaChiKhachHangService _diaChiKhachHangService;
        private readonly IVnPayService _vnPayService;
        private readonly IEmailNotificationService _emailNotificationService;
        private readonly ILogger<GioHangsController> _logger;
        private readonly ISanPhamChiTietService _sanPhamChiTietService;
        private readonly ISanPhamService _sanPhamService;

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
            ILogger<GioHangsController> logger,
            ISanPhamChiTietService sanPhamChiTietService,
            IEmailNotificationService emailNotificationService,
            ISanPhamService sanPhamService)
        {
            _gioHangService = gioHangService;
            _voucherService = voucherService;
            _khachHangService = khachHangService;
            _hinhThucThanhToanService = hinhThucThanhToanService;
            _diaChiKhachHangService = diaChiKhachHangService;
            _vnPayService = vnPayService;
            _emailNotificationService = emailNotificationService;
            _logger = logger;
            _sanPhamChiTietService = sanPhamChiTietService;
            _sanPhamService = sanPhamService;
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

                Guid khachHangId;
                try
                {
                    khachHangId = GetKhachHangId();
                }
                catch (InvalidOperationException)
                {
                    // Nếu là AJAX (fetch), trả về JSON với thông báo cần đăng nhập
                    if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                    {
                        return StatusCode(401, new { success = false, message = "Vui lòng đăng nhập để thêm sản phẩm vào giỏ hàng", redirectToLogin = true });
                    }
                    // Ngược lại điều hướng đến trang đăng nhập
                    return Redirect("/KhachHangLogin/DangNhap");
                }

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
                    return StatusCode(500, new { success = false, message = ex.Message }); // ✅
                }
                ViewBag.ErrorMessage = ex.Message;
                return RedirectToAction("Index", "SanPhamKhachHang");
            }
        }


        [HttpPost]
        public async Task<IActionResult> UpdateQuantity(Guid chiTietId, int soLuong, Guid? voucherId)
        {
            var result = await _gioHangService.UpdateSoLuongAsync(chiTietId, soLuong);

    if (!result.Success)
    {
        TempData["ErrorMessage"] = result.Message;
        return RedirectToAction("Index", new { voucherId });
    }

    TempData["SuccessMessage"] = result.Message;
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
            // ✅ Kiểm tra dto có null không
            if (dto == null)
            {
                TempData["Loi"] = "😔 Có lỗi xảy ra: Dữ liệu thanh toán không hợp lệ. Vui lòng thử lại.";
                return RedirectToAction("Index", "GioHangs");
            }

            // ✅ Kiểm tra KhachHangId có hợp lệ không
            if (dto.KhachHangId == Guid.Empty)
            {
                TempData["Loi"] = "😔 Có lỗi xảy ra: Không tìm thấy thông tin khách hàng. Vui lòng đăng nhập lại.";
                return RedirectToAction("Index", "GioHangs");
            }

            // ✅ Ngăn chặn double order: Kiểm tra session
            var sessionKey = $"ThanhToan_{dto.KhachHangId}";
            var lastThanhToanTime = HttpContext.Session.GetString(sessionKey);
            
            if (!string.IsNullOrEmpty(lastThanhToanTime) && 
                DateTime.TryParse(lastThanhToanTime, out var lastTime))
            {
                var timeDiff = DateTime.Now - lastTime;
                if (timeDiff.TotalSeconds < 30) // 30 giây
                {
                    TempData["Loi"] = $"😔 Rất tiếc! Bạn vừa thực hiện thanh toán cách đây {timeDiff.TotalSeconds:F0} giây. Vui lòng chờ một chút trước khi thử lại.";
                    return RedirectToAction("Index", "GioHangs");
                }
            }
            
            // ✅ Lưu thời gian thanh toán vào session
            HttpContext.Session.SetString(sessionKey, DateTime.Now.ToString("O"));

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
            
            // ✅ Kiểm tra hình thức thanh toán có hợp lệ không
            var allowedPaymentMethodIds = new[]
            {
                Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), // Thanh toán khi nhận hàng
                Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb")  // Thanh toán VNPay
            };
            
            if (!allowedPaymentMethodIds.Contains(dto.HinhThucThanhToanId))
            {
                ViewBag.HinhThucThanhToanList = await _hinhThucThanhToanService.GetAllAsync();
                ModelState.AddModelError("HinhThucThanhToanId", "Chỉ hỗ trợ thanh toán khi nhận hàng hoặc thanh toán VNPay!");
                return View(dto);
            }
            // 🔍 Kiểm tra voucher nếu có
            if (dto.VoucherId.HasValue && dto.VoucherId != Guid.Empty)
            {
                try
                {
                    var voucher = await _voucherService.GetByIdAsync(dto.VoucherId.Value);
                    if (voucher == null)
                    {
                        TempData["Loi"] = "Voucher không tồn tại hoặc đã bị xóa.";
                        return RedirectToAction("Index", "GioHangs");
                    }

                    if (voucher.TrangThai == 0)
                    {
                        TempData["Loi"] = "Voucher đang ở trạng thái không hoạt động.";
                        return RedirectToAction("Index", "GioHangs");
                    }
                    if (voucher.NgayKetThuc < DateTime.Now)
                    {
                        TempData["Loi"] = "Voucher đã hết hạn sử dụng.";
                        return RedirectToAction("Index", "GioHangs");
                    }
                    if (voucher.SoLuong <= 0)
                    {
                        TempData["Loi"] = "Voucher đã hết số lượng.";
                        return RedirectToAction("Index", "GioHangs");
                    }
                }
                catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    TempData["Loi"] = "Voucher không tồn tại hoặc đã bị xóa.";
                    return RedirectToAction("Index", "GioHangs");
                }
            }

            var gioHang = await _gioHangService.GetGioHangAsync(dto.KhachHangId);

            // ✅ Kiểm tra gioHang có null không
            if (gioHang == null)
            {
                TempData["Loi"] = "😔 Có lỗi xảy ra: Không thể tải thông tin giỏ hàng. Vui lòng thử lại.";
                return RedirectToAction("Index", "GioHangs");
            }

            // ✅ Kiểm tra giỏ hàng có sản phẩm không
            if (gioHang.GioHangChiTiets == null || !gioHang.GioHangChiTiets.Any())
            {
                TempData["Loi"] = "Giỏ hàng của bạn đang trống.";
                return RedirectToAction("Index", "GioHangs");
            }

            foreach (var item in gioHang.GioHangChiTiets)
            {
                var spct = await _sanPhamChiTietService.GetByIdAsync(item.SanPhamChiTietId);
                if (spct == null)
                {
                    TempData["Loi"] = "Sản phẩm không tồn tại hoặc đã bị xóa.";
                    return RedirectToAction("Index", "GioHangs");
                }

                var sanPham = await _sanPhamService.GetByIdAsync(spct.SanPhamId);
                if (sanPham == null)
                {
                    TempData["Loi"] = "Sản phẩm không tồn tại hoặc đã bị xóa.";
                    return RedirectToAction("Index", "GioHangs");
                }

                if (spct.TrangThai == 0 || sanPham.TrangThai == false)
                {
                    TempData["Loi"] = $"Sản phẩm {sanPham.TenSanPham ?? "N/A"} hiện không còn hoạt động.";
                    return RedirectToAction("Index", "GioHangs");
                }

                if (spct.SoLuong < item.SoLuong)
                {
                    // ✅ Thông báo lỗi thân thiện hơn
                    var tenSanPham = sanPham.TenSanPham ?? "N/A";
                    var soLuongHienTai = spct.SoLuong;
                    var soLuongCanMua = item.SoLuong;
                    
                    if (soLuongHienTai == 0)
                    {
                        TempData["Loi"] = $"😔 Rất tiếc! Sản phẩm \"{tenSanPham}\" đã hết hàng. Vui lòng xóa khỏi giỏ hàng hoặc chọn sản phẩm khác.";
                    }
                    else
                    {
                        TempData["Loi"] = $"😔 Rất tiếc! Sản phẩm \"{tenSanPham}\" chỉ còn {soLuongHienTai} sản phẩm trong kho, nhưng bạn muốn mua {soLuongCanMua} sản phẩm. Vui lòng giảm số lượng hoặc chọn sản phẩm khác.";
                    }
                    return RedirectToAction("Index", "GioHangs");
                }
            }


            // Bổ sung: nếu VoucherId không bind được từ form, thử lấy từ form/query thủ công
            if (!dto.VoucherId.HasValue)
            {
                try
                {
                    // ✅ Kiểm tra Request.Form có tồn tại không
                    if (Request?.Form != null && Request.Form.ContainsKey("VoucherId"))
                    {
                        var vFromForm = Request.Form["VoucherId"].FirstOrDefault();
                        if (Guid.TryParse(vFromForm, out var vid))
                        {
                            dto.VoucherId = vid;
                        }
                    }
                    
                    // ✅ Nếu không có trong form, thử lấy từ query
                    if (!dto.VoucherId.HasValue && Request?.Query != null && Request.Query.ContainsKey("voucherId"))
                    {
                        var vFromQuery = Request.Query["voucherId"].FirstOrDefault();
                        if (Guid.TryParse(vFromQuery, out var vid2))
                        {
                            dto.VoucherId = vid2;
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning($"Không thể lấy VoucherId từ form/query: {ex.Message}");
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
                TempData["Loi"] = $"Không thể đặt hàng vì tổng tiền vượt quá {MAX_ORDER_AMOUNT:N0} VNĐ. Tổng tiền hiện tại: {tongTien:N0} VNĐ.";
                return RedirectToAction("Index", "GioHangs");
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

            if (!isVnPay)
            {
                var soDonChoDuyet = await _gioHangService.GetDonChoDuyetCountAsync(dto.KhachHangId);
                if (soDonChoDuyet >= 5)
                {
                    ViewBag.HinhThucThanhToanList = await _hinhThucThanhToanService.GetAllAsync();
                    TempData["Loi"] = "Bạn đã có 5 đơn hàng ở trạng thái 'Chờ duyệt', vui lòng chờ xử lý xong trước khi đặt thêm!";
                    return RedirectToAction("Index", "GioHangs"); // ✅ Giống voucher/sp
                }
            }

            // Nếu không phải VNPay, xử lý thanh toán thông thường
            try
            {
                _logger.LogInformation($"🔍 Debug - About to call ThanhToanAsync with KhachHangId: {dto.KhachHangId}");
                _logger.LogInformation($"🔍 Debug - DTO properties: HinhThucThanhToanId={dto.HinhThucThanhToanId}, VoucherId={dto.VoucherId}");
                
                // ✅ Kiểm tra dto trước khi gọi API
                if (dto == null)
                {
                    _logger.LogError("DTO is null before calling ThanhToanAsync");
                    TempData["Loi"] = "😔 Có lỗi xảy ra: Dữ liệu thanh toán không hợp lệ. Vui lòng thử lại.";
                    return RedirectToAction("Index", "GioHangs");
                }
                
                var result = await _gioHangService.ThanhToanAsync(dto);
                
                _logger.LogInformation($"🔍 Debug - ThanhToanAsync completed, result type: {result?.GetType().Name ?? "NULL"}");
                
                // ✅ Kiểm tra kết quả có null không
                if (result == null)
                {
                    _logger.LogError("ThanhToanAsync trả về null");
                    TempData["Loi"] = "Có lỗi xảy ra khi xử lý thanh toán. Vui lòng thử lại.";
                    return RedirectToAction("Index", "GioHangs");
                }
                
                // ✅ Gửi thông báo email cho admin khi đặt hàng thành công
                try
                {
                    // ✅ Sử dụng dynamic để truy cập HoaDonId từ object
                    dynamic resultDynamic = result;
                    var hoaDonId = resultDynamic?.HoaDonId;
                    
                    if (hoaDonId != null)
                    {
                        // ✅ Gửi thông báo email cho admin khi đặt hàng thành công
                        await _emailNotificationService.SendOrderNotificationToAdminAsync(result);
                        _logger.LogInformation($"✅ Order created successfully: {hoaDonId}");
                    }
                    else
                    {
                        _logger.LogWarning("Không thể lấy HoaDonId từ kết quả thanh toán");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError($"❌ Error sending admin notification: {ex.Message}");
                    // Không throw exception để không ảnh hưởng đến luồng thanh toán
                }
                
                return View("KetQuaThanhToan", result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi xử lý thanh toán");
                
                // ✅ Xóa session key để tránh bị khóa vĩnh viễn khi có lỗi
                try
                {
                    HttpContext.Session.Remove(sessionKey);
                }
                catch
                {
                    // Ignore session removal errors
                }
                
                // ✅ Cải thiện thông báo lỗi thân thiện hơn
                string errorMessage;
                if (ex.Message.Contains("Rất tiếc!"))
                {
                    // Sử dụng thông báo lỗi đã được cải thiện từ API
                    errorMessage = ex.Message;
                }
                else if (ex.Message.Contains("vừa tạo đơn hàng") || ex.Message.Contains("cách đây"))
                {
                    // ✅ Thông báo lỗi double order
                    errorMessage = ex.Message;
                }
                else if (ex.Message.Contains("không đủ số lượng"))
                {
                    errorMessage = "😔 Rất tiếc! Sản phẩm trong giỏ hàng của bạn hiện không đủ số lượng để mua. Có thể có người khác vừa mua sản phẩm này. Vui lòng kiểm tra lại giỏ hàng.";
                }
                else if (ex.Message.Contains("Voucher") || ex.Message.Contains("voucher"))
                {
                    // ✅ Cải thiện thông báo lỗi voucher
                    if (ex.Message.Contains("hết lượt sử dụng"))
                    {
                        errorMessage = "😔 Rất tiếc! Voucher bạn đang sử dụng đã hết lượt. Có thể có người khác vừa sử dụng voucher này. Vui lòng thử voucher khác hoặc thanh toán không sử dụng voucher.";
                    }
                    else if (ex.Message.Contains("hết hạn"))
                    {
                        errorMessage = "😔 Rất tiếc! Voucher bạn đang sử dụng đã hết hạn. Vui lòng chọn voucher khác hoặc thanh toán không sử dụng voucher.";
                    }
                    else if (ex.Message.Contains("không đủ điều kiện"))
                    {
                        errorMessage = "😔 Rất tiếc! Voucher không đủ điều kiện áp dụng cho đơn hàng này. Vui lòng kiểm tra điều kiện sử dụng voucher hoặc thanh toán không sử dụng voucher.";
                    }
                    else
                    {
                        errorMessage = "😔 Rất tiếc! Có vấn đề với voucher bạn đang sử dụng. Vui lòng kiểm tra lại voucher hoặc thử thanh toán không sử dụng voucher.";
                    }
                }
                else
                {
                    errorMessage = "😔 Có lỗi xảy ra khi xử lý thanh toán. Vui lòng thử lại hoặc liên hệ hỗ trợ nếu vấn đề vẫn tiếp tục.";
                }
                
                TempData["Loi"] = errorMessage;
                return RedirectToAction("Index", "GioHangs");
            }
        }

        public IActionResult CreatePaymentUrlVnpay(PaymentInformationModel model)
        {
            var url = _vnPayService.CreatePaymentUrl(model, HttpContext);
            return Redirect(url);
        }

        [HttpGet]
        public async Task<IActionResult> PaymentCallbackVnpay()
        {
            try
            {
                var response = _vnPayService.PaymentExecute(Request.Query);

                if (response.Success)
                {
                    // Lấy lại DTO từ Session
                    var dtoJson = HttpContext.Session.GetString("ThanhToanDTO");
                    if (string.IsNullOrEmpty(dtoJson))
                    {
                        TempData["Loi"] = "😔 Có lỗi xảy ra: Không tìm thấy thông tin thanh toán. Vui lòng thử lại.";
                        return RedirectToAction("Index", "GioHangs");
                    }

                    var dto = System.Text.Json.JsonSerializer.Deserialize<ThanhToanDTO>(dtoJson);
                    if (dto == null)
                    {
                        TempData["Loi"] = "😔 Có lỗi xảy ra: Dữ liệu thanh toán không hợp lệ. Vui lòng thử lại.";
                        return RedirectToAction("Index", "GioHangs");
                    }

                    // ✅ Kiểm tra KhachHangId có hợp lệ không
                    if (dto.KhachHangId == Guid.Empty)
                    {
                        TempData["Loi"] = "😔 Có lỗi xảy ra: Không tìm thấy thông tin khách hàng. Vui lòng đăng nhập lại.";
                        return RedirectToAction("Index", "GioHangs");
                    }

                    // ✅ Ngăn chặn double order cho VNPay callback
                    var sessionKey = $"VNPayThanhToan_{dto.KhachHangId}";
                    var lastVNPayTime = HttpContext.Session.GetString(sessionKey);
                    
                    if (!string.IsNullOrEmpty(lastVNPayTime) && 
                        DateTime.TryParse(lastVNPayTime, out var lastTime))
                    {
                        var timeDiff = DateTime.Now - lastTime;
                        if (timeDiff.TotalSeconds < 30) // 30 giây
                        {
                            TempData["Loi"] = $"😔 Rất tiếc! Bạn vừa thực hiện thanh toán VNPay cách đây {timeDiff.TotalSeconds:F0} giây. Vui lòng chờ một chút trước khi thử lại.";
                            return RedirectToAction("Index", "GioHangs");
                        }
                    }
                    
                    // ✅ Lưu thời gian thanh toán VNPay vào session
                    HttpContext.Session.SetString(sessionKey, DateTime.Now.ToString("O"));

                    var result = await _gioHangService.ThanhToanAsync(dto);
                    
                    // ✅ Gửi thông báo email cho admin khi thanh toán VNPay thành công
                    try
                    {
                        await _emailNotificationService.SendOrderNotificationToAdminAsync(result);
                        _logger.LogInformation($"✅ Admin notification sent for VNPay order {result.HoaDonId}");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError($"❌ Error sending admin notification for VNPay: {ex.Message}");
                        // Không throw exception để không ảnh hưởng đến luồng thanh toán
                    }
                    
                    HttpContext.Session.Remove("ThanhToanDTO"); // Xóa sau khi xử lý
                    return View("KetQuaThanhToan", result);
                }

                return View("ThanhToanThatBai", response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi trong PaymentCallbackVnpay");
                TempData["Loi"] = "😔 Có lỗi xảy ra khi xử lý thanh toán VNPay. Vui lòng thử lại.";
                return RedirectToAction("Index", "GioHangs");
            }
        }
    }
}
