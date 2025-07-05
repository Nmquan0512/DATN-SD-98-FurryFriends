using FurryFriends.API.Models;

namespace FurryFriends.Web.Services.IService
{
	public interface IKhachHangService
	{
		Task<IEnumerable<KhachHang>> GetAllKhachHangAsync();
		Task<KhachHang> GetKhachHangByIdAsync(Guid id);
		Task AddKhachHangAsync(KhachHang model);
		Task UpdateKhachHangAsync(KhachHang model);
		Task DeleteKhachHangAsync(Guid id);
	}
}
