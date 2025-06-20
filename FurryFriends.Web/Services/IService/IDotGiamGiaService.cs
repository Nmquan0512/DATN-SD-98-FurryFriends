using FurryFriends.API.Models;

namespace FurryFriends.Web.Services.IService
{
    public interface IDotGiamGiaService
    {
        Task<IEnumerable<DotGiamGiaSanPham>> GetAllAsync();
        Task<DotGiamGiaSanPham?> GetByIdAsync(Guid id);
        Task<bool> AddAsync(DotGiamGiaSanPham dotGiamGia);
        Task<bool> UpdateAsync(Guid id, DotGiamGiaSanPham dotGiamGia);
        Task<bool> DeleteAsync(Guid id);
    }
}