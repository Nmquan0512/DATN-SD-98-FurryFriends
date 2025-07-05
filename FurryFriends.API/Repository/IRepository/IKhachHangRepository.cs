using FurryFriends.API.Models;

namespace FurryFriends.API.Repository.IRepository
{
	public interface IKhachHangRepository
	{
        Task<IEnumerable<KhachHang>> GetAllAsync();
        Task<KhachHang?> GetByIdAsync(Guid id);
        Task<KhachHang> AddAsync(KhachHang khachHang);
        Task<KhachHang?> UpdateAsync(Guid id, KhachHang khachHang);
        Task<bool> DeleteAsync(Guid id);
    }
}
