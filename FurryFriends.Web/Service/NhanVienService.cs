using FurryFriends.API.Models;
using FurryFriends.API.Repository.IRepository;
using FurryFriends.Web.Service.IService;
using System.Net.Mail;
using System.Text.RegularExpressions;

namespace FurryFriends.Web.Service
{
	public class NhanVienService : INhanVienService
	{
		private readonly INhanVienRepository _nhanVienRepository;

		public NhanVienService(INhanVienRepository nhanVienRepository)
		{
			_nhanVienRepository = nhanVienRepository;
		}

		public async Task<IEnumerable<NhanVien>> GetAllAsync()
		{
			return await _nhanVienRepository.GetAllAsync();
		}

		public async Task<NhanVien?> GetByIdAsync(Guid nhanVienId)
		{
			if (nhanVienId == Guid.Empty)
				throw new ArgumentException("NhanVienId không hợp lệ.");

			return await _nhanVienRepository.GetByIdAsync(nhanVienId);
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
				nhanVien.NgayTao = DateTime.Now; // Đồng bộ với repository
			if (nhanVien.NgayCapNhat == default)
				nhanVien.NgayCapNhat = DateTime.Now; // Đồng bộ

			// Bỏ qua navigation properties
			nhanVien.TaiKhoan = null;
			nhanVien.ChucVu = null;

			// Repository sẽ kiểm tra TaiKhoanId, ChucVuId, và liên kết
			await _nhanVienRepository.AddAsync(nhanVien);
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
				nhanVien.NgayCapNhat = DateTime.Now; // Đồng bộ

			// Bỏ qua navigation properties
			nhanVien.TaiKhoan = null;
			nhanVien.ChucVu = null;

			// Repository sẽ kiểm tra TaiKhoanId, ChucVuId, và liên kết
			await _nhanVienRepository.UpdateAsync(nhanVien);
		}

		public async Task DeleteAsync(Guid nhanVienId)
		{
			if (nhanVienId == Guid.Empty)
				throw new ArgumentException("NhanVienId không hợp lệ.");

			await _nhanVienRepository.DeleteAsync(nhanVienId);
		}

		public async Task<IEnumerable<NhanVien>> FindByHoVaTenAsync(string hoVaTen)
		{
			if (string.IsNullOrWhiteSpace(hoVaTen))
				throw new ArgumentException("Họ và tên không được để trống.");

			return await _nhanVienRepository.FindByNameAsync(hoVaTen);
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
