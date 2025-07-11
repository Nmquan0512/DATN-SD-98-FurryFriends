using FurryFriends.Web.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using FurryFriends.API.Models;
using FurryFriends.Web.Services.IService;

namespace FurryFriends.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IHoaDonService _hoaDonService;

        public HomeController(ILogger<HomeController> logger, IHoaDonService hoaDonService)
        {
            _logger = logger;
            _hoaDonService = hoaDonService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpPost]
        public async Task<IActionResult> ChatGemini([FromBody] ChatRequest req)
        {
            string userMsg = req.Message?.Trim() ?? "";
            string answer = "";

            // 1. Tra cứu hóa đơn (ưu tiên, không hỏi Gemini)
            var invoiceMatch = Regex.Match(userMsg, @"h[oóòỏõọ][aáàảãạăắằẳẵặâấầẩẫậ]? ?đ[oóòỏõọơớờởỡợ]n.*?([0-9a-fA-F]{8}|[0-9a-fA-F]{8}-[0-9a-fA-F\-]{27,})", RegexOptions.IgnoreCase);
            if (invoiceMatch.Success)
            {
                var invoiceId = invoiceMatch.Groups[1].Value;
                HoaDon hoaDon = null;
                try
                {
                    if (Guid.TryParse(invoiceId, out Guid hoaDonGuid))
                    {
                        hoaDon = await _hoaDonService.GetHoaDonByIdAsync(hoaDonGuid);
                    }
                    else if (invoiceId.Length == 8)
                    {
                        var all = await _hoaDonService.GetHoaDonListAsync();
                        hoaDon = all.FirstOrDefault(h => h.HoaDonId.ToString().Substring(0, 8).Equals(invoiceId, StringComparison.OrdinalIgnoreCase));
                    }
                }
                catch { }
                if (hoaDon == null)
                {
                    answer = $"Không tìm thấy hóa đơn với mã {invoiceId}.";
                }
                else
                {
                    string[] trangThaiArr = { "Chờ xác nhận", "Đang xử lý", "Đang giao hàng", "Đã giao hàng", "Đã hủy" };
                    string trangThai = hoaDon.TrangThai >= 0 && hoaDon.TrangThai < trangThaiArr.Length ? trangThaiArr[hoaDon.TrangThai] : "Không xác định";
                    string payment = hoaDon.HinhThucThanhToan?.TenHinhThuc ?? "Không rõ";
                    answer = $@"<div><h3>Thông tin hóa đơn #{hoaDon.HoaDonId.ToString().Substring(0, 8).ToUpper()}</h3><ul><li><b>Ngày tạo:</b> {hoaDon.NgayTao:dd/MM/yyyy HH:mm}</li><li><b>Khách hàng:</b> {hoaDon.TenCuaKhachHang}</li><li><b>Tổng tiền:</b> {hoaDon.TongTienSauKhiGiam:N0}đ</li><li><b>Trạng thái:</b> {trangThai}</li><li><b>Phương thức thanh toán:</b> {payment}</li></ul></div>";
                }
                return Json(new { answer });
            }

            // 2. Nếu không nhập mã hóa đơn, yêu cầu nhập mã
            var containsHoaDon = Regex.IsMatch(userMsg, @"h[oó]a ?đ[oơ]n", RegexOptions.IgnoreCase);
            if (containsHoaDon)
            {
                answer = "Vui lòng ghi mã hóa đơn để tra cứu.";
                return Json(new { answer });
            }

            // 3. Fallback Gemini cho các câu hỏi khác
            string prompt = $"Bạn là một trợ lý bán hàng cho website bán đồ dùng cho thú cưng tên là FurryFriends chuyên nghiệp, thân thiện, trả lời ngắn gọn, dễ hiểu, ưu tiên trả lời đúng nghiệp vụ bán hàng. Nếu câu hỏi của khách hàng liên quan đến hóa đơn thì hãy trả lời theo dữ liệu thực tế (nếu có). Nếu không có dữ liệu, hãy trả lời tự nhiên, không bịa thông tin. Câu hỏi của khách hàng: '{userMsg}'. Luôn trả lời bằng tiếng Việt. Nếu khách hàng hỏi bạn gì liên quan tới tìm hoá đơn ,đơn hàng , sản phẩm thì hãy trả lời là hãy viết theo format là mã hoá đơn , đơn hàng là + mã , tên sản phẩm + tên ví dụ mã hoá đơn là 3FA85F64 @\"m[aã] h[oó]a ?đ[oơ]n l[aà] ([0-9a-fA-F\\-]{{5,}}) , @\"t[eê]n s[aả]n ph[aẩ]m l[aà] ([\\w\\s\\-\\(\\)\\,\\.]+)\"\r\n   và không được giới thiệu website bán thú cưng khác ngoài FurryFriends";
            using var http = new HttpClient();
            var requestBody = new
            {
                contents = new[]
                {
                    new { parts = new[] { new { text = prompt } } }
                }
            };
            var content = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json");
            var apiKey = "AIzaSyCXRI7hVFop8QLSwLXoGLDthI7nq8vlUI8";
            var endpoint = "https://generativelanguage.googleapis.com/v1beta/models/gemini-2.0-flash:generateContent?key=" + apiKey;
            var response = await http.PostAsync(endpoint, content);
            var json = await response.Content.ReadAsStringAsync();
            dynamic result = Newtonsoft.Json.JsonConvert.DeserializeObject(json);
            answer = result?.candidates?[0]?.content?.parts?[0]?.text ?? "Xin lỗi, tôi chưa có câu trả lời.";
            return Json(new { answer });
        }

        public class ChatRequest
        {
            public string Message { get; set; }
        }
    }
}

