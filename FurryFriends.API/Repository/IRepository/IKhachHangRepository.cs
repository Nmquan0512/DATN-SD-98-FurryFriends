using FurryFriends.API.Models;

namespace FurryFriends.API.Repository.IRepository
{
	public interface IKhachHangRepository
	{
		Task<IEnumerable<KhachHang>> GetAllAsync();            // Lấy tất cả khách hàng
		Task<KhachHang> GetByIdAsync(Guid id);                 // Lấy 1 khách hàng theo ID
		Task AddAsync(KhachHang khachHang);                    // Thêm mới
		Task UpdateAsync(KhachHang khachHang);                 // Cập nhật
		Task DeleteAsync(Guid id);

	}
}
