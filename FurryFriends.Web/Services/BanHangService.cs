using FurryFriends.API.Models.DTO.BanHang;
using FurryFriends.API.Models.DTO.BanHang.Requests;
using FurryFriends.Web.Services.IService;
using System.Net;
using System.Net.Http.Json;
using Newtonsoft.Json;

namespace FurryFriends.Web.Services
{
    public class BanHangService : IBanHangService
    {
        private readonly HttpClient _httpClient;
        // ĐẢM BẢO ĐƯỜNG DẪN NÀY KHỚP VỚI [Route] CỦA API CONTROLLER
        private const string BasePath = "api/BanHang";

        public BanHangService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        #region Hóa Đơn
        public async Task<IEnumerable<HoaDonBanHangDto>> GetAllHoaDonsAsync()
        {
            var response = await _httpClient.GetAsync($"{BasePath}/hoa-don");
            return await ProcessResponse<IEnumerable<HoaDonBanHangDto>>(response);
        }

        public async Task<HoaDonBanHangDto> GetHoaDonByIdAsync(Guid hoaDonId)
        {
            var response = await _httpClient.GetAsync($"{BasePath}/hoa-don/{hoaDonId}");
            return await ProcessResponse<HoaDonBanHangDto>(response);
        }

        public async Task<HoaDonBanHangDto> TaoHoaDonAsync(TaoHoaDonRequest request)
        {
            var response = await _httpClient.PostAsJsonAsync($"{BasePath}/hoa-don", request);
            return await ProcessResponse<HoaDonBanHangDto>(response, HttpStatusCode.Created);
        }

        public async Task<HoaDonBanHangDto> HuyHoaDonAsync(Guid hoaDonId)
        {
            var response = await _httpClient.PostAsync($"{BasePath}/hoa-don/{hoaDonId}/huy", null);
            return await ProcessResponse<HoaDonBanHangDto>(response);
        }
        #endregion

        #region Quản lý Chi tiết Hóa đơn (Items)
        public async Task<HoaDonBanHangDto> ThemSanPhamVaoHoaDonAsync(Guid hoaDonId, ThemSanPhamRequest request)
        {
            var response = await _httpClient.PostAsJsonAsync($"{BasePath}/hoa-don/{hoaDonId}/items", request);
            return await ProcessResponse<HoaDonBanHangDto>(response);
        }

        public async Task<HoaDonBanHangDto> CapNhatSoLuongSanPhamAsync(Guid hoaDonId, Guid sanPhamChiTietId, CapNhatSoLuongRequest request)
        {
            var response = await _httpClient.PutAsJsonAsync($"{BasePath}/hoa-don/{hoaDonId}/items/{sanPhamChiTietId}", request);
            return await ProcessResponse<HoaDonBanHangDto>(response);
        }

        public async Task<HoaDonBanHangDto> XoaSanPhamKhoiHoaDonAsync(Guid hoaDonId, Guid sanPhamChiTietId)
        {
            var response = await _httpClient.DeleteAsync($"{BasePath}/hoa-don/{hoaDonId}/items/{sanPhamChiTietId}");
            return await ProcessResponse<HoaDonBanHangDto>(response);
        }
        #endregion

        #region Voucher & Khách hàng
        public async Task<HoaDonBanHangDto> GanKhachHangAsync(Guid hoaDonId, GanKhachHangRequest request)
        {
            var response = await _httpClient.PutAsJsonAsync($"{BasePath}/hoa-don/{hoaDonId}/khach-hang", request);
            return await ProcessResponse<HoaDonBanHangDto>(response);
        }

        public async Task<HoaDonBanHangDto> ApDungVoucherAsync(Guid hoaDonId, ApDungVoucherRequest request)
        {
            var response = await _httpClient.PostAsJsonAsync($"{BasePath}/hoa-don/{hoaDonId}/voucher", request);
            return await ProcessResponse<HoaDonBanHangDto>(response);
        }

        // Sử dụng API của Giỏ hàng để áp dụng voucher (giống logic Giỏ hàng)
        public async Task<object> ApDungVoucherGioHangAsync(Guid khachHangId, Guid voucherId)
        {
            var dto = new { KhachHangId = khachHangId, VoucherId = voucherId };
            var response = await _httpClient.PostAsJsonAsync("/api/GioHang/ap-dung-voucher", dto);
            
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"Không thể áp dụng voucher: {error}");
            }

            var responseBody = await response.Content.ReadAsStringAsync();
            return Newtonsoft.Json.JsonConvert.DeserializeObject(responseBody);
        }

        public async Task<HoaDonBanHangDto> GoBoVoucherAsync(Guid hoaDonId)
        {
            var response = await _httpClient.DeleteAsync($"{BasePath}/hoa-don/{hoaDonId}/voucher");
            return await ProcessResponse<HoaDonBanHangDto>(response);
        }

        public async Task<HoaDonBanHangDto> CapNhatDiaChiGiaoHangAsync(Guid hoaDonId, DiaChiMoiDto diaChiMoi)
        {
            var response = await _httpClient.PutAsJsonAsync($"{BasePath}/hoa-don/{hoaDonId}/dia-chi-giao-hang", diaChiMoi);
            return await ProcessResponse<HoaDonBanHangDto>(response);
        }
        #endregion
         
        #region Thanh toán
        public async Task<HoaDonBanHangDto> ThanhToanHoaDonAsync(Guid hoaDonId, ThanhToanRequest request)
        {
            var response = await _httpClient.PostAsJsonAsync($"{BasePath}/hoa-don/{hoaDonId}/thanh-toan", request);
            return await ProcessResponse<HoaDonBanHangDto>(response);
        }

        public async Task<IEnumerable<HinhThucThanhToanDto>> GetHinhThucThanhToanAsync()
        {
            var response = await _httpClient.GetAsync("api/HinhThucThanhToan");
            return await ProcessResponse<IEnumerable<HinhThucThanhToanDto>>(response);
        }

        public async Task<object> GetQRCodeAsync(Guid hoaDonId)
        {
            var response = await _httpClient.GetAsync($"{BasePath}/hoa-don/{hoaDonId}/qr-code");
            return await ProcessResponse<object>(response);
        }
        #endregion

        #region Tìm kiếm
        public async Task<IEnumerable<SanPhamBanHangDto>> TimKiemSanPhamAsync(string keyword)
        {
            var response = await _httpClient.GetAsync($"{BasePath}/tim-kiem/san-pham?keyword={Uri.EscapeDataString(keyword ?? "")}");
            return await ProcessResponse<IEnumerable<SanPhamBanHangDto>>(response);
        }

        public async Task<IEnumerable<KhachHangDto>> TimKiemKhachHangAsync(string keyword)
        {
            var response = await _httpClient.GetAsync($"{BasePath}/tim-kiem/khach-hang?keyword={Uri.EscapeDataString(keyword ?? "")}");
            return await ProcessResponse<IEnumerable<KhachHangDto>>(response);
        }

        public async Task<IEnumerable<VoucherDto>> TimKiemVoucherHopLeAsync(Guid hoaDonId)
        {
            var response = await _httpClient.GetAsync($"{BasePath}/hoa-don/{hoaDonId}/vouchers-hop-le");
            return await ProcessResponse<IEnumerable<VoucherDto>>(response);
        }
        #endregion

        #region Khách hàng
        public async Task<KhachHangDto> TaoKhachHangMoiAsync(TaoKhachHangRequest request)
        {
            var response = await _httpClient.PostAsJsonAsync($"{BasePath}/khach-hang", request);
            return await ProcessResponse<KhachHangDto>(response);
        }
        #endregion

        #region Helper Method để xử lý Response
        /// <summary>
        /// Xử lý phản hồi từ API. Nếu thành công, deserialize nội dung. Nếu thất bại, ném ra ApiException.
        /// </summary>
        private async Task<T> ProcessResponse<T>(HttpResponseMessage response, HttpStatusCode expectedStatusCode = HttpStatusCode.OK)
        {
            if (response.StatusCode == expectedStatusCode)
            {
                if (response.StatusCode == HttpStatusCode.NoContent || response.Content.Headers.ContentLength == 0)
                {
                    return default; // Trả về giá trị mặc định (null) nếu không có nội dung
                }
                return await response.Content.ReadFromJsonAsync<T>();
            }

            // Xử lý các trường hợp thành công khác (ví dụ: GET trả về OK)
            if (response.IsSuccessStatusCode && response.StatusCode != expectedStatusCode)
            {
                if (response.Content.Headers.ContentLength == 0) return default;
                return await response.Content.ReadFromJsonAsync<T>();
            }

            // Xử lý lỗi
            var errorContent = await response.Content.ReadAsStringAsync();
            var errorMessage = errorContent;

            // Cố gắng parse message từ cấu trúc JSON { "message": "Lỗi ở đây" } hoặc chỉ là một chuỗi "Lỗi ở đây"
            try
            {
                var errorObject = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(errorContent);
                if (errorObject != null && errorObject.TryGetValue("message", out var msg))
                {
                    errorMessage = msg;
                }
            }
            catch
            {
                // Nếu không parse được, thì lỗi là một chuỗi đơn giản
                errorMessage = errorContent.Trim('"');
            }

            throw new ApiException(errorMessage, response.StatusCode, errorContent);
        }
        public async Task<IEnumerable<SanPhamBanHangDto>> GetSuggestedProductsAsync()
        {
            // Gọi đến endpoint mới bạn vừa tạo trong API Controller
            var response = await _httpClient.GetAsync($"{BasePath}/tim-kiem/san-pham-goi-y");
            return await ProcessResponse<IEnumerable<SanPhamBanHangDto>>(response);
        }

        #endregion
    }
}
