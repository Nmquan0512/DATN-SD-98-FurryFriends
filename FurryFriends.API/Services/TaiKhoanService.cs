using FurryFriends.API.Models.Dtos;
using FurryFriends.API.Repository.IRepository;
using FurryFriends.API.Services.IService;

namespace FurryFriends.API.Services
{
	public class TaiKhoanService : ITaiKhoanService
	{
		private readonly ITaiKhoanRepository _taiKhoanRepo;
		private readonly INhanVienRepository _nhanVienRepo;
		private readonly IKhachHangRepository _khachHangRepo;
		private readonly IChucVuRepository _chucVuRepo;

		public TaiKhoanService(
			ITaiKhoanRepository taiKhoanRepo,
			INhanVienRepository nhanVienRepo,
			IKhachHangRepository khachHangRepo,
			IChucVuRepository chucVuRepo)
		{
			_taiKhoanRepo = taiKhoanRepo;
			_nhanVienRepo = nhanVienRepo;
			_khachHangRepo = khachHangRepo;
			_chucVuRepo = chucVuRepo;
		}

		public async Task<LoginResponse?> DangNhapAdminNhanVienAsync(LoginRequest model)
		{
			var taiKhoan = await _taiKhoanRepo.FindByUserNameAsync(model.UserName);
			if (taiKhoan == null || taiKhoan.Password != model.Password)
				return null;

			var nhanVienList = await _nhanVienRepo.GetAllAsync();
			var nhanVien = nhanVienList.FirstOrDefault(nv => nv.TaiKhoanId == taiKhoan.TaiKhoanId);
			if (nhanVien == null)
				return null;

			var chucVu = nhanVien.ChucVu;
			if (chucVu == null)
				return null;

			return new LoginResponse
			{
				TaiKhoanId = taiKhoan.TaiKhoanId,
				Role = chucVu.TenChucVu,
				HoTen = nhanVien.HoVaTen
			};
		}

		public async Task<LoginResponse?> DangNhapKhachHangAsync(LoginRequest model)
		{
			var taiKhoan = await _taiKhoanRepo.FindByUserNameAsync(model.UserName);
			if (taiKhoan == null || taiKhoan.Password != model.Password)
				return null;

			if (taiKhoan.KhachHangId == null)
				return null;

			var khachHangList = await _khachHangRepo.GetAllAsync();
			var khachHang = khachHangList.FirstOrDefault(kh => kh.KhachHangId == taiKhoan.KhachHangId);
			if (khachHang == null)
				return null;

			return new LoginResponse
			{
				TaiKhoanId = taiKhoan.TaiKhoanId,
				Role = "KhachHang",
				HoTen = khachHang.TenKhachHang
			};
		}
	}
}
