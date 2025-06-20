using FurryFriends.API.Models;
using FurryFriends.Web.Services.IService;

namespace FurryFriends.Web.Services
{
    public class GiamGiaService : IGiamGiaService
    {
        private readonly HttpClient _httpClient;

        public GiamGiaService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IEnumerable<GiamGia>> GetAllAsync()
        {
            var response = await _httpClient.GetAsync("GiamGia");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<IEnumerable<GiamGia>>() ?? new List<GiamGia>();
            }
            return new List<GiamGia>();
        }

        public async Task<GiamGia?> GetByIdAsync(Guid id)
        {
            var response = await _httpClient.GetAsync($"GiamGia/{id}");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<GiamGia>();
            }
            return null;
        }

        public async Task<bool> CreateAsync(GiamGia giamGia)
        {
            var response = await _httpClient.PostAsJsonAsync("GiamGia", giamGia);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateAsync(Guid id, GiamGia giamGia)
        {
            var response = await _httpClient.PutAsJsonAsync($"GiamGia/{id}", giamGia);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var response = await _httpClient.DeleteAsync($"GiamGia/{id}");
            return response.IsSuccessStatusCode;
        }
    }

}