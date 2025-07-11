using FurryFriends.API.Models;
using FurryFriends.API.Repository.IRepository;
using FurryFriends.Web.Services.IService;

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
            if (chucVu == null)
                throw new ArgumentNullException(nameof(chucVu));

            // Validation
            if (string.IsNullOrWhiteSpace(chucVu.TenChucVu))
                throw new ArgumentException("Tên chức vụ không được để trống.");
            if (chucVu.TenChucVu.Length > 50)
                throw new ArgumentException("Tên chức vụ không được vượt quá 50 ký tự.");
            if (string.IsNullOrWhiteSpace(chucVu.MoTaChucVu))
                throw new ArgumentException("Mô tả chức vụ không được để trống.");
            if (chucVu.MoTaChucVu.Length > 250)
                throw new ArgumentException("Mô tả chức vụ không được vượt quá 250 ký tự.");
            if (chucVu.NgayTao == default)
                chucVu.NgayTao = DateTime.Now;
            if (chucVu.NgayCapNhat == default)
                chucVu.NgayCapNhat = DateTime.Now;

            chucVu.NhanViens = null; // Bỏ qua navigation property

            var response = await _httpClient.PostAsJsonAsync("ChucVuApi", chucVu);
            response.EnsureSuccessStatusCode();
        }

        public async Task UpdateAsync(ChucVu chucVu)
        {
            if (chucVu == null)
                throw new ArgumentNullException(nameof(chucVu));
            if (chucVu.ChucVuId == Guid.Empty)
                throw new ArgumentException("ChucVuId không hợp lệ.");

            // Validation
            if (string.IsNullOrWhiteSpace(chucVu.TenChucVu))
                throw new ArgumentException("Tên chức vụ không được để trống.");
            if (chucVu.TenChucVu.Length > 50)
                throw new ArgumentException("Tên chức vụ không được vượt quá 50 ký tự.");
            if (string.IsNullOrWhiteSpace(chucVu.MoTaChucVu))
                throw new ArgumentException("Mô tả chức vụ không được để trống.");
            if (chucVu.MoTaChucVu.Length > 250)
                throw new ArgumentException("Mô tả chức vụ không được vượt quá 250 ký tự.");
            if (chucVu.NgayTao == default)
                throw new ArgumentException("Ngày tạo không được để trống.");
            if (chucVu.NgayCapNhat == default)
                chucVu.NgayCapNhat = DateTime.Now;

            chucVu.NhanViens = null; // Bỏ qua navigation property

            var response = await _httpClient.PutAsJsonAsync($"ChucVuApi/{chucVu.ChucVuId}", chucVu);
            response.EnsureSuccessStatusCode();
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