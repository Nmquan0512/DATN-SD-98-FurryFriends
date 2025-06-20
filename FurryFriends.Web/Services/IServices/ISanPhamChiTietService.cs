namespace FurryFriends.API.Services
{
	public interface ISanPhamChiTietService
	{
		Task<IEnumerable<SanPhamChiTiet>> GetAllAsync();
		Task<SanPhamChiTiet?> GetByIdAsync(Guid id);
		Task<IEnumerable<SanPhamChiTiet>> GetBySanPhamIdAsync(Guid sanPhamId);
		Task<object> GetPagingAsync(Guid? sanPhamId, string? search, int page, int pageSize);
		Task<SanPhamChiTiet> CreateAsync(SanPhamChiTiet model);
		Task<bool> UpdateAsync(Guid id, SanPhamChiTiet model);
		Task<bool> DeleteAsync(Guid id);
		Task<bool> UpdateAnhAsync(Guid id, Guid anhId);
		Task<bool> UpdateMauAsync(Guid id, Guid mauId);
		Task<bool> UpdateKichCoAsync(Guid id, Guid kichCoId);
		Task<bool> UpdateSoLuongGiaAsync(Guid id, SanPhamChiTiet update);
	}
}