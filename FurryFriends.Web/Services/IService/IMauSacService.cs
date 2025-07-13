using FurryFriends.API.Models;
using FurryFriends.API.Models.DTO;

namespace FurryFriends.Web.Services.IService
{
    public interface IMauSacService
    {
        Task<IEnumerable<MauSacDTO>> GetAllAsync();
        Task<MauSacDTO> GetByIdAsync(Guid id);
        Task<ApiResult<MauSacDTO>> CreateAsync(MauSacDTO dto);
        Task<ApiResult<bool>> UpdateAsync(Guid id, MauSacDTO dto);
        Task<bool> DeleteAsync(Guid id);
    }
}
