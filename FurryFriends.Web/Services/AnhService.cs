using FurryFriends.API.Models.DTO;
using FurryFriends.Web.Services.IService;
using Microsoft.AspNetCore.Http;
using System.Net.Http;
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
            var result = await _httpClient.GetFromJsonAsync<IEnumerable<AnhDTO>>(BaseUrl);
            return result ?? new List<AnhDTO>();
        }

        public async Task<AnhDTO?> GetByIdAsync(Guid id)
        {
            var response = await _httpClient.GetAsync($"{BaseUrl}/{id}");
            if (response.IsSuccessStatusCode)
                return await response.Content.ReadFromJsonAsync<AnhDTO>();

            return null;
        }

        public async Task<AnhDTO?> UploadAsync(IFormFile file, Guid? sanPhamChiTietId = null)
        {
            if (file == null || file.Length == 0)
                return null;

            using var content = new MultipartFormDataContent();
            var streamContent = new StreamContent(file.OpenReadStream());
            streamContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(file.ContentType);

            content.Add(streamContent, "file", file.FileName);
            if (sanPhamChiTietId != null)
                content.Add(new StringContent(sanPhamChiTietId.ToString()), "sanPhamChiTietId");

            var response = await _httpClient.PostAsync($"{BaseUrl}/upload", content);
            if (!response.IsSuccessStatusCode) return null;

            return await response.Content.ReadFromJsonAsync<AnhDTO>();
        }

        public async Task<bool> UpdateAsync(Guid id, AnhDTO dto)
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
