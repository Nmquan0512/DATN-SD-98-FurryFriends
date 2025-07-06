using FurryFriends.API.Models;

namespace FurryFriends.API.Repository.IRepository
{
    public interface IHoaDonRepository
    {
        Task<IEnumerable<HoaDon>> GetHoaDonListAsync();
        Task<HoaDon> GetHoaDonByIdAsync(Guid hoaDonId);
        Task<IEnumerable<HoaDon>> SearchHoaDonAsync(Func<HoaDon, bool> predicate);
        Task<byte[]> ExportHoaDonToPdfAsync(Guid hoaDonId);
    }

}
