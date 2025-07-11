using FurryFriends.API.Models.DTO;
using FurryFriends.Web.Services.IService;
using Microsoft.AspNetCore.Http;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace FurryFriends.Web.Services
{
    public class AnhService : IAnhService
    {
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "api/Anh";

        public AnhService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IEnumerable<AnhDTO>> GetAllAsync()
        {
            var response = await _httpClient.GetAsync(BaseUrl);
            if (!response.IsSuccessStatusCode) return new List<AnhDTO>();
            return await response.Content.ReadFromJsonAsync<IEnumerable<AnhDTO>>();
        }

        public async Task<AnhDTO> GetByIdAsync(Guid id)
        {
            var response = await _httpClient.GetAsync($"{BaseUrl}/{id}");
            if (!response.IsSuccessStatusCode) return null;
            return await response.Content.ReadFromJsonAsync<AnhDTO>();
        }

        public async Task<AnhDTO> UploadAsync(IFormFile file)
        {
            if (file == null || file.Length == 0) return null;

            using var content = new MultipartFormDataContent();
            var streamContent = new StreamContent(file.OpenReadStream());
            streamContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);
            content.Add(streamContent, "file", file.FileName);

            var response = await _httpClient.PostAsync($"{BaseUrl}/upload", content);
            if (!response.IsSuccessStatusCode) return null;

            return await response.Content.ReadFromJsonAsync<AnhDTO>();
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var response = await _httpClient.DeleteAsync($"{BaseUrl}/{id}");
            return response.IsSuccessStatusCode;
        }
    }
}
