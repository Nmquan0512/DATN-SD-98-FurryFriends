using FurryFriends.API.Models.DTO;
using FurryFriends.Web.Services.IService;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

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
            try
            {
                var response = await _httpClient.GetAsync(BaseUrl);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<IEnumerable<GiamGiaDTO>>();
            }
            catch (HttpRequestException ex)
            {
                throw new ApplicationException("Lỗi khi lấy danh sách giảm giá", ex);
            }
        }

        public async Task<GiamGiaDTO> GetByIdAsync(Guid id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{BaseUrl}/{id}");
                if (response.StatusCode == HttpStatusCode.NotFound)
                    return null;

                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<GiamGiaDTO>();
            }
            catch (HttpRequestException ex)
            {
                throw new ApplicationException($"Lỗi khi lấy giảm giá với ID {id}", ex);
            }
        }

        public async Task<GiamGiaDTO> CreateAsync(GiamGiaDTO dto)
        {
            try
            {
                ValidateDto(dto);
                var response = await _httpClient.PostAsJsonAsync(BaseUrl, dto);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<GiamGiaDTO>();
            }
            catch (ValidationException)
            {
                throw;
            }
            catch (HttpRequestException ex)
            {
                throw new ApplicationException("Lỗi khi tạo giảm giá", ex);
            }
        }

        public async Task<GiamGiaDTO> UpdateAsync(Guid id, GiamGiaDTO dto)
        {
            try
            {
                if (id != dto.GiamGiaId)
                    throw new ArgumentException("ID không khớp với đối tượng giảm giá");

                ValidateDto(dto);
                var response = await _httpClient.PutAsJsonAsync($"{BaseUrl}/{id}", dto);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<GiamGiaDTO>();
            }
            catch (ValidationException)
            {
                throw;
            }
            catch (ArgumentException)
            {
                throw;
            }
            catch (HttpRequestException ex)
            {
                throw new ApplicationException($"Lỗi khi cập nhật giảm giá với ID {id}", ex);
            }
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"{BaseUrl}/{id}");
                return response.IsSuccessStatusCode;
            }
            catch (HttpRequestException ex)
            {
                throw new ApplicationException($"Lỗi khi xóa giảm giá với ID {id}", ex);
            }
        }

        public async Task<bool> AssignProductsAsync(Guid giamGiaId, List<Guid> productIds)
        {
            try
            {
                if (productIds == null || productIds.Count == 0)
                    throw new ArgumentException("Danh sách sản phẩm không được rỗng");

                var response = await _httpClient.PostAsJsonAsync(
                    $"{BaseUrl}/{giamGiaId}/assign-products",
                    productIds);

                return response.IsSuccessStatusCode;
            }
            catch (ArgumentException)
            {
                throw;
            }
            catch (HttpRequestException ex)
            {
                throw new ApplicationException($"Lỗi khi gán sản phẩm vào giảm giá {giamGiaId}", ex);
            }
        }

        private void ValidateDto(GiamGiaDTO dto)
        {
            var validationResults = new List<ValidationResult>();
            if (!Validator.TryValidateObject(dto, new ValidationContext(dto), validationResults, true))
            {
                throw new ValidationException(string.Join("\n", validationResults.Select(r => r.ErrorMessage)));
            }
        }
    }
}