using FurryFriends.API.Models.Dtos;

namespace FurryFriends.API.Services.IService
{
	public interface ITaiKhoanService
	{
		Task<LoginResponse?> DangNhapAdminNhanVienAsync(LoginRequest model);
		Task<LoginResponse?> DangNhapKhachHangAsync(LoginRequest model);
	}
}
