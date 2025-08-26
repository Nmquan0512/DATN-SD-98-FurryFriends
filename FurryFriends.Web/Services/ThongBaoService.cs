using FurryFriends.API.Models.DTO;
using FurryFriends.Web.Models;
using FurryFriends.Web.Services.IService;
using Newtonsoft.Json;
using System.Text;

namespace FurryFriends.Web.Services
{
    public class ThongBaoService : IThongBaoService
    {
        private readonly HttpClient _client;
        private readonly string _baseUrl = "https://localhost:7289/api/ThongBao";

        public ThongBaoService(HttpClient client)
        {
            _client = client;
        }

        public async Task<IEnumerable<ThongBaoDTO>> GetAllAsync()
        {
            try
            {
                var res = await _client.GetAsync(_baseUrl);
                if (!res.IsSuccessStatusCode) 
                {
                    System.Diagnostics.Debug.WriteLine($"ThongBaoService GetAllAsync failed: {res.StatusCode} - {res.ReasonPhrase}");
                    return new List<ThongBaoDTO>();
                }

                var json = await res.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<List<ThongBaoDTO>>(json);
                return result ?? new List<ThongBaoDTO>();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"ThongBaoService GetAllAsync exception: {ex.Message}");
                return new List<ThongBaoDTO>();
            }
        }

        public async Task CreateAsync(ThongBaoDTO dto)
        {
            var json = JsonConvert.SerializeObject(dto);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _client.PostAsync(_baseUrl, content);
            response.EnsureSuccessStatusCode();
        }

        public async Task MarkAsReadAsync(Guid id)
        {
            var response = await _client.PutAsync($"{_baseUrl}/mark-as-read/{id}", null);
            response.EnsureSuccessStatusCode();
        }

        public async Task MarkAllAsReadAsync()
        {
            var response = await _client.PutAsync($"{_baseUrl}/mark-all-as-read", null);
            response.EnsureSuccessStatusCode();
        }

        public async Task DeleteAsync(Guid id)
        {
            var response = await _client.DeleteAsync($"{_baseUrl}/{id}");
            response.EnsureSuccessStatusCode();
        }
    }
}
