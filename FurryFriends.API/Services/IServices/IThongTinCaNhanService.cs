using FurryFriends.API.Models.DTO;

namespace FurryFriends.API.Services.IServices
{
	public interface IThongTinCaNhanService
	{
		Task<ThongTinCaNhanDTO?> GetThongTinCaNhanAsync(Guid taiKhoanId);
		Task<bool> UpdateThongTinCaNhanAsync(Guid taiKhoanId, CapNhatThongTinCaNhanDTO dto);
		Task<bool> DoiMatKhauAsync(Guid taiKhoanId, string matKhauCu, string matKhauMoi);
	}
}
