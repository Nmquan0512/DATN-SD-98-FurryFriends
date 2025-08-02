using FurryFriends.API.Models;

namespace FurryFriends.Web.Services.IService
{
    public interface IHinhThucThanhToanService
    {
        Task<IEnumerable<HinhThucThanhToan>> GetAllAsync();
    }

}
