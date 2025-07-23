using FurryFriends.API.Models;
using FurryFriends.API.Models.DTO;
using FurryFriends.Web.Services.IService;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Net.Http.Json;

namespace FurryFriends.Web.Services
{
    public class GiamGiaService : IGiamGiaService
    {
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "api/GiamGia";

        public GiamGiaService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IEnumerable<GiamGiaDTO>> GetAllAsync()
        {
            var result = await _httpClient.GetFromJsonAsync<IEnumerable<GiamGiaDTO>>(BaseUrl);
            return result ?? new List<GiamGiaDTO>();
        }

        public async Task<GiamGiaDTO?> GetByIdAsync(Guid id)
        {
            var response = await _httpClient.GetAsync($"{BaseUrl}/{id}");
            if (response.IsSuccessStatusCode)
                return await response.Content.ReadFromJsonAsync<GiamGiaDTO>();
            return null;
        }

        public async Task<ApiResult<GiamGiaDTO>> CreateAsync(GiamGiaDTO dto)
        {
            var response = await _httpClient.PostAsJsonAsync(BaseUrl, dto);

            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadFromJsonAsync<GiamGiaDTO>();
                return new ApiResult<GiamGiaDTO> { Data = data };
            }

            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                var errors = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
                return new ApiResult<GiamGiaDTO>
                {
                    Errors = errors?.Errors?.ToDictionary(e => e.Key, e => e.Value)
                };
            }

            return new ApiResult<GiamGiaDTO>
            {
                Errors = new Dictionary<string, string[]>
                {
                    { "", new[] { "Lỗi không xác định!" } }
                }
            };
        }

        public async Task<ApiResult<bool>> UpdateAsync(Guid id, GiamGiaDTO dto)
        {
            var response = await _httpClient.PutAsJsonAsync($"{BaseUrl}/{id}", dto);
            if (response.IsSuccessStatusCode)
            {
                return new ApiResult<bool> { Data = true };
            }

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

        public async Task<bool> AddSanPhamChiTietToGiamGiaAsync(Guid giamGiaId, List<Guid> sanPhamChiTietIds)
        {
            var response = await _httpClient.PostAsJsonAsync($"{BaseUrl}/{giamGiaId}/assign-sanphamchitiet", sanPhamChiTietIds);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var response = await _httpClient.DeleteAsync($"{BaseUrl}/{id}");
            return response.IsSuccessStatusCode;
        }
    }
}
