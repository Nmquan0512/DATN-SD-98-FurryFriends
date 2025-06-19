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

        public async Task<IEnumerable<KhachHang>> GetAllKhachHangAsync()
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<KhachHang>>("api/KhachHang");
        }

        public async Task<KhachHang> GetKhachHangByIdAsync(Guid id)
        {
            return await _httpClient.GetFromJsonAsync<KhachHang>($"api/KhachHang/{id}");
        }

        public async Task AddKhachHangAsync(KhachHang khachHang)
        {
            await _httpClient.PostAsJsonAsync("api/KhachHang", khachHang);
        }

        public async Task UpdateKhachHangAsync(KhachHang khachHang)
        {
            await _httpClient.PutAsJsonAsync($"api/KhachHang/{khachHang.KhachHangId}", khachHang);
        }

        public async Task DeleteKhachHangAsync(Guid id)
        {
            await _httpClient.DeleteAsync($"api/KhachHang/{id}");
        }

        Task<string?> IKhachHangService.GetAllKhachHangAsync()
        {
            throw new NotImplementedException();
        }

        Task<string?> IKhachHangService.GetKhachHangByIdAsync(Guid id)
        {
            throw new NotImplementedException();
        }
    }
}