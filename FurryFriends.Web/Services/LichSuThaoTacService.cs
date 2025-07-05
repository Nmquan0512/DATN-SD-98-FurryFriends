using FurryFriends.API.Models;
using FurryFriends.Web.Services.IService;
using Newtonsoft.Json;
using System.Text;

namespace FurryFriends.Web.Services
{
    public class LichSuThaoTacService : ILichSuThaoTacService
    {
        private readonly HttpClient _httpClient;

        public LichSuThaoTacService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<LichSuThaoTac>> GetRecentLogsAsync(int take = 5)
        {
            var response = await _httpClient.GetAsync($"/api/LichSuThaoTac/recent?take={take}");

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"API trả về lỗi {response.StatusCode}: {error}");
            }

            var content = await response.Content.ReadAsStringAsync();

            var apiLogs = JsonConvert.DeserializeObject<List<FurryFriends.API.Models.LichSuThaoTac>>(content);

            if (apiLogs == null)
            {
                throw new Exception("Không thể parse JSON từ API: " + content);
            }

            return apiLogs.Select(x => new LichSuThaoTac
            {
                HanhDong = x.HanhDong,
                NoiDung = x.NoiDung,
                ThoiGian = x.ThoiGian
            }).ToList();
        }


        public async Task AddLogAsync(LichSuThaoTac log)
        {
            var json = JsonConvert.SerializeObject(log);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("/api/LichSuThaoTac", content);
            response.EnsureSuccessStatusCode();
        }
    }
}
