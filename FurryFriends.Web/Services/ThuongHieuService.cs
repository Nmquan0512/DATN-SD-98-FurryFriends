using System.Net.Http;
using System.Net.Http.Json;
using FurryFriends.Web.Services.IService;
using FurryFriends.API.Models.DTO;

namespace FurryFriends.Web.Services
{
    public class ThuongHieuService : IThuongHieuService
    {
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "api/ThuongHieu";

        public ThuongHieuService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IEnumerable<ThuongHieuDTO>> GetAllAsync()
        {
            var result = await _httpClient.GetFromJsonAsync<IEnumerable<ThuongHieuDTO>>(BaseUrl);
            return result ?? new List<ThuongHieuDTO>();
        }

        public async Task<ThuongHieuDTO> GetByIdAsync(Guid id)
        {
            var response = await _httpClient.GetAsync($"{BaseUrl}/{id}");
            if (response.IsSuccessStatusCode)
                return await response.Content.ReadFromJsonAsync<ThuongHieuDTO>();
            return null;
        }

        public async Task<ThuongHieuDTO> CreateAsync(ThuongHieuDTO dto)
        {
            var response = await _httpClient.PostAsJsonAsync(BaseUrl, dto);
            if (response.IsSuccessStatusCode)
                return await response.Content.ReadFromJsonAsync<ThuongHieuDTO>();
            return null;
        }

        public async Task<bool> UpdateAsync(Guid id, ThuongHieuDTO dto)
        {
            var response = await _httpClient.PutAsJsonAsync($"{BaseUrl}/{id}", dto);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var response = await _httpClient.DeleteAsync($"{BaseUrl}/{id}");
            return response.IsSuccessStatusCode;
        }
    }
}
