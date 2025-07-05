using System.Net.Http;
using System.Net.Http.Json;
using FurryFriends.Web.Services.IService;
using FurryFriends.API.Models.DTO;

namespace FurryFriends.Web.Services
{
    public class ChatLieuService : IChatLieuService
    {
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "api/ChatLieu";

        public ChatLieuService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IEnumerable<ChatLieuDTO>> GetAllAsync()
        {
            var result = await _httpClient.GetFromJsonAsync<IEnumerable<ChatLieuDTO>>(BaseUrl);
            return result ?? new List<ChatLieuDTO>();
        }

        public async Task<ChatLieuDTO> GetByIdAsync(Guid id)
        {
            var response = await _httpClient.GetAsync($"{BaseUrl}/{id}");
            if (response.IsSuccessStatusCode)
                return await response.Content.ReadFromJsonAsync<ChatLieuDTO>();
            return null;
        }

        public async Task<ChatLieuDTO> CreateAsync(ChatLieuDTO dto)
        {
            var response = await _httpClient.PostAsJsonAsync(BaseUrl, dto);
            if (response.IsSuccessStatusCode)
                return await response.Content.ReadFromJsonAsync<ChatLieuDTO>();
            return null;
        }

        public async Task<bool> UpdateAsync(Guid id, ChatLieuDTO dto)
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
