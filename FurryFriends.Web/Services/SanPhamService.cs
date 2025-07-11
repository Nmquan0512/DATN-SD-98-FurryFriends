using FurryFriends.API.Models.DTO;
using FurryFriends.Web.Services.IService;
using System.Net.Http;
using System.Net.Http.Json;

namespace FurryFriends.Web.Services
{
    public class SanPhamService : ISanPhamService
    {
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "api/SanPham";

        public SanPhamService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IEnumerable<SanPhamDTO>> GetAllAsync()
        {
            var result = await _httpClient.GetFromJsonAsync<IEnumerable<SanPhamDTO>>(BaseUrl);
            return result ?? new List<SanPhamDTO>();
        }
            
        public async Task<SanPhamDTO> GetByIdAsync(Guid id)
            {
            var response = await _httpClient.GetAsync($"{BaseUrl}/{id}");
            if (response.IsSuccessStatusCode)
                return await response.Content.ReadFromJsonAsync<SanPhamDTO>();
            return null;
        }

        public async Task<SanPhamDTO> CreateAsync(SanPhamDTO dto)
        {
            var response = await _httpClient.PostAsJsonAsync(BaseUrl, dto);
            if (response.IsSuccessStatusCode)
                return await response.Content.ReadFromJsonAsync<SanPhamDTO>();
            return null;
        }

        public async Task<bool> UpdateAsync(Guid id, SanPhamDTO dto)
        {
            var response = await _httpClient.PutAsJsonAsync($"{BaseUrl}/{id}", dto);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var response = await _httpClient.DeleteAsync($"{BaseUrl}/{id}");
            return response.IsSuccessStatusCode;
        }

        public async Task<(IEnumerable<SanPhamDTO> Data, int TotalItems)> GetFilteredAsync(string? loai, int page, int pageSize)
        {
            var url = $"{BaseUrl}/filter?loai={loai}&page={page}&pageSize={pageSize}";

            var response = await _httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode)
                return (new List<SanPhamDTO>(), 0);

            var result = await response.Content.ReadFromJsonAsync<SanPhamFilterResponse>();

            return (result.Items ?? new List<SanPhamDTO>(), result.TotalItems);
        }
            
        private class SanPhamFilterResponse
            {
            public int TotalItems { get; set; }
            public int Page { get; set; }
            public int PageSize { get; set; }
            public IEnumerable<SanPhamDTO> Items { get; set; }
        }
    }
} 