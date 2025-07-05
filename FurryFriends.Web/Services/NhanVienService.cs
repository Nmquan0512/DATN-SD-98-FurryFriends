using FurryFriends.API.Models;
using FurryFriends.API.Repository.IRepository;
using FurryFriends.Web.Services.IService;
using System.Net.Mail;
using System.Text.RegularExpressions;

namespace FurryFriends.Web.Services
{
    public class NhanVienService : INhanVienService
    {
        private readonly HttpClient _httpClient;

        public NhanVienService(HttpClient httpClient)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }

        public async Task<IEnumerable<NhanVien>> GetAllAsync()
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<NhanVien>>("NhanVienApi")
                ?? throw new HttpRequestException("Không thể lấy danh sách nhân viên.");
        }

        public async Task<NhanVien?> GetByIdAsync(Guid nhanVienId)
        {
            if (nhanVienId == Guid.Empty)
                throw new ArgumentException("NhanVienId không hợp lệ.");

            return await _httpClient.GetFromJsonAsync<NhanVien>($"NhanVienApi/{nhanVienId}")
                ?? throw new HttpRequestException($"Không tìm thấy nhân viên với ID {nhanVienId}.");
        }

        public async Task AddAsync(NhanVien nhanVien)
        {
            if (nhanVien == null)
                throw new ArgumentNullException(nameof(nhanVien));

            // Validation
            if (string.IsNullOrWhiteSpace(nhanVien.HoVaTen))
                throw new ArgumentException("Họ và tên không được để trống.");
            if (nhanVien.NgaySinh > DateTime.Now)
                throw new ArgumentException("Ngày sinh không thể trong tương lai.");
            if (string.IsNullOrWhiteSpace(nhanVien.DiaChi))
                throw new ArgumentException("Địa chỉ không được để trống.");
            if (string.IsNullOrWhiteSpace(nhanVien.SDT))
                throw new ArgumentException("Số điện thoại không được để trống.");
            if (!IsValidPhoneNumber(nhanVien.SDT))
                throw new ArgumentException("Số điện thoại phải có 10 chữ số.");
            if (string.IsNullOrWhiteSpace(nhanVien.Email))
                throw new ArgumentException("Email không được để trống.");
            if (!IsValidEmail(nhanVien.Email))
                throw new ArgumentException("Email không hợp lệ.");
            if (string.IsNullOrWhiteSpace(nhanVien.GioiTinh))
                throw new ArgumentException("Giới tính không được để trống.");
            if (!new[] { "Nam", "Nữ" }.Contains(nhanVien.GioiTinh))
                throw new ArgumentException("Giới tính phải là 'Nam' hoặc 'Nữ'.");
            if (nhanVien.TaiKhoanId == Guid.Empty)
                throw new ArgumentException("TaiKhoanId không hợp lệ.");
            if (nhanVien.ChucVuId == Guid.Empty)
                throw new ArgumentException("ChucVuId không hợp lệ.");
            if (nhanVien.NgayTao == default)
                nhanVien.NgayTao = DateTime.Now;
            if (nhanVien.NgayCapNhat == default)
                nhanVien.NgayCapNhat = DateTime.Now;

            nhanVien.TaiKhoan = null;
            nhanVien.ChucVu = null;

            var response = await _httpClient.PostAsJsonAsync("NhanVienApi", nhanVien);
            response.EnsureSuccessStatusCode();
        }

        public async Task UpdateAsync(NhanVien nhanVien)
        {
            if (nhanVien == null)
                throw new ArgumentNullException(nameof(nhanVien));
            if (nhanVien.NhanVienId == Guid.Empty)
                throw new ArgumentException("NhanVienId không hợp lệ.");

            // Validation
            if (string.IsNullOrWhiteSpace(nhanVien.HoVaTen))
                throw new ArgumentException("Họ và tên không được để trống.");
            if (nhanVien.NgaySinh > DateTime.Now)
                throw new ArgumentException("Ngày sinh không thể trong tương lai.");
            if (string.IsNullOrWhiteSpace(nhanVien.DiaChi))
                throw new ArgumentException("Địa chỉ không được để trống.");
            if (string.IsNullOrWhiteSpace(nhanVien.SDT))
                throw new ArgumentException("Số điện thoại không được để trống.");
            if (!IsValidPhoneNumber(nhanVien.SDT))
                throw new ArgumentException("Số điện thoại phải có 10 chữ số.");
            if (string.IsNullOrWhiteSpace(nhanVien.Email))
                throw new ArgumentException("Email không được để trống.");
            if (!IsValidEmail(nhanVien.Email))
                throw new ArgumentException("Email không hợp lệ.");
            if (string.IsNullOrWhiteSpace(nhanVien.GioiTinh))
                throw new ArgumentException("Giới tính không được để trống.");
            if (!new[] { "Nam", "Nữ" }.Contains(nhanVien.GioiTinh))
                throw new ArgumentException("Giới tính phải là 'Nam' hoặc 'Nữ'.");
            if (nhanVien.TaiKhoanId == Guid.Empty)
                throw new ArgumentException("TaiKhoanId không hợp lệ.");
            if (nhanVien.ChucVuId == Guid.Empty)
                throw new ArgumentException("ChucVuId không hợp lệ.");
            if (nhanVien.NgayTao == default)
                throw new ArgumentException("Ngày tạo không được để trống.");
            if (nhanVien.NgayCapNhat == default)
                nhanVien.NgayCapNhat = DateTime.Now;

            nhanVien.TaiKhoan = null;
            nhanVien.ChucVu = null;

            var response = await _httpClient.PutAsJsonAsync($"NhanVienApi/{nhanVien.NhanVienId}", nhanVien);
            response.EnsureSuccessStatusCode();
        }

        public async Task DeleteAsync(Guid nhanVienId)
        {
            if (nhanVienId == Guid.Empty)
                throw new ArgumentException("NhanVienId không hợp lệ.");

            var response = await _httpClient.DeleteAsync($"NhanVienApi/{nhanVienId}");
            response.EnsureSuccessStatusCode();
        }

        public async Task<IEnumerable<NhanVien>> FindByHoVaTenAsync(string hoVaTen)
        {
            if (string.IsNullOrWhiteSpace(hoVaTen))
                throw new ArgumentException("Họ và tên không được để trống.");

            return await _httpClient.GetFromJsonAsync<IEnumerable<NhanVien>>($"NhanVienApi/search?hoVaTen={Uri.EscapeDataString(hoVaTen)}")
                ?? throw new HttpRequestException("Không thể tìm kiếm nhân viên.");
        }

        // Dashboard method
        public async Task<int> GetTotalEmployeesAsync()
        {
            // Simulate database call
            await Task.Delay(100);
            return 25; // Mock data
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        private bool IsValidPhoneNumber(string phoneNumber)
        {
            return Regex.IsMatch(phoneNumber, @"^\d{10}$");
        }
    }
}