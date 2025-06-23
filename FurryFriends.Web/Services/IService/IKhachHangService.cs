using FurryFriends.API.Models;

namespace FurryFriends.Web.Services.IService
{
    public interface IKhachHangService
    {
        Task<IEnumerable<KhachHang>> GetAllAsync();
        Task<KhachHang> GetByIdAsync(Guid id);
        Task<bool> CreateAsync(KhachHang khachHang);
        Task<bool> UpdateAsync(Guid id, KhachHang khachHang);
        Task<bool> DeleteAsync(Guid id);
        Task<string?> GetAllKhachHangAsync();
        Task<string?> GetKhachHangByIdAsync(Guid id);
        Task DeleteKhachHangAsync(Guid id);
        Task AddKhachHangAsync(KhachHang model);
        Task UpdateKhachHangAsync(KhachHang model);
    }
}
