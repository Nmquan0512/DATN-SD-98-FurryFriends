using FurryFriends.API.Models;

namespace FurryFriends.API.Repository.IRepository
{
	public interface IDiaChiKhachHangRepository
	{
		Task<IEnumerable<DiaChiKhachHang>> GetAllAsync();               // Lấy tất cả địa chỉ
		Task<DiaChiKhachHang> GetByIdAsync(Guid id);                    // Lấy địa chỉ theo ID
		Task<IEnumerable<DiaChiKhachHang>> GetByKhachHangIdAsync(Guid khachHangId); // Lấy địa chỉ theo KhachHangId
		Task AddAsync(DiaChiKhachHang diaChi);                          // Thêm mới
		Task UpdateAsync(DiaChiKhachHang diaChi);                       // Cập nhật
		Task DeleteAsync(Guid id);
	}
}
