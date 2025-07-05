using System.Net.Http;
using System.Net.Http.Json;
using FurryFriends.Web.Services.IService;
using FurryFriends.API.Models.DTO;

namespace FurryFriends.Web.Services
{
    public class KichCoService : IKichCoService
    {
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "api/KichCo";

        public KichCoService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IEnumerable<KichCoDTO>> GetAllAsync()
        {
            var result = await _httpClient.GetFromJsonAsync<IEnumerable<KichCoDTO>>(BaseUrl);
            return result ?? new List<KichCoDTO>();
        }

        public async Task<KichCoDTO> GetByIdAsync(Guid id)
        {
            var response = await _httpClient.GetAsync($"{BaseUrl}/{id}");
            if (response.IsSuccessStatusCode)
                return await response.Content.ReadFromJsonAsync<KichCoDTO>();
            return null;
        }

        public async Task<KichCoDTO> CreateAsync(KichCoDTO dto)
        {
            var response = await _httpClient.PostAsJsonAsync(BaseUrl, dto);
            if (response.IsSuccessStatusCode)
                return await response.Content.ReadFromJsonAsync<KichCoDTO>();
            return null;
        }

        public async Task<bool> UpdateAsync(Guid id, KichCoDTO dto)
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
