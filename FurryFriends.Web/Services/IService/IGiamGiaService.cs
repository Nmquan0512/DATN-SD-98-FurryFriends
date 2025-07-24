using FurryFriends.API.Models;
using FurryFriends.API.Models.DTO;

namespace FurryFriends.Web.Services.IService
{
    public interface IGiamGiaService
    {
        Task<IEnumerable<GiamGiaDTO>> GetAllAsync();
        Task<GiamGiaDTO?> GetByIdAsync(Guid id);
        Task<ApiResult<GiamGiaDTO>> CreateAsync(GiamGiaDTO dto);
        Task<ApiResult<bool>> UpdateAsync(Guid id, GiamGiaDTO dto);
        Task<bool> AddSanPhamChiTietToGiamGiaAsync(Guid giamGiaId, List<Guid> sanPhamChiTietIds);
        Task<bool> DeleteAsync(Guid id);
    }
}
