using FurryFriends.API.Models;
using FurryFriends.API.Models.DTO;
using FurryFriends.Web.Services.IService;
using Microsoft.AspNetCore.Mvc; // Cần thêm using này để dùng ValidationProblemDetails
using System;
using System.Collections.Generic;
using System.Linq; // Cần thêm using này
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace FurryFriends.Web.Services
{
    // Lớp này đã được sửa để sử dụng ApiResult<T> giống như SanPhamChiTietService
    public class SanPhamService : ISanPhamService
    {
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "api/SanPhams";

        public SanPhamService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IEnumerable<SanPhamDTO>> GetAllAsync()
        {
            var response = await _httpClient.GetAsync(BaseUrl);
            if (!response.IsSuccessStatusCode) return new List<SanPhamDTO>();
            return await response.Content.ReadFromJsonAsync<IEnumerable<SanPhamDTO>>() ?? new List<SanPhamDTO>();
        }

        public async Task<SanPhamDTO?> GetByIdAsync(Guid id)
        {
            var response = await _httpClient.GetAsync($"{BaseUrl}/{id}");
            if (response.StatusCode == HttpStatusCode.NotFound)
                return null;

            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<SanPhamDTO>();
        }

    
        public async Task<ApiResult<SanPhamDTO>> CreateAsync(SanPhamDTO dto)
        {
            var response = await _httpClient.PostAsJsonAsync(BaseUrl, dto);

            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadFromJsonAsync<SanPhamDTO>();
                return new ApiResult<SanPhamDTO> { Data = data };
            }

            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                var errors = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
                return new ApiResult<SanPhamDTO>
                {
                    Errors = errors?.Errors?.ToDictionary(e => e.Key, e => e.Value)
                };
            }

            return new ApiResult<SanPhamDTO> { Errors = new() { { "", new[] { "Lỗi không xác định khi tạo sản phẩm!" } } } };
        }

        // SỬA LẠI PHƯƠNG THỨC UPDATE
        public async Task<ApiResult<bool>> UpdateAsync(Guid id, SanPhamDTO dto)
        {
            var response = await _httpClient.PutAsJsonAsync($"{BaseUrl}/{id}", dto);

            if (response.IsSuccessStatusCode)
            {
                // API trả về thành công (204 NoContent), chỉ cần báo lại là thành công.
                return new ApiResult<bool> { Data = true };
            }

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return new ApiResult<bool> { Data = false, Errors = new() { { "", new[] { "Không tìm thấy sản phẩm để cập nhật." } } } };
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

            return new ApiResult<bool> { Data = false, Errors = new() { { "", new[] { "Lỗi không xác định khi cập nhật!" } } } };
        }

        // SỬA LẠI PHƯƠNG THỨC DELETE
        public async Task<ApiResult<bool>> DeleteAsync(Guid id)
        {
            var response = await _httpClient.DeleteAsync($"{BaseUrl}/{id}");

            if (response.IsSuccessStatusCode)
            {
                return new ApiResult<bool> { Data = true };
            }

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return new ApiResult<bool> { Data = false, Errors = new() { { "", new[] { "Không tìm thấy sản phẩm để xóa." } } } };
            }

            return new ApiResult<bool> { Data = false, Errors = new() { { "", new[] { "Lỗi không xác định khi xóa!" } } } };
        }

        // Các phương thức lọc và thống kê đã tốt, giữ nguyên.
        public async Task<(IEnumerable<SanPhamDTO> Data, int Total)> GetFilteredAsync(string? loai, int page, int pageSize)
        {
            var url = $"{BaseUrl}/filter?loai={loai}&page={page}&pageSize={pageSize}";
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