using FurryFriends.API.Models;

namespace FurryFriends.Web.Services.IService
{
    public interface ILichSuThaoTacService
    {
        Task<List<LichSuThaoTac>> GetRecentLogsAsync(int take = 5);
        Task AddLogAsync(LichSuThaoTac log);
    }
}
