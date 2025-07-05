using FurryFriends.API.Models;
using FurryFriends.Web.Service.IService;

namespace FurryFriends.Web.Service
{
    public class DiaChiKhachHangService : IDiaChiKhachHangService
    {
        private readonly HttpClient _httpClient;

        public DiaChiKhachHangService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IEnumerable<DiaChiKhachHang>> GetAllAsync()
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<DiaChiKhachHang>>("api/DiaChiKhachHang");
        }

        public async Task<DiaChiKhachHang> GetByIdAsync(Guid id)
        {
            return await _httpClient.GetFromJsonAsync<DiaChiKhachHang>($"api/DiaChiKhachHang/{id}");
        }

        public async Task<IEnumerable<DiaChiKhachHang>> GetByKhachHangIdAsync(Guid khachHangId)
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<DiaChiKhachHang>>($"api/DiaChiKhachHang/khachhang/{khachHangId}");
        }

        public async Task AddAsync(DiaChiKhachHang diaChi)
        {
            await _httpClient.PostAsJsonAsync("api/DiaChiKhachHang", diaChi);
        }

        public async Task UpdateAsync(DiaChiKhachHang diaChi)
        {
            await _httpClient.PutAsJsonAsync($"api/DiaChiKhachHang/{diaChi.DiaChiId}", diaChi);
        }

        public async Task DeleteAsync(Guid id)
        {
            await _httpClient.DeleteAsync($"api/DiaChiKhachHang/{id}");
        }
    }
}