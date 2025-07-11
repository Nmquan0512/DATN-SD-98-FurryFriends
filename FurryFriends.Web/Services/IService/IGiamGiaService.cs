using FurryFriends.API.Models;

namespace FurryFriends.Web.Services.IService
{
    public interface IGiamGiaService
    {
        Task<IEnumerable<GiamGia>> GetAllAsync();
        Task<GiamGia?> GetByIdAsync(Guid id);
        Task<bool> CreateAsync(GiamGia giamGia);
        Task<bool> UpdateAsync(Guid id, GiamGia giamGia);
        Task<bool> DeleteAsync(Guid id);
    }
}