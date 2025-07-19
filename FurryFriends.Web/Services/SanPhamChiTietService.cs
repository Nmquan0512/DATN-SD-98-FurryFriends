using FurryFriends.API.Models;
using FurryFriends.API.Models.DTO;
using FurryFriends.Web.Services.IService;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Net.Http.Json;

namespace FurryFriends.Web.Services
{
    public class SanPhamChiTietService : ISanPhamChiTietService
    {
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "api/SanPhamChiTiet";

        public SanPhamChiTietService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IEnumerable<SanPhamChiTietDTO>> GetAllAsync()
        {
            var result = await _httpClient.GetFromJsonAsync<IEnumerable<SanPhamChiTietDTO>>(BaseUrl);
            return result ?? new List<SanPhamChiTietDTO>();
        }

        public async Task<SanPhamChiTietDTO?> GetByIdAsync(Guid id)
        {
            var response = await _httpClient.GetAsync($"{BaseUrl}/{id}");
            if (response.IsSuccessStatusCode)
                return await response.Content.ReadFromJsonAsync<SanPhamChiTietDTO>();
            return null;
        }

        public async Task<ApiResult<SanPhamChiTietDTO>> CreateAsync(SanPhamChiTietDTO dto)
        {
            Console.WriteLine($"[SanPhamChiTietService] Gửi CreateAsync: {System.Text.Json.JsonSerializer.Serialize(dto)}");
            var response = await _httpClient.PostAsJsonAsync(BaseUrl, dto);

            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadFromJsonAsync<SanPhamChiTietDTO>();
                return new ApiResult<SanPhamChiTietDTO> { Data = data };
            }

            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                var content = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"[SanPhamChiTietService] Lỗi 400: {content}");
                var errors = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
                return new ApiResult<SanPhamChiTietDTO>
                {
                    Errors = errors?.Errors?.ToDictionary(e => e.Key, e => e.Value)
                };
            }

            var unknown = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"[SanPhamChiTietService] Lỗi không xác định: {unknown}");
            return new ApiResult<SanPhamChiTietDTO>
            {
                Errors = new() { { "", new[] { "Lỗi không xác định khi tạo!" } } }
            };
        }

        public async Task<ApiResult<bool>> UpdateAsync(Guid id, SanPhamChiTietDTO dto)
        {
            var response = await _httpClient.PutAsJsonAsync($"{BaseUrl}/{id}", dto);

            if (response.IsSuccessStatusCode)
                return new ApiResult<bool> { Data = true };

            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                var errors = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
                return new ApiResult<bool>
                {
                    Data = false,
                    Errors = errors?.Errors?.ToDictionary(e => e.Key, e => e.Value)
                };
            }

            return new ApiResult<bool>
            {
                Data = false,
                Errors = new() { { "", new[] { "Lỗi không xác định khi cập nhật!" } } }
            };
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var response = await _httpClient.DeleteAsync($"{BaseUrl}/{id}");
            return response.IsSuccessStatusCode;
        }
    }
}
