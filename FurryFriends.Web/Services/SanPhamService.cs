using FurryFriends.API.Models.DTO;
using FurryFriends.Web.Services.IService;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace FurryFriends.Web.Services
{
    public class SanPhamService : ISanPhamService
    {
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "api/SanPhams";

        public SanPhamService(HttpClient httpClient)
        {
            _httpClient = httpClient ;
        }

        public async Task<IEnumerable<SanPhamDTO>> GetAllAsync()
        {
            var response = await _httpClient.GetAsync(BaseUrl);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<IEnumerable<SanPhamDTO>>() ?? new List<SanPhamDTO>();
        }

        public async Task<SanPhamDTO> GetByIdAsync(Guid id)
        {
            var response = await _httpClient.GetAsync($"{BaseUrl}/{id}");

            if (response.StatusCode == HttpStatusCode.NotFound)
                throw new KeyNotFoundException($"Không tìm thấy sản phẩm với ID {id}");

            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<SanPhamDTO>();
        }

        public async Task<SanPhamDTO> CreateAsync(SanPhamDTO dto)
        {
            var response = await _httpClient.PostAsJsonAsync(BaseUrl, dto);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<SanPhamDTO>();
        }

        public async Task<SanPhamDTO> UpdateAsync(Guid id, SanPhamDTO dto)
        {
            var response = await _httpClient.PutAsJsonAsync($"{BaseUrl}/{id}", dto);
            if (response.StatusCode == HttpStatusCode.NotFound)
                throw new KeyNotFoundException($"Không tìm thấy sản phẩm với ID {id}");

            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<SanPhamDTO>() ?? throw new Exception("Cập nhật sản phẩm thất bại");
        }
        public async Task<bool> DeleteAsync(Guid id)
        {
            var response = await _httpClient.DeleteAsync($"{BaseUrl}/{id}");
            if (response.StatusCode == HttpStatusCode.NotFound)
                throw new KeyNotFoundException($"Không tìm thấy sản phẩm với ID {id}");

            response.EnsureSuccessStatusCode();
            return response.IsSuccessStatusCode;
        }

        public async Task<(IEnumerable<SanPhamDTO> Data, int Total)> GetFilteredAsync(string? loai, int page, int pageSize)
        {
            var url = $"{BaseUrl}/filter?loaiSanPham={loai}&page={page}&pageSize={pageSize}";
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<FilterResponse>();
            return (Data: result?.Items ?? new List<SanPhamDTO>(), Total: result?.TotalItems ?? 0);
        }

        private class FilterResponse
        {
            public int TotalItems { get; set; }
            public IEnumerable<SanPhamDTO> Items { get; set; }
        }

        public async Task<int> GetTotalProductsAsync()
        {
            var response = await _httpClient.GetAsync($"{BaseUrl}/total");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<int>();
        }

        public async Task<IEnumerable<SanPhamDTO>> GetTopSellingProductsAsync(int top)
        {
            var response = await _httpClient.GetAsync($"{BaseUrl}/top-selling?top={top}");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<IEnumerable<SanPhamDTO>>() ?? new List<SanPhamDTO>();
        }
    }
}