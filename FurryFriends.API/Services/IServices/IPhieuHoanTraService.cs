using FurryFriends.API.Models;

namespace FurryFriends.API.Services.IServices
{
	public interface IPhieuHoanTraService
	{
		Task<PhieuHoanTra?> GetByIdAsync(Guid id, bool includeHoaDonChiTiet = false);
		Task<IReadOnlyList<PhieuHoanTra>> GetByHoaDonIdAsync(Guid hoaDonId);
		Task<IReadOnlyList<PhieuHoanTra>> GetByHoaDonChiTietIdAsync(Guid hoaDonChiTietId);
		Task<int> GetTongSoLuongHoanByHdctAsync(Guid hoaDonChiTietId);

		Task<PhieuHoanTra> AddAsync(PhieuHoanTra entity);
		Task UpdateAsync(PhieuHoanTra entity);
		Task UpdateTrangThaiAsync(Guid id, int trangThai);
		Task DeleteAsync(Guid id);

		Task<bool> ExistsAsync(Guid id);
	}
}
