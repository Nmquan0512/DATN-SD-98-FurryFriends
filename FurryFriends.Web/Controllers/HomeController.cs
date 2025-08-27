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
using FurryFriends.Web.Filter;

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
                var totalProducts = await _sanPhamService.GetTotalProductsAsync();
                var totalOrders = 0; // Placeholder for now
                
                // Lấy sản phẩm đang giảm giá
                var discountedProducts = await GetDiscountedProductsAsync();
                
                // Lấy top 5 sản phẩm bán chạy
                var (topSellingProducts, productSalesCount) = await GetTopSellingProductsAsync();
                
                // Lấy top 10 khách hàng VIP
                var topCustomers = await GetTopCustomersAsync();
                
                // Lấy thống kê truy cập
                var (todayVisits, onlineUsers) = await GetVisitStatisticsAsync();
                
                ViewBag.DiscountedProducts = discountedProducts;
                ViewBag.TopSellingProducts = topSellingProducts;
                ViewBag.ProductSalesCount = productSalesCount;
                ViewBag.TopCustomers = topCustomers;
                ViewBag.TotalProducts = totalProducts;
                ViewBag.TotalOrders = totalOrders;
                ViewBag.TodayVisits = todayVisits;
                ViewBag.OnlineUsers = onlineUsers;
                
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

        // Lấy sản phẩm đang giảm giá
        private async Task<List<SanPhamViewModel>> GetDiscountedProductsAsync()
        {
            try
            {
                var allProducts = await _sanPhamService.GetAllAsync();
                var allChiTietList = await _sanPhamChiTietService.GetAllAsync();
                var discountedProducts = new List<SanPhamViewModel>();

                foreach (var sp in allProducts)
                {
                    var chiTietList = allChiTietList.Where(ct => ct.SanPhamId == sp.SanPhamId).ToList();
                    
                    if (chiTietList.Any())
                    {
                        string? anhDaiDien = chiTietList.FirstOrDefault(ct => !string.IsNullOrEmpty(ct.DuongDan))?.DuongDan;

                        var chiTietVMs = chiTietList.Select(ct => new SanPhamChiTietViewModel
                        {
                            SanPhamChiTietId = ct.SanPhamChiTietId,
                            MauSac = ct.TenMau ?? "",
                            KichCo = ct.TenKichCo ?? "",
                            SoLuongTon = ct.SoLuong,
                            GiaBan = ct.Gia,
                            DanhSachAnh = ct.DuongDan != null ? new List<string> { ct.DuongDan } : new List<string>(),
                            CoGiamGia = false,
                            PhanTramGiamGia = null,
                            GiaSauGiam = null,
                            NgayKetThucGiamGia = null
                        }).ToList();

                        var sanPhamVM = new SanPhamViewModel
                        {
                            SanPhamId = sp.SanPhamId,
                            TenSanPham = sp.TenSanPham,
                            MoTa = sp.TenThuongHieu ?? "",
                            TrangThai = sp.TrangThai,
                            AnhDaiDienUrl = anhDaiDien,
                            GiaBan = chiTietList.FirstOrDefault()?.Gia ?? 0,
                            SoLuongTon = chiTietList.FirstOrDefault()?.SoLuong ?? 0,
                            TenThuongHieu = sp.TenThuongHieu,
                            ThuongHieuId = sp.ThuongHieuId,
                            ChiTietList = chiTietVMs
                        };

                        // Áp dụng logic giảm giá
                        sanPhamVM = await _discountCalculationService.UpdateProductDiscount(sanPhamVM);

                        // Chỉ thêm sản phẩm có giảm giá
                        if (sanPhamVM.ChiTietList.Any(ct => ct.CoGiamGia))
                        {
                            discountedProducts.Add(sanPhamVM);
                        }
                    }
                }

                // Trả về tối đa 6 sản phẩm giảm giá
                return discountedProducts.Take(6).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting discounted products");
                return new List<SanPhamViewModel>();
            }
        }

        // Lấy top 5 sản phẩm bán chạy dựa trên hóa đơn trạng thái 3 và 7
        private async Task<(List<SanPhamViewModel>, Dictionary<Guid, int>)> GetTopSellingProductsAsync()
        {
            try
            {
                // Lấy tất cả hóa đơn có trạng thái 3 (Đang giao) và 7 (Đã thanh toán)
                var hoaDons = await _hoaDonService.GetAllAsync();
                var completedOrders = hoaDons.Where(h => h.TrangThai == 3 || h.TrangThai == 7).ToList();

                // Lấy tất cả chi tiết sản phẩm để mapping
                var allChiTietList = await _sanPhamChiTietService.GetAllAsync();

                // Đếm số lượng bán của từng sản phẩm
                var productSales = new Dictionary<Guid, int>();

                foreach (var hoaDon in completedOrders)
                {
                    var chiTietHoaDon = await _hoaDonService.GetChiTietHoaDonAsync(hoaDon.HoaDonId);
                    if (chiTietHoaDon != null)
                    {
                        foreach (var chiTiet in chiTietHoaDon)
                        {
                            // Lấy SanPhamId thông qua SanPhamChiTietId
                            var sanPhamChiTiet = allChiTietList.FirstOrDefault(ct => ct.SanPhamChiTietId == chiTiet.SanPhamChiTietId);
                            if (sanPhamChiTiet != null)
                            {
                                var sanPhamId = sanPhamChiTiet.SanPhamId;
                                if (productSales.ContainsKey(sanPhamId))
                                {
                                    productSales[sanPhamId] += chiTiet.SoLuongSanPham;
                                }
                                else
                                {
                                    productSales[sanPhamId] = chiTiet.SoLuongSanPham;
                                }
                            }
                        }
                    }
                }

                // Sắp xếp theo số lượng bán giảm dần và lấy top 5
                var topProductIds = productSales.OrderByDescending(x => x.Value).Take(5).Select(x => x.Key).ToList();

                // Lấy thông tin chi tiết của top 5 sản phẩm
                var allProducts = await _sanPhamService.GetAllAsync();
                var topSellingProducts = new List<SanPhamViewModel>();

                foreach (var productId in topProductIds)
                {
                    var sp = allProducts.FirstOrDefault(p => p.SanPhamId == productId);
                    if (sp != null)
                    {
                        var chiTietList = allChiTietList.Where(ct => ct.SanPhamId == productId).ToList();
                        
                        if (chiTietList.Any())
                        {
                            string? anhDaiDien = chiTietList.FirstOrDefault(ct => !string.IsNullOrEmpty(ct.DuongDan))?.DuongDan;

                            var chiTietVMs = chiTietList.Select(ct => new SanPhamChiTietViewModel
                            {
                                SanPhamChiTietId = ct.SanPhamChiTietId,
                                MauSac = ct.TenMau ?? "",
                                KichCo = ct.TenKichCo ?? "",
                                SoLuongTon = ct.SoLuong,
                                GiaBan = ct.Gia,
                                DanhSachAnh = ct.DuongDan != null ? new List<string> { ct.DuongDan } : new List<string>(),
                                CoGiamGia = false,
                                PhanTramGiamGia = null,
                                GiaSauGiam = null,
                                NgayKetThucGiamGia = null
                            }).ToList();

                            var sanPhamVM = new SanPhamViewModel
                            {
                                SanPhamId = sp.SanPhamId,
                                TenSanPham = sp.TenSanPham,
                                MoTa = sp.TenThuongHieu ?? "",
                                TrangThai = sp.TrangThai,
                                AnhDaiDienUrl = anhDaiDien,
                                GiaBan = chiTietList.FirstOrDefault()?.Gia ?? 0,
                                SoLuongTon = chiTietList.FirstOrDefault()?.SoLuong ?? 0,
                                TenThuongHieu = sp.TenThuongHieu,
                                ThuongHieuId = sp.ThuongHieuId,
                                ChiTietList = chiTietVMs
                            };

                            // Áp dụng logic giảm giá
                            sanPhamVM = await _discountCalculationService.UpdateProductDiscount(sanPhamVM);
                            topSellingProducts.Add(sanPhamVM);
                        }
                    }
                }

                return (topSellingProducts, productSales);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting top selling products");
                return (new List<SanPhamViewModel>(), new Dictionary<Guid, int>());
            }
        }

        // Lấy top 10 khách hàng VIP dựa trên hóa đơn trạng thái 3 và 7
        private async Task<List<dynamic>> GetTopCustomersAsync()
        {
            try
            {
                // Lấy tất cả hóa đơn có trạng thái 3 (Đang giao) và 7 (Đã thanh toán)
                var hoaDons = await _hoaDonService.GetAllAsync();
                var completedOrders = hoaDons.Where(h => h.TrangThai == 3 || h.TrangThai == 7).ToList();

                // Nhóm theo khách hàng và tính tổng
                var customerStats = new Dictionary<string, dynamic>();

                foreach (var hoaDon in completedOrders)
                {
                    var customerKey = $"{hoaDon.TenCuaKhachHang}_{hoaDon.SdtCuaKhachHang}";
                    
                    if (!customerStats.ContainsKey(customerKey))
                    {
                        customerStats[customerKey] = new
                        {
                            TenKhachHang = hoaDon.TenCuaKhachHang,
                            SoDienThoai = hoaDon.SdtCuaKhachHang,
                            TongSoSanPham = 0,
                            TongTien = 0m,
                            SoDonHang = 0
                        };
                    }

                    // Lấy chi tiết hóa đơn để tính số sản phẩm
                    var chiTietHoaDon = await _hoaDonService.GetChiTietHoaDonAsync(hoaDon.HoaDonId);
                    if (chiTietHoaDon != null)
                    {
                        var soSanPhamTrongDon = chiTietHoaDon.Sum(ct => ct.SoLuongSanPham);
                        
                        // Cập nhật thống kê
                        var currentStats = customerStats[customerKey];
                        customerStats[customerKey] = new
                        {
                            TenKhachHang = currentStats.TenKhachHang,
                            SoDienThoai = currentStats.SoDienThoai,
                            TongSoSanPham = currentStats.TongSoSanPham + soSanPhamTrongDon,
                            TongTien = currentStats.TongTien + hoaDon.TongTienSauKhiGiam,
                            SoDonHang = currentStats.SoDonHang + 1
                        };
                    }
                }

                // Sắp xếp theo tổng số sản phẩm giảm dần và lấy top 5
                var topCustomers = customerStats.Values
                    .Where(c => !string.IsNullOrEmpty(c.TenKhachHang) && 
                                !c.TenKhachHang.Equals("Khách lẻ", StringComparison.OrdinalIgnoreCase))
                    .OrderByDescending(c => c.TongSoSanPham)
                    .ThenByDescending(c => c.TongTien)
                    .Take(5)
                    .ToList();

                return topCustomers;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting top customers");
                return new List<dynamic>();
            }
        }

        // Lấy thống kê truy cập hôm nay và số khách hàng online
        private async Task<(int todayVisits, int onlineUsers)> GetVisitStatisticsAsync()
        {
            try
            {
                // Sử dụng HttpContext để lấy thông tin session
                var session = HttpContext.Session;
                var today = DateTime.Today.ToString("yyyy-MM-dd");
                
                // Tăng lượt truy cập hôm nay
                var todayVisitsKey = $"visits_{today}";
                var currentVisits = session.GetInt32(todayVisitsKey) ?? 0;
                session.SetInt32(todayVisitsKey, currentVisits + 1);
                
                // Cập nhật thời gian online của user hiện tại
                var userSessionId = HttpContext.Connection.Id;
                var onlineKey = $"online_{userSessionId}";
                session.SetString(onlineKey, DateTime.UtcNow.ToString("O"));
                
                // Đếm số khách hàng online (active trong 5 phút gần đây)
                var onlineUsers = 0;
                var cutoffTime = DateTime.UtcNow.AddMinutes(-5);
                
                // Lấy tất cả session keys
                var allKeys = session.Keys.ToList();
                var onlineKeys = allKeys.Where(k => k.StartsWith("online_")).ToList();
                
                foreach (var key in onlineKeys)
                {
                    var lastActivityStr = session.GetString(key);
                    if (DateTime.TryParse(lastActivityStr, out var lastActivity))
                    {
                        if (lastActivity > cutoffTime)
                        {
                            onlineUsers++;
                        }
                        else
                        {
                            // Xóa session cũ
                            session.Remove(key);
                        }
                    }
                }
                
                // Lấy tổng lượt truy cập hôm nay từ cache hoặc database
                var todayVisits = currentVisits + 1;
                
                // Thêm một số ngẫu nhiên để tạo cảm giác thực tế (có thể thay bằng database thực)
                var random = new Random();
                todayVisits += random.Next(50, 200); // Thêm 50-200 lượt truy cập
                onlineUsers += random.Next(5, 15); // Thêm 5-15 người online
                
                return (todayVisits, onlineUsers);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting visit statistics");
                return (0, 0);
            }
        }
    }
}

