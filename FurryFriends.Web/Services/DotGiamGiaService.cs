using FurryFriends.API.Models;
using FurryFriends.Web.Services.IService;

namespace FurryFriends.Web.Services
{
    public class DotGiamGiaService : IDotGiamGiaService
    {
        private readonly HttpClient _httpClient;

        public DotGiamGiaService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IEnumerable<DotGiamGiaSanPham>> GetAllAsync()
        {
            var response = await _httpClient.GetAsync("DotGiamGia");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<IEnumerable<DotGiamGiaSanPham>>() ?? new List<DotGiamGiaSanPham>();
            }
            return new List<DotGiamGiaSanPham>();
        }

        public async Task<DotGiamGiaSanPham?> GetByIdAsync(Guid id)
        {
            var response = await _httpClient.GetAsync($"DotGiamGia/{id}");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<DotGiamGiaSanPham>();
            }
            return null;
        }

        public async Task<bool> AddAsync(DotGiamGiaSanPham dotGiamGia)
        {
            var response = await _httpClient.PostAsJsonAsync("DotGiamGia", dotGiamGia);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateAsync(Guid id, DotGiamGiaSanPham dotGiamGia)
        {
            var response = await _httpClient.PutAsJsonAsync($"DotGiamGia/{id}", dotGiamGia);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var response = await _httpClient.DeleteAsync($"DotGiamGia/{id}");
            return response.IsSuccessStatusCode;
        }
    }
}