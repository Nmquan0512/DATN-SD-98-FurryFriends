using FurryFriends.Web.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using FurryFriends.API.Models;
using FurryFriends.Web.Services.IService;
using FurryFriends.API.Models.DTO;
using FurryFriends.Web.ViewModels;
using FurryFriends.Web.Services;

namespace FurryFriends.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IHoaDonService _hoaDonService;
        private readonly ISanPhamService _sanPhamService;
        private readonly IThuongHieuService _thuongHieuService;
        private readonly IAnhService _anhService;
        private readonly ISanPhamChiTietService _sanPhamChiTietService;
        private readonly DiscountCalculationService _discountCalculationService;

        public HomeController(
            ILogger<HomeController> logger, 
            IHoaDonService hoaDonService,
            ISanPhamService sanPhamService,
            IThuongHieuService thuongHieuService,
            IAnhService anhService,
            ISanPhamChiTietService sanPhamChiTietService,
            DiscountCalculationService discountCalculationService)
        {
            _logger = logger;
            _hoaDonService = hoaDonService;
            _sanPhamService = sanPhamService;
            _thuongHieuService = thuongHieuService;
            _anhService = anhService;
            _sanPhamChiTietService = sanPhamChiTietService;
            _discountCalculationService = discountCalculationService;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                // Sử dụng cùng cấu trúc như SanPhamKhachHang
                var danhSachSanPhamDTO = await _sanPhamService.GetAllAsync();
                var viewModelList = new List<SanPhamViewModel>();

                // Lấy toàn bộ danh sách chi tiết một lần để tránh gọi API nhiều lần
                var allChiTietListDTO = await _sanPhamChiTietService.GetAllAsync();

                foreach (var sp in danhSachSanPhamDTO.Take(4)) // Chỉ lấy 4 sản phẩm
                {
                    var chiTietListDTO = allChiTietListDTO
                                            .Where(ct => ct.SanPhamId == sp.SanPhamId)
                                            .ToList();

                    string? anhDaiDien = chiTietListDTO
                                            .FirstOrDefault(ct => !string.IsNullOrEmpty(ct.DuongDan))
                                            ?.DuongDan;

                    // Chuyển sang ViewModel với thông tin giảm giá
                    var chiTietVMs = chiTietListDTO.Select(ct => new SanPhamChiTietViewModel
                    {
                        SanPhamChiTietId = ct.SanPhamChiTietId,
                        MauSac = ct.TenMau ?? "",
                        KichCo = ct.TenKichCo ?? "",
                        SoLuongTon = ct.SoLuong,
                        GiaBan = ct.Gia,
                        DanhSachAnh = ct.DuongDan != null ? new List<string> { ct.DuongDan } : new List<string>(),
                        
                        // Thông tin giảm giá sẽ được tính toán sau
                        CoGiamGia = false,
                        PhanTramGiamGia = null,
                        GiaSauGiam = null
                    }).ToList();

                    var sanPhamVM = new SanPhamViewModel
                    {
                        SanPhamId = sp.SanPhamId,
                        TenSanPham = sp.TenSanPham,
                        MoTa = sp.TenThuongHieu ?? "", // Sử dụng tên thương hiệu làm mô tả
                        TrangThai = sp.TrangThai,
                        AnhDaiDienUrl = anhDaiDien,
                        GiaBan = chiTietListDTO.FirstOrDefault()?.Gia ?? 0,
                        SoLuongTon = chiTietListDTO.FirstOrDefault()?.SoLuong ?? 0,
                        
                        // Thông tin thương hiệu
                        TenThuongHieu = sp.TenThuongHieu,
                        ThuongHieuId = sp.ThuongHieuId,
                        
                        ChiTietList = chiTietVMs
                    };

                    // Áp dụng logic giảm giá với % cao nhất
                    sanPhamVM = await _discountCalculationService.UpdateProductDiscount(sanPhamVM);
                    viewModelList.Add(sanPhamVM);
                }

                var totalProducts = await _sanPhamService.GetTotalProductsAsync();
                var totalOrders = 0; // Placeholder for now
                
                ViewBag.FeaturedProducts = viewModelList;
                ViewBag.TotalProducts = totalProducts;
                ViewBag.TotalOrders = totalOrders;
                
                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading homepage data");
                // Return view with empty data if there's an error
                ViewBag.FeaturedProducts = new List<SanPhamViewModel>();
                ViewBag.TotalProducts = 0;
                ViewBag.TotalOrders = 0;
            return View();
            }
        }

        public IActionResult Privacy()
        {
            return View();
        }

        // ✅ Trang Giới thiệu
        public IActionResult About()
        {
            return View();
        }

        // ✅ Trang Liên hệ
        public IActionResult Contact()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpPost]
        [HttpPost]
        public async Task<IActionResult> ChatGemini([FromBody] ChatRequest req)
        {
            string userMsg = req?.Message?.Trim() ?? "";
            string answer = "";

            try
            {
                // Patterns
                const string guidPattern = @"\b[0-9a-fA-F]{8}(?:-[0-9a-fA-F]{4}){3}-[0-9a-fA-F]{12}\b";
                const string shortPattern = @"\b[0-9a-fA-F]{8}\b";

                // Detect if user mentioned 'đơn hàng' hoặc 'hóa đơn'
                bool containsDonHang = Regex.IsMatch(userMsg, @"\bđ[oơ]n ?h[aà]ng\b", RegexOptions.IgnoreCase);
                bool containsHoaDon = Regex.IsMatch(userMsg, @"\bh[oó]a ?đ[oơ]n\b", RegexOptions.IgnoreCase);

                // Try to extract code: ưu tiên GUID, nếu không có thì 8 ký tự
                string? code = null;
                if (containsDonHang || containsHoaDon)
                {
                    var mGuid = Regex.Match(userMsg, guidPattern, RegexOptions.IgnoreCase);
                    if (mGuid.Success) code = mGuid.Value;
                    else
                    {
                        var mShort = Regex.Match(userMsg, shortPattern, RegexOptions.IgnoreCase);
                        if (mShort.Success) code = mShort.Value;
                    }
                }

                // Fallback: người dùng chỉ gửi mã (không có từ 'đơn hàng'/'hóa đơn')
                if (code == null)
                {
                    var mGuid2 = Regex.Match(userMsg, guidPattern, RegexOptions.IgnoreCase);
                    if (mGuid2.Success) { code = mGuid2.Value; containsDonHang = true; }
                    else
                    {
                        var mShort2 = Regex.Match(userMsg, shortPattern, RegexOptions.IgnoreCase);
                        if (mShort2.Success) { code = mShort2.Value; containsDonHang = true; }
                    }
                }

                // Nếu tìm được mã (GUID hoặc 8 ký tự) => xử lý tra cứu đơn/hóa đơn
                if (!string.IsNullOrEmpty(code))
                {
                    try
                    {
                        HoaDon hoaDon = null;
                        if (Guid.TryParse(code, out Guid parsedGuid))
                        {
                            hoaDon = await _hoaDonService.GetHoaDonByIdAsync(parsedGuid);
                        }
                        else if (code.Length == 8)
                        {
                            var all = await _hoaDonService.GetHoaDonListAsync();
                            // Dùng StartsWith để match prefix (an toàn hơn Substring)
                            hoaDon = all.FirstOrDefault(h => h.HoaDonId.ToString().StartsWith(code, StringComparison.OrdinalIgnoreCase));
                        }

                        if (hoaDon == null)
                        {
                            answer = $"Không tìm thấy {(containsHoaDon ? "hóa đơn" : "đơn hàng")} với mã {code}.";
                            return Json(new { answer });
                        }

                        // Build HTML trả về (giống nhau cho GUID và 8 ký tự)
                        string[] trangThaiArr = { "Chờ xác nhận", "Đang xử lý", "Đang giao hàng", "Đã giao hàng", "Đã hủy" };
                        string trangThai = hoaDon.TrangThai >= 0 && hoaDon.TrangThai < trangThaiArr.Length ? trangThaiArr[hoaDon.TrangThai] : "Không xác định";
                        string payment = hoaDon.HinhThucThanhToan?.TenHinhThuc ?? "Không rõ";

                        var sb = new StringBuilder();
                        sb.Append($@"<div><h3>Thông tin {(containsHoaDon ? "hóa đơn" : "đơn hàng")} #{hoaDon.HoaDonId.ToString().Substring(0, 8).ToUpper()}</h3><ul>");
                        sb.Append($"<li><b>Ngày đặt:</b> {hoaDon.NgayTao:dd/MM/yyyy HH:mm}</li>");
                        sb.Append($"<li><b>Khách hàng:</b> {hoaDon.TenCuaKhachHang}</li>");
                        if (!string.IsNullOrEmpty(hoaDon.EmailCuaKhachHang)) sb.Append($"<li><b>Email:</b> {hoaDon.EmailCuaKhachHang}</li>");
                        if (!string.IsNullOrEmpty(hoaDon.SdtCuaKhachHang)) sb.Append($"<li><b>Số điện thoại:</b> {hoaDon.SdtCuaKhachHang}</li>");
                        sb.Append($"<li><b>Tổng tiền:</b> {hoaDon.TongTienSauKhiGiam:N0}đ</li>");
                        sb.Append($"<li><b>Trạng thái:</b> {trangThai}</li>");
                        sb.Append($"<li><b>Phương thức thanh toán:</b> {payment}</li>");

                        // Lấy chi tiết đơn/hóa đơn
                        var chiTietHoaDon = await _hoaDonService.GetChiTietHoaDonAsync(hoaDon.HoaDonId);
                        _logger.LogInformation($"Retrieved {chiTietHoaDon?.Count() ?? 0} chi tiết hóa đơn for order {hoaDon.HoaDonId}");

                        if (chiTietHoaDon != null && chiTietHoaDon.Any())
                        {
                            sb.Append("<li><b>Sản phẩm đã đặt:</b><ul>");
                            int shown = 0;
                            var baseUrl = $"{Request.Scheme}://{Request.Host}";
                            foreach (var ct in chiTietHoaDon)
                            {
                                if (shown++ >= 5) break; // hiển thị tối đa 5 sản phẩm
                                sb.Append("<li>");
                                sb.Append($"<b>{ct.TenSanPhamLucMua ?? "Không xác định"}</b><br>");
                                if (!string.IsNullOrEmpty(ct.KichCoLucMua)) sb.Append($"• Kích cỡ: {ct.KichCoLucMua}<br>");
                                if (!string.IsNullOrEmpty(ct.MauSacLucMua)) sb.Append($"• Màu sắc: {ct.MauSacLucMua}<br>");
                                if (!string.IsNullOrEmpty(ct.ChatLieuLucMua)) sb.Append($"• Chất liệu: {ct.ChatLieuLucMua}<br>");
                                if (!string.IsNullOrEmpty(ct.ThanhPhanLucMua)) sb.Append($"• Thành phần: {ct.ThanhPhanLucMua}<br>");
                                if (!string.IsNullOrEmpty(ct.ThuongHieuLucMua)) sb.Append($"• Thương hiệu: {ct.ThuongHieuLucMua}<br>");
                                if (!string.IsNullOrEmpty(ct.MoTaSanPhamLucMua)) sb.Append($"• Mô tả: {ct.MoTaSanPhamLucMua}<br>");
                                sb.Append($"• Số lượng: {ct.SoLuongSanPham}<br>");
                                sb.Append($"• Giá: {ct.Gia:N0}đ<br>");
                                if (!string.IsNullOrEmpty(ct.AnhSanPhamLucMua))
                                {
                                    var imageUrl = $"https://localhost:7289/{ct.AnhSanPhamLucMua}";
                                    sb.Append($"• <img src='{imageUrl}' style='max-width:80px;height:auto;margin-top:5px;'/>");
                                }
                                sb.Append("</li>");
                            }
                            if (chiTietHoaDon.Count() > 5) sb.Append($"<li>... và {chiTietHoaDon.Count() - 5} sản phẩm khác</li>");
                            sb.Append("</ul></li>");
                        }
                        else
                        {
                            sb.Append("<li><b>Sản phẩm:</b> Không có thông tin chi tiết</li>");
                        }

                        sb.Append("</ul></div>");
                        answer = sb.ToString();
                        return Json(new { answer });
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error searching for order");
                        answer = "Có lỗi xảy ra khi tìm kiếm đơn hàng.";
                        return Json(new { answer });
                    }
                }


                // 4. Nếu user chỉ nói "đơn hàng" / "hóa đơn" / "sản phẩm" mà không cung cấp mã/tên
                var containsSanPham = Regex.IsMatch(userMsg, @"s[aả]n ?ph[aẩ]m", RegexOptions.IgnoreCase);
                if (containsHoaDon || containsDonHang || containsSanPham)
                {
                    if (containsHoaDon)
                        answer = "Vui lòng ghi mã hóa đơn để tra cứu (ví dụ: mã hóa đơn là 3FA85F64).";
                    else if (containsDonHang)
                        answer = "Vui lòng ghi mã đơn hàng để tra cứu (ví dụ: đơn hàng 3FA85F64).";
                    else if (containsSanPham)
                        answer = "Vui lòng ghi tên sản phẩm để tra cứu (ví dụ: tên sản phẩm là thức ăn mèo).";
                    return Json(new { answer });
                }

                // 5. Fallback: gọi Gemini (nguyên bản)
                string prompt = $"Bạn là một trợ lý bán hàng cho website bán đồ dùng cho thú cưng tên là FurryFriends chuyên nghiệp, thân thiện, trả lời ngắn gọn, dễ hiểu, ưu tiên trả lời đúng nghiệp vụ bán hàng. Nếu câu hỏi của khách hàng liên quan đến hóa đơn, đơn hàng hoặc sản phẩm thì hãy trả lời theo dữ liệu thực tế (nếu có). Nếu không có dữ liệu, hãy trả lời tự nhiên, không bịa thông tin. Câu hỏi của khách hàng: '{userMsg}'. Luôn trả lời bằng tiếng Việt. Nếu khách hàng hỏi bạn gì liên quan tới tìm hoá đơn, đơn hàng, sản phẩm thì hãy trả lời là hãy viết theo format: mã hoá đơn là + mã, mã đơn hàng là + mã, tên sản phẩm là + tên. Ví dụ: mã hoá đơn là 3FA85F64, đơn hàng 3FA85F64, tên sản phẩm là thức ăn mèo. Không được giới thiệu website bán thú cưng khác ngoài FurryFriends.";

                using var http = new HttpClient();
                var requestBody = new
                {
                    contents = new[]
                    {
                new { parts = new[] { new { text = prompt } } }
            }
                };
                var content = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json");
                var apiKey = "AIzaSyCXRI7hVFop8QLSwLXoGLDthI7nq8vlUI8"; // giữ nguyên như bạn đang có
                var endpoint = "https://generativelanguage.googleapis.com/v1beta/models/gemini-2.0-flash:generateContent?key=" + apiKey;
                var response = await http.PostAsync(endpoint, content);
                var json = await response.Content.ReadAsStringAsync();
                dynamic result = Newtonsoft.Json.JsonConvert.DeserializeObject(json);
                answer = result?.candidates?[0]?.content?.parts?[0]?.text ?? "Xin lỗi, tôi chưa có câu trả lời.";
                return Json(new { answer });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled error in ChatGemini");
                answer = "Có lỗi xảy ra khi xử lý yêu cầu. Vui lòng thử lại sau.";
                return Json(new { answer });
            }
        }

        public class ChatRequest
        {
            public string Message { get; set; }
        }
    }
}

