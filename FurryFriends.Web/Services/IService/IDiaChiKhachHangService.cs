using FurryFriends.API.Models;

namespace FurryFriends.Web.Service.IService
{
    public interface IDiaChiKhachHangService
    {
        Task<IEnumerable<DiaChiKhachHang>> GetAllAsync();
        Task<DiaChiKhachHang> GetByIdAsync(Guid id);
        Task<IEnumerable<DiaChiKhachHang>> GetByKhachHangIdAsync(Guid khachHangId);
        Task AddAsync(DiaChiKhachHang diaChi);
        Task UpdateAsync(DiaChiKhachHang diaChi);
        Task DeleteAsync(Guid id);
    }
}