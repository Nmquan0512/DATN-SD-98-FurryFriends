using FurryFriends.API.Models;
using FurryFriends.API.Models.DTO;

namespace FurryFriends.Web.Services.IService
{
    public interface IKichCoService
    {
        Task<IEnumerable<KichCoDTO>> GetAllAsync();
        Task<KichCoDTO> GetByIdAsync(Guid id);
        Task<ApiResult<KichCoDTO>> CreateAsync(KichCoDTO dto);
        Task<ApiResult<bool>> UpdateAsync(Guid id, KichCoDTO dto);
        Task<bool> DeleteAsync(Guid id);
    }
}
