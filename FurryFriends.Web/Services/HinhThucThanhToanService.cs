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
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<IEnumerable<HinhThucThanhToan>>();
        }
    }

}
