using FurryFriends.API.Models;
using FurryFriends.Web.Services.IService;
using System.Net.Http.Json;

namespace FurryFriends.Web.Services
{
    public class HoaDonService : IHoaDonService
    {
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "api/HoaDon";

        public HoaDonService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IEnumerable<HoaDon>> GetHoaDonListAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync(BaseUrl);
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<IEnumerable<HoaDon>>();
                    return result ?? new List<HoaDon>();
                }
                throw new Exception($"Lỗi khi lấy danh sách hóa đơn: {response.StatusCode}");
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy danh sách hóa đơn: {ex.Message}");
            }
        }

        public async Task<HoaDon> GetHoaDonByIdAsync(Guid hoaDonId)
        {
            try
            {
                if (hoaDonId == Guid.Empty)
                {
                    throw new ArgumentException("ID hóa đơn không hợp lệ");
                }

                var response = await _httpClient.GetAsync($"{BaseUrl}/{hoaDonId}");
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<HoaDon>();
                    if (result == null)
                    {
                        throw new KeyNotFoundException($"Không tìm thấy hóa đơn với ID: {hoaDonId}");
                    }
                    return result;
                }
                throw new Exception($"Lỗi khi lấy thông tin hóa đơn: {response.StatusCode}");
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy thông tin hóa đơn: {ex.Message}");
            }
        }

        public async Task<IEnumerable<HoaDon>> SearchHoaDonAsync(string keyword)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(keyword))
                {
                    throw new ArgumentException("Từ khóa tìm kiếm không được để trống");
                }

                var response = await _httpClient.GetAsync($"{BaseUrl}/search?keyword={Uri.EscapeDataString(keyword)}");
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<IEnumerable<HoaDon>>();
                    return result ?? new List<HoaDon>();
                }
                throw new Exception($"Lỗi khi tìm kiếm hóa đơn: {response.StatusCode}");
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi tìm kiếm hóa đơn: {ex.Message}");
            }
        }

        public async Task<byte[]> ExportHoaDonToPdfAsync(Guid hoaDonId)
        {
            try
            {
                if (hoaDonId == Guid.Empty)
                {
                    throw new ArgumentException("ID hóa đơn không hợp lệ");
                }

                var response = await _httpClient.GetAsync($"{BaseUrl}/{hoaDonId}/pdf");
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsByteArrayAsync();
                }
                throw new Exception($"Lỗi khi xuất hóa đơn PDF: {response.StatusCode}");
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi xuất hóa đơn PDF: {ex.Message}");
            }
        }
    }
} 