using FurryFriends.API.Models;

namespace FurryFriends.API.Repository.IRepository
{
    public interface ILichSuThaoTacRepository
    {
        Task AddAsync(LichSuThaoTac log);
        Task<List<LichSuThaoTac>> GetRecentAsync(int take);
    }
}
