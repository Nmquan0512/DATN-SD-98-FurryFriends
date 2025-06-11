using FurryFriends.API.Models;
using FurryFriends.Web.Services.IService;
using System.Net.Http.Json;

namespace FurryFriends.Web.Services
{
    public class HoaDonService : IHoaDonService
    {
        private readonly HttpClient _httpClient;

        public HoaDonService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IEnumerable<HoaDon>> GetHoaDonListAsync()
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<IEnumerable<HoaDon>>("api/HoaDon");
                return response ?? Enumerable.Empty<HoaDon>();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error getting hoa don list: {ex.Message}", ex);
            }
        }

        public async Task<HoaDon> GetHoaDonByIdAsync(Guid hoaDonId)
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<HoaDon>($"api/HoaDon/{hoaDonId}");
                if (response == null)
                {
                    throw new KeyNotFoundException($"Không tìm thấy hóa đơn với ID: {hoaDonId}");
                }
                return response;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error getting hoa don by id: {ex.Message}", ex);
            }
        }

        public async Task<IEnumerable<HoaDon>> SearchHoaDonAsync(string keyword)
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<IEnumerable<HoaDon>>($"api/HoaDon/search?keyword={keyword}");
                return response ?? Enumerable.Empty<HoaDon>();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error searching hoa don: {ex.Message}", ex);
            }
        }

        public async Task<byte[]> ExportHoaDonToPdfAsync(Guid hoaDonId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/HoaDon/{hoaDonId}/pdf");
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsByteArrayAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error exporting hoa don to PDF: {ex.Message}", ex);
            }
        }
    }
} 