using FurryFriends.API.Models;
using FurryFriends.Web.Services.IService;
using System.ComponentModel.DataAnnotations;
using System.Net.Http.Json;

namespace FurryFriends.Web.Services
{
    public class VoucherService : IVoucherService
    {
        private readonly HttpClient _httpClient;

        public VoucherService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IEnumerable<Voucher>> GetAllAsync()
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<Voucher>>("api/Voucher");
        }

        public async Task<Voucher?> GetByIdAsync(Guid id)
        {
            return await _httpClient.GetFromJsonAsync<Voucher>($"api/Voucher/{id}");
        }

        public async Task<bool> CreateAsync(Voucher voucher)
        {
            var response = await _httpClient.PostAsJsonAsync("api/Voucher", voucher);

            if (!response.IsSuccessStatusCode)
            {
                // Nếu là lỗi validation
                if (response.StatusCode == System.Net.HttpStatusCode.BadRequest &&
                    response.Content?.Headers.ContentType?.MediaType == "application/problem+json")
                {
                    var content = await response.Content.ReadAsStringAsync();
                    throw new ValidationException(content); // gửi JSON lỗi lên Controller
                }

                // Trường hợp lỗi khác
                throw new HttpRequestException(await response.Content.ReadAsStringAsync());
            }

            return true;
        }

        public async Task<bool> UpdateAsync(Guid id, Voucher voucher)
        {
            var response = await _httpClient.PutAsJsonAsync($"api/Voucher/{id}", voucher);

            if (!response.IsSuccessStatusCode)
            {
                if (response.StatusCode == System.Net.HttpStatusCode.BadRequest &&
                    response.Content?.Headers.ContentType?.MediaType == "application/problem+json")
                {
                    var content = await response.Content.ReadAsStringAsync();
                    throw new ValidationException(content);
                }

                throw new HttpRequestException(await response.Content.ReadAsStringAsync());
            }

            return true;
        }


        public async Task<bool> DeleteAsync(Guid id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"api/Voucher/{id}");
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                // Log error here if needed
                return false;
            }
        }
    }
}