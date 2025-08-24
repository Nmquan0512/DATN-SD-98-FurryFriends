using FurryFriends.API.Models;

namespace FurryFriends.API.Repository.IRepository
{
	public interface IPhieuHoanTraRepository
	{
		Task<PhieuHoanTra?> GetByIdAsync(Guid id, bool includeHoaDonChiTiet = false);
		Task<IReadOnlyList<PhieuHoanTra>> GetByHoaDonIdAsync(Guid hoaDonId);                 // lấy theo Hóa đơn (join qua HoaDonChiTiet)
		Task<IReadOnlyList<PhieuHoanTra>> GetByHoaDonChiTietIdAsync(Guid hoaDonChiTietId);   // lấy theo dòng HĐCT
		Task<int> GetTongSoLuongHoanByHdctAsync(Guid hoaDonChiTietId);                       // tổng SL đã hoàn của 1 dòng HĐCT

		Task<PhieuHoanTra> AddAsync(PhieuHoanTra entity);      // tạo phiếu
		Task UpdateAsync(PhieuHoanTra entity);                  // cập nhật (VD: sửa lý do)
		Task UpdateTrangThaiAsync(Guid id, int trangThai);      // đổi trạng thái
		Task DeleteAsync(Guid id);                              // xóa phiếu (nếu cần)

		Task<bool> ExistsAsync(Guid id);
	}
}
