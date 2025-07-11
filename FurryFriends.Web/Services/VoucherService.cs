using FurryFriends.API.Models;
using FurryFriends.Web.Services.IService;
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
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateAsync(Guid id, Voucher voucher)
        {
            var response = await _httpClient.PutAsJsonAsync($"api/Voucher/{id}", voucher);
            return response.IsSuccessStatusCode;
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