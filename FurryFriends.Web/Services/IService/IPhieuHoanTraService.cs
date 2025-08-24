using FurryFriends.Web.ViewModels;

namespace FurryFriends.Web.Services.IService
{
	public interface IPhieuHoanTraService
	{
		Task<IEnumerable<PhieuHoanTraViewModel>> GetByHoaDonIdAsync(Guid hoaDonId);
		Task<PhieuHoanTraViewModel?> GetByIdAsync(Guid id);
		Task<bool> CreateAsync(PhieuHoanTraViewModel model);
		Task<bool> UpdateTrangThaiAsync(Guid id, int trangThai);
		Task<bool> DeleteAsync(Guid id);
	}
}
