using FurryFriends.API.Models.DTO.BanHang;
using FurryFriends.API.Models.DTO.BanHang.Requests;
using FurryFriends.Web.Services.IService;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace FurryFriends.Web.Services
{
    public class BanHangService : IBanHangService
    {
        private readonly HttpClient _httpClient;
        private const string BasePath = "api/ban-hang";

        public BanHangService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        #region Hóa Đơn
        public async Task<IEnumerable<HoaDonBanHangDto>> GetAllHoaDonsAsync()
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<HoaDonBanHangDto>>($"{BasePath}/hoa-don");
        }

        public async Task<HoaDonBanHangDto> GetHoaDonByIdAsync(Guid hoaDonId)
        {
            return await _httpClient.GetFromJsonAsync<HoaDonBanHangDto>($"{BasePath}/hoa-don/{hoaDonId}");
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

        public async Task<HoaDonBanHangDto> GoBoVoucherAsync(Guid hoaDonId)
        {
            var response = await _httpClient.DeleteAsync($"{BasePath}/hoa-don/{hoaDonId}/voucher");
            return await ProcessResponse<HoaDonBanHangDto>(response);
        }
        #endregion

        #region Thanh toán
        public async Task<HoaDonBanHangDto> ThanhToanHoaDonAsync(Guid hoaDonId, ThanhToanRequest request)
        {
            var response = await _httpClient.PostAsJsonAsync($"{BasePath}/hoa-don/{hoaDonId}/thanh-toan", request);
            return await ProcessResponse<HoaDonBanHangDto>(response);
        }
        #endregion

        #region Tìm kiếm
        public async Task<IEnumerable<SanPhamBanHangDto>> TimKiemSanPhamAsync(string keyword)
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<SanPhamBanHangDto>>($"{BasePath}/tim-kiem/san-pham?keyword={Uri.EscapeDataString(keyword ?? "")}");
        }

        public async Task<IEnumerable<KhachHangDto>> TimKiemKhachHangAsync(string keyword)
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<KhachHangDto>>($"{BasePath}/tim-kiem/khach-hang?keyword={Uri.EscapeDataString(keyword ?? "")}");
        }

        public async Task<IEnumerable<VoucherDto>> TimKiemVoucherHopLeAsync(Guid hoaDonId)
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<VoucherDto>>($"{BasePath}/hoa-don/{hoaDonId}/vouchers-hop-le");
        }
        #endregion

        #region Khách hàng
        public async Task<KhachHangDto> TaoKhachHangMoiAsync(TaoKhachHangRequest request)
        {
            var response = await _httpClient.PostAsJsonAsync($"{BasePath}/khach-hang", request);
            return await ProcessResponse<KhachHangDto>(response);
        }
        #endregion

        #region Helper Methods
        private async Task<T> ProcessResponse<T>(HttpResponseMessage response, System.Net.HttpStatusCode expectedStatusCode = System.Net.HttpStatusCode.OK)
        {
            if (response.StatusCode == expectedStatusCode)
            {
                if (response.Content.Headers.ContentLength == 0) return default;
                return await response.Content.ReadFromJsonAsync<T>();
            }

            var errorMessage = await response.Content.ReadAsStringAsync();
            throw new HttpRequestException($"API request failed with status {response.StatusCode}. Message: {errorMessage}");
        }
        #endregion
    }
}
