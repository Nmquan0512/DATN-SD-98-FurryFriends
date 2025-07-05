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
            var response = await _httpClient.GetAsync("api/Voucher");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<IEnumerable<Voucher>>() ?? new List<Voucher>();
            }

            return new List<Voucher>();
        }

        public async Task<Voucher?> GetByIdAsync(Guid id)
        {
            var response = await _httpClient.GetAsync($"api/Voucher/{id}");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<Voucher>();
            }

            return null;
        }

        public async Task<Voucher> CreateAsync(Voucher voucher)
        {
            var response = await _httpClient.PostAsJsonAsync("api/Voucher", voucher);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<Voucher>();
        }

        public async Task<Voucher?> UpdateAsync(Guid id, Voucher voucher)
        {
            var response = await _httpClient.PutAsJsonAsync($"api/Voucher/{id}", voucher);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<Voucher>();
            }

            return null;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var response = await _httpClient.DeleteAsync($"api/Voucher/{id}");
            return response.IsSuccessStatusCode;
        }
    }
}
