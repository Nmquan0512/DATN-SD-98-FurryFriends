using FurryFriends.API.Models;
using FurryFriends.Web.Models;

namespace FurryFriends.Web.Services.IService
{
	public interface ITaiKhoanService
	{
		Task<LoginResponse?> DangNhapAdminAsync(LoginRequest model);
		Task<IEnumerable<TaiKhoan>> GetAllAsync();
		Task<TaiKhoan?> GetByIdAsync(Guid taiKhoanId);
		Task AddAsync(TaiKhoan taiKhoan);
		Task UpdateAsync(TaiKhoan taiKhoan);
		Task DeleteAsync(Guid taiKhoanId);
		Task<IEnumerable<TaiKhoan>> FindByUserNameAsync(string userName);
		// ITaiKhoanService.cs
		Task<LoginResponse?> DangNhapKhachHangAsync(LoginRequest model);
	}
}