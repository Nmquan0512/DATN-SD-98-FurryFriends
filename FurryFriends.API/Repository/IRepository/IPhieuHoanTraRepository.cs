using FurryFriends.API.Models;

namespace FurryFriends.API.Repository.IRepository
{
    public interface IPhieuHoanTraRepository
    {
        Task<IEnumerable<PhieuHoanTra>> GetAllAsync();
        Task<PhieuHoanTra> GetByIdAsync(Guid id);
        Task<IEnumerable<PhieuHoanTra>> GetByKhachHangAsync(Guid khachHangId);
        Task AddAsync(PhieuHoanTra entity);
        Task UpdateAsync(PhieuHoanTra entity);
        Task DeleteAsync(Guid id);

        // ✅ Thêm mới
        Task<HoaDonChiTiet?> GetHoaDonChiTietWithRelationsAsync(Guid hoaDonChiTietId);
        Task<int> GetTongSoLuongDaHoanAsync(Guid hoaDonChiTietId);
    }
}
