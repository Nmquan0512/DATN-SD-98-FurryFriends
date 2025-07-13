using FurryFriends.API.Models;
using FurryFriends.API.Repository.IRepository;
using FurryFriends.Web.Services.IService;
using System.ComponentModel.DataAnnotations;

namespace FurryFriends.Web.Services
{
    public class ChucVuService : IChucVuService
    {
        private readonly HttpClient _httpClient;

        public ChucVuService(HttpClient httpClient)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }

        public async Task<IEnumerable<ChucVu>> GetAllAsync()
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<ChucVu>>("ChucVuApi")
                ?? throw new HttpRequestException("Không thể lấy danh sách chức vụ.");
        }

        public async Task<ChucVu?> GetByIdAsync(Guid chucVuId)
        {
            if (chucVuId == Guid.Empty)
                throw new ArgumentException("ChucVuId không hợp lệ.");

            return await _httpClient.GetFromJsonAsync<ChucVu>($"ChucVuApi/{chucVuId}")
                ?? throw new HttpRequestException($"Không tìm thấy chức vụ với ID {chucVuId}.");
        }

        public async Task AddAsync(ChucVu chucVu)
        {
            var response = await _httpClient.PostAsJsonAsync("ChucVuApi", chucVu);

            if (!response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();

                // Nếu là lỗi 400 và trả về ValidationProblemDetails
                if (response.StatusCode == System.Net.HttpStatusCode.BadRequest &&
                    response.Content.Headers.ContentType?.MediaType == "application/problem+json")
                {
                    throw new ValidationException(content);
                }

                throw new HttpRequestException(content);
            }
        }

        public async Task UpdateAsync(ChucVu chucVu)
        {
            var response = await _httpClient.PutAsJsonAsync($"ChucVuApi/{chucVu.ChucVuId}", chucVu);

            if (!response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();

                if (response.StatusCode == System.Net.HttpStatusCode.BadRequest &&
                    response.Content.Headers.ContentType?.MediaType == "application/problem+json")
                {
                    throw new ValidationException(content);
                }

                throw new HttpRequestException(content);
            }

        }

        public async Task DeleteAsync(Guid chucVuId)
        {
            if (chucVuId == Guid.Empty)
                throw new ArgumentException("ChucVuId không hợp lệ.");

            var response = await _httpClient.DeleteAsync($"ChucVuApi/{chucVuId}");
            response.EnsureSuccessStatusCode();
        }

        public async Task<IEnumerable<ChucVu>> FindByTenChucVuAsync(string tenChucVu)
        {
            if (string.IsNullOrWhiteSpace(tenChucVu))
                throw new ArgumentException("Tên chức vụ không được để trống.");

            return await _httpClient.GetFromJsonAsync<IEnumerable<ChucVu>>($"ChucVuApi/search?tenChucVu={tenChucVu}")
                ?? throw new HttpRequestException("Không thể tìm kiếm chức vụ.");
        }
    }
}