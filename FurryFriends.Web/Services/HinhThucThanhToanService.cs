using FurryFriends.API.Models;
using FurryFriends.Web.Services.IService;

namespace FurryFriends.Web.Services
{
    public class HinhThucThanhToanService : IHinhThucThanhToanService
    {
        private readonly HttpClient _httpClient;

        public HinhThucThanhToanService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IEnumerable<HinhThucThanhToan>> GetAllAsync()
        {
            var response = await _httpClient.GetAsync("https://localhost:7289/api/HinhThucThanhToan");
            if (!response.IsSuccessStatusCode)
            {
                var err = await response.Content.ReadAsStringAsync();
                throw new Exception($"API HinhThucThanhToan lỗi ({(int)response.StatusCode}): {err}");
            }

            return await response.Content.ReadFromJsonAsync<IEnumerable<HinhThucThanhToan>>();
        }

        public async Task<HinhThucThanhToan?> GetByIdAsync(Guid id)
        {
            var response = await _httpClient.GetAsync($"https://localhost:7289/api/HinhThucThanhToan/{id}");
            if (!response.IsSuccessStatusCode)
            {
                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return null;
                }
                var err = await response.Content.ReadAsStringAsync();
                throw new Exception($"API HinhThucThanhToan lỗi ({(int)response.StatusCode}): {err}");
            }

            return await response.Content.ReadFromJsonAsync<HinhThucThanhToan>();
        }
    }
}
