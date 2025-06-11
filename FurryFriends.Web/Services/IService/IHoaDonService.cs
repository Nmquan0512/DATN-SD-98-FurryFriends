using FurryFriends.API.Models;

namespace FurryFriends.Web.Services.IService
{
    public interface IHoaDonService
    {
        Task<IEnumerable<HoaDon>> GetHoaDonListAsync();
        Task<HoaDon> GetHoaDonByIdAsync(Guid hoaDonId);
        Task<IEnumerable<HoaDon>> SearchHoaDonAsync(string keyword);
        Task<byte[]> ExportHoaDonToPdfAsync(Guid hoaDonId);
    }
}