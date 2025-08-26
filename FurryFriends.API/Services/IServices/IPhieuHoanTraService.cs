using FurryFriends.API.Models;
using FurryFriends.API.Models.DTO;

namespace FurryFriends.API.Services.IServices
{
    public interface IPhieuHoanTraService
    {
        Task<IEnumerable<PhieuHoanTraDto>> GetAllAsync();
        Task<PhieuHoanTraDto> GetByIdAsync(Guid id);
        Task<IEnumerable<PhieuHoanTraDto>> GetByKhachHangAsync(Guid khachHangId);
        Task<bool> CreateAsync(PhieuHoanTraCreateRequest request);
        Task<bool> UpdateAsync(Guid id, PhieuHoanTraUpdateRequest request);
        Task<bool> DeleteAsync(Guid id);
    }
}
