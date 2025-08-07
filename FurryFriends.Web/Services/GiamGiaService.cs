using FurryFriends.API.Models.DTO;
using FurryFriends.Web.Services.IService;
using Microsoft.AspNetCore.Mvc; // Dùng cho ValidationProblemDetails
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace FurryFriends.Web.Services
{
    // Lớp Exception tùy chỉnh để chứa thông tin lỗi từ API
    public class ApiException : Exception
    {
        public HttpStatusCode StatusCode { get; }
        public string Content { get; }

        public ApiException(string message, HttpStatusCode statusCode, string content) : base(message)
        {
            StatusCode = statusCode;
            Content = content;
        }
    }

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
            var response = await _httpClient.GetAsync(BaseUrl);
            await HandleError(response); // Kiểm tra lỗi
            return await response.Content.ReadFromJsonAsync<IEnumerable<GiamGiaDTO>>() ?? new List<GiamGiaDTO>();
        }

        public async Task<GiamGiaDTO> GetByIdAsync(Guid id)
        {
            var response = await _httpClient.GetAsync($"{BaseUrl}/{id}");
            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return null;
            }
            await HandleError(response); // Kiểm tra các lỗi khác
            return await response.Content.ReadFromJsonAsync<GiamGiaDTO>();
        }

        public async Task<GiamGiaDTO> CreateAsync(GiamGiaDTO dto)
        {
            var response = await _httpClient.PostAsJsonAsync(BaseUrl, dto);
            await HandleError(response);
            return await response.Content.ReadFromJsonAsync<GiamGiaDTO>();
        }

        public async Task<bool> UpdateAsync(Guid id, GiamGiaDTO dto)
        {
            var response = await _httpClient.PutAsJsonAsync($"{BaseUrl}/{id}", dto);
            await HandleError(response);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var response = await _httpClient.DeleteAsync($"{BaseUrl}/{id}");
            // Chỉ cần kiểm tra NotFound cho Delete, các lỗi khác sẽ bị ném ra
            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return false;
            }
            await HandleError(response);
            return response.IsSuccessStatusCode;
        }

        // Hàm hỗ trợ chung để kiểm tra và ném lỗi
        private async Task HandleError(HttpResponseMessage response)
        {
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                // Ném ra một exception tùy chỉnh chứa mã trạng thái và nội dung lỗi
                throw new ApiException(
                    $"Lỗi từ API: {response.ReasonPhrase}",
                    response.StatusCode,
                    errorContent);
            }
        }
    }
}