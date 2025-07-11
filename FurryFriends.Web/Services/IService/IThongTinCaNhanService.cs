using FurryFriends.API.Models.DTO;
using FurryFriends.Web.Models;

namespace FurryFriends.Web.Services.IService
{
	public interface IThongTinCaNhanService
	{
		Task<ThongTinCaNhanViewModel?> GetThongTinCaNhanAsync(Guid taiKhoanId);
		Task<bool> UpdateThongTinCaNhanAsync(Guid taiKhoanId, ThongTinCaNhanViewModel dto);
		Task<bool> DoiMatKhauAsync(Guid taiKhoanId, string matKhauCu, string matKhauMoi);
	}
}
