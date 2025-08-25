using FurryFriends.API.Models.DTO;
using FurryFriends.Web.Models;

namespace FurryFriends.Web.Services.IService
{
    public interface IThongBaoService
    {
        Task<IEnumerable<ThongBaoDTO>> GetAllAsync();
        Task CreateAsync(ThongBaoDTO dto);
    }
}
