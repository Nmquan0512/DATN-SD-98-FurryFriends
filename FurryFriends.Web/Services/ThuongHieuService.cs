using FurryFriends.API.Models;
using FurryFriends.API.Models.DTO;
using FurryFriends.Web.Services.IService;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;

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

        public async Task<ApiResult<ThuongHieuDTO>> CreateAsync(ThuongHieuDTO dto)
        {
            var response = await _httpClient.PostAsJsonAsync(BaseUrl, dto);

            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadFromJsonAsync<ThuongHieuDTO>();
                return new ApiResult<ThuongHieuDTO> { Data = data };
            }

            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                var errors = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
                return new ApiResult<ThuongHieuDTO>
                {
                    Errors = errors?.Errors?.ToDictionary(e => e.Key, e => e.Value)
                };
            }

            return new ApiResult<ThuongHieuDTO>
            {
                Errors = new() { { "", new[] { "Lỗi không xác định khi tạo!" } } }
            };
        }

        public async Task<ApiResult<bool>> UpdateAsync(Guid id, ThuongHieuDTO dto)
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
