using System.Net.Http;
using System.Net.Http.Json;
using FurryFriends.Web.Services.IService;
using FurryFriends.API.Models.DTO;

namespace FurryFriends.Web.Services
{
    public class MauSacService : IMauSacService
    {
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "api/MauSac";

        public MauSacService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IEnumerable<MauSacDTO>> GetAllAsync()
        {
            var result = await _httpClient.GetFromJsonAsync<IEnumerable<MauSacDTO>>(BaseUrl);
            return result ?? new List<MauSacDTO>();
        }

        public async Task<MauSacDTO> GetByIdAsync(Guid id)
        {
            var response = await _httpClient.GetAsync($"{BaseUrl}/{id}");
            if (response.IsSuccessStatusCode)
                return await response.Content.ReadFromJsonAsync<MauSacDTO>();
            return null;
        }

        public async Task<MauSacDTO> CreateAsync(MauSacDTO dto)
        {
            var response = await _httpClient.PostAsJsonAsync(BaseUrl, dto);
            if (response.IsSuccessStatusCode)
                return await response.Content.ReadFromJsonAsync<MauSacDTO>();
            return null;
        }

        public async Task<bool> UpdateAsync(Guid id, MauSacDTO dto)
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
