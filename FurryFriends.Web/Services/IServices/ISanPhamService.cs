using FurryFriends.API.Models;
using FurryFriends.API.ViewModels;

namespace FurryFriends.API.Services
{
	public interface ISanPhamService
	{
		Task<(List<SanPhamViewModel> Data, int TotalCount)> GetSanPhamViewAsync(string? search, int page, int pageSize);
		Task<SanPhamViewModel?> GetByIdAsync(Guid id);
		Task<SanPham> CreateAsync(SanPham model);
		Task<bool> UpdateAsync(Guid id, SanPham model);
		Task<bool> DeleteAsync(Guid id);
	}
}
