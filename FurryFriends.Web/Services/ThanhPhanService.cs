using FurryFriends.API.Models.DTO;
using FurryFriends.Web.Services.IService;
using System.Net.Http;
using System.Net.Http.Json;

namespace FurryFriends.Web.Services
{
    public class ThanhPhanService : IThanhPhanService
    {
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "api/ThanhPhan";

        public ThanhPhanService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IEnumerable<ThanhPhanDTO>> GetAllAsync()
        {
            var result = await _httpClient.GetFromJsonAsync<IEnumerable<ThanhPhanDTO>>(BaseUrl);
            return result ?? new List<ThanhPhanDTO>();
        }

        public async Task<ThanhPhanDTO> GetByIdAsync(Guid id)
        {
            var response = await _httpClient.GetAsync($"{BaseUrl}/{id}");
            if (response.IsSuccessStatusCode)
                return await response.Content.ReadFromJsonAsync<ThanhPhanDTO>();
            return null;
        }

        public async Task<ThanhPhanDTO> CreateAsync(ThanhPhanDTO dto)
        {
            var response = await _httpClient.PostAsJsonAsync(BaseUrl, dto);
            if (response.IsSuccessStatusCode)
                return await response.Content.ReadFromJsonAsync<ThanhPhanDTO>();
            return null;
        }

        public async Task<bool> UpdateAsync(Guid id, ThanhPhanDTO dto)
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
