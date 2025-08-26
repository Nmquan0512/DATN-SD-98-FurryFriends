
using FurryFriends.Web.ViewModels;

namespace FurryFriends.Web.Services.IService
{
    public interface IPhieuHoanTraService
    {
        Task<IEnumerable<PhieuHoanTraViewModel>> GetAllAsync();
        Task<PhieuHoanTraViewModel> GetByIdAsync(Guid id);
        Task<IEnumerable<PhieuHoanTraViewModel>> GetByKhachHangAsync(Guid khachHangId);
        Task<bool> CreateAsync(ViewModels.PhieuHoanTraCreateRequest request);
        Task<bool> UpdateAsync(Guid id, ViewModels.PhieuHoanTraUpdateRequest request);
        Task<bool> DeleteAsync(Guid id);
    }
}
