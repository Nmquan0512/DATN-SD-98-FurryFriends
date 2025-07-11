using FurryFriends.API.Models;
using FurryFriends.API.Models.DTO;
using FurryFriends.Web.Services.IService;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;

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

        public async Task<ApiResult<ChatLieuDTO>> CreateAsync(ChatLieuDTO dto)
        {
            var response = await _httpClient.PostAsJsonAsync(BaseUrl, dto);

            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadFromJsonAsync<ChatLieuDTO>();
                return new ApiResult<ChatLieuDTO> { Data = data };
            }

            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                var errors = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
                return new ApiResult<ChatLieuDTO>
                {
                    Errors = errors?.Errors.ToDictionary(e => e.Key, e => e.Value)
                };
            }

            return new ApiResult<ChatLieuDTO>
            {
                Errors = new Dictionary<string, string[]>
        {
            { "", new[] { "Lỗi không xác định!" } }
        }
            };
        }


        public async Task<ApiResult<bool>> UpdateAsync(Guid id, ChatLieuDTO dto)
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

        public async Task<bool> DeleteAsync(Guid id)
        {
            var response = await _httpClient.DeleteAsync($"{BaseUrl}/{id}");
            return response.IsSuccessStatusCode;
        }
    }
}
