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
            var res = await _client.GetAsync(_baseUrl);
            if (!res.IsSuccessStatusCode) return new List<ThongBaoDTO>();

            var json = await res.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<List<ThongBaoDTO>>(json);
        }

        public async Task CreateAsync(ThongBaoDTO dto)
        {
            var json = JsonConvert.SerializeObject(dto);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _client.PostAsync(_baseUrl, content);
            response.EnsureSuccessStatusCode();
        }
    }
}
