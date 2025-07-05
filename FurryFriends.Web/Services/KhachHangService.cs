using FurryFriends.API.Models;
using FurryFriends.Web.Services;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using FurryFriends.Web.Services.IService;

namespace FurryFriends.Web.Services
{
    public class KhachHangService : IKhachHangService
    {
        private readonly HttpClient _httpClient;

        public KhachHangService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IEnumerable<KhachHang>> GetAllAsync()
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<KhachHang>>("api/KhachHang");
        }

        public async Task<KhachHang> GetByIdAsync(Guid id)
        {
            return await _httpClient.GetFromJsonAsync<KhachHang>($"api/KhachHang/{id}");
        }

        public async Task<bool> CreateAsync(KhachHang khachHang)
        {
            var response = await _httpClient.PostAsJsonAsync("api/KhachHang", khachHang);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateAsync(Guid id, KhachHang khachHang)
        {
            var response = await _httpClient.PutAsJsonAsync($"api/KhachHang/{id}", khachHang);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var response = await _httpClient.DeleteAsync($"api/KhachHang/{id}");
            return response.IsSuccessStatusCode;
        }

        public Task<string?> GetAllKhachHangAsync()
        {
            throw new NotImplementedException();
        }

        public Task<string?> GetKhachHangByIdAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task DeleteKhachHangAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task AddKhachHangAsync(KhachHang model)
        {
            throw new NotImplementedException();
        }

        public Task UpdateKhachHangAsync(KhachHang model)
        {
            throw new NotImplementedException();
        }

        public async Task<KhachHang?> FindByEmailAsync(string email)
        {
            var all = await GetAllAsync();
            return all.FirstOrDefault(kh => kh.EmailCuaKhachHang == email);
        }

        public async Task<int> GetTotalCustomersAsync()
        {
            var all = await GetAllAsync();
            return all.Count();
        }
    }
}