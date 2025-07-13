using FurryFriends.API.Models;
using FurryFriends.API.Models.DTO;
using FurryFriends.Web.Services.IService;
using Microsoft.AspNetCore.Mvc;
using System.Net;
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

        public async Task<ApiResult<ThanhPhanDTO>> CreateAsync(ThanhPhanDTO dto)
        {
            var response = await _httpClient.PostAsJsonAsync(BaseUrl, dto);

            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadFromJsonAsync<ThanhPhanDTO>();
                return new ApiResult<ThanhPhanDTO> { Data = data };
            }

            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                var errors = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
                return new ApiResult<ThanhPhanDTO>
                {
                    Errors = errors?.Errors?.ToDictionary(e => e.Key, e => e.Value)
                };
            }

            return new ApiResult<ThanhPhanDTO>
            {
                Errors = new Dictionary<string, string[]> { { "", new[] { "Lỗi không xác định!" } } }
            };
        }

        public async Task<ApiResult<bool>> UpdateAsync(Guid id, ThanhPhanDTO dto)
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
                Errors = new Dictionary<string, string[]> { { "", new[] { "Lỗi không xác định khi cập nhật!" } } }
            };
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var response = await _httpClient.DeleteAsync($"{BaseUrl}/{id}");
            return response.IsSuccessStatusCode;
        }
    }
}
