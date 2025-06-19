using FurryFriends.API.Models;
using FurryFriends.API.Repository.IRepository;
using FurryFriends.Web.Services.IService;

namespace FurryFriends.Web.Services
{
	public class TaiKhoanService : ITaiKhoanService
	{
		private readonly ITaiKhoanRepository _taiKhoanRepository;

		public TaiKhoanService(ITaiKhoanRepository taiKhoanRepository)
		{
			_taiKhoanRepository = taiKhoanRepository;
		}

		public async Task<IEnumerable<TaiKhoan>> GetAllAsync()
		{
			return await _taiKhoanRepository.GetAllAsync();
		}

		public async Task<TaiKhoan?> GetByIdAsync(Guid taiKhoanId)
		{
			if (taiKhoanId == Guid.Empty)
				throw new ArgumentException("TaiKhoanId không hợp lệ.");

			return await _taiKhoanRepository.GetByIdAsync(taiKhoanId);
		}

		public async Task AddAsync(TaiKhoan taiKhoan)
		{
			if (taiKhoan == null)
				throw new ArgumentNullException(nameof(taiKhoan));

			// Validation
			if (string.IsNullOrWhiteSpace(taiKhoan.UserName))
				throw new ArgumentException("Tên đăng nhập không được để trống.");
			if (taiKhoan.UserName.Length > 50)
				throw new ArgumentException("Tên đăng nhập không được vượt quá 50 ký tự.");
			if (string.IsNullOrWhiteSpace(taiKhoan.Password))
				throw new ArgumentException("Mật khẩu không được để trống.");
			if (taiKhoan.Password.Length > 100)
				throw new ArgumentException("Mật khẩu không được vượt quá 100 ký tự.");
			if (taiKhoan.NgayTaoTaiKhoan == default)
				taiKhoan.NgayTaoTaiKhoan = DateTime.Now;
			if (taiKhoan.NgayCapNhatCuoiCung == null)
				taiKhoan.NgayCapNhatCuoiCung = DateTime.Now;

			// Bỏ qua navigation properties
			taiKhoan.NhanVien = null;
			taiKhoan.KhachHang = null;
			taiKhoan.SanPhams = null;
			taiKhoan.Vouchers = null;
			taiKhoan.HoaDons = null;

			// Repository sẽ kiểm tra UserName, KhachHangId
			await _taiKhoanRepository.AddAsync(taiKhoan);
		}

		public async Task UpdateAsync(TaiKhoan taiKhoan)
		{
			if (taiKhoan == null)
				throw new ArgumentNullException(nameof(taiKhoan));
			if (taiKhoan.TaiKhoanId == Guid.Empty)
				throw new ArgumentException("TaiKhoanId không hợp lệ.");

			// Validation
			if (string.IsNullOrWhiteSpace(taiKhoan.UserName))
				throw new ArgumentException("Tên đăng nhập không được để trống.");
			if (taiKhoan.UserName.Length > 50)
				throw new ArgumentException("Tên đăng nhập không được vượt quá 50 ký tự.");
			if (string.IsNullOrWhiteSpace(taiKhoan.Password))
				throw new ArgumentException("Mật khẩu không được để trống.");
			if (taiKhoan.Password.Length > 100)
				throw new ArgumentException("Mật khẩu không được vượt quá 100 ký tự.");
			if (taiKhoan.NgayTaoTaiKhoan == default)
				throw new ArgumentException("Ngày tạo không được để trống.");
			if (taiKhoan.NgayCapNhatCuoiCung == null)
				taiKhoan.NgayCapNhatCuoiCung = DateTime.Now;

			// Bỏ qua navigation properties
			taiKhoan.NhanVien = null;
			taiKhoan.KhachHang = null;
			taiKhoan.SanPhams = null;
			taiKhoan.Vouchers = null;
			taiKhoan.HoaDons = null;

			// Repository sẽ kiểm tra UserName, KhachHangId
			await _taiKhoanRepository.UpdateAsync(taiKhoan);
		}

		public async Task DeleteAsync(Guid taiKhoanId)
		{
			if (taiKhoanId == Guid.Empty)
				throw new ArgumentException("TaiKhoanId không hợp lệ.");

			await _taiKhoanRepository.DeleteAsync(taiKhoanId);
		}

		public async Task<TaiKhoan?> FindByUserNameAsync(string userName)
		{
			if (string.IsNullOrWhiteSpace(userName))
				throw new ArgumentException("Tên đăng nhập không được để trống.");

			return await _taiKhoanRepository.FindByUserNameAsync(userName);
		}
	}
}
