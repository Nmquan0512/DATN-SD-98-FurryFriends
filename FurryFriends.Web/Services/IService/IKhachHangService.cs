

using FurryFriends.API.Models;

namespace FurryFriends.Web.Services.IService
{
    public interface IKhachHangService
    {
        Task AddKhachHangAsync(KhachHang model);
        Task DeleteKhachHangAsync(Guid id);
        Task<string?> GetAllKhachHangAsync();
        Task<string?> GetKhachHangByIdAsync(Guid id);
        Task UpdateKhachHangAsync(KhachHang model);
    }
}
