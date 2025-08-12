using FurryFriends.API.Models;

namespace FurryFriends.API.Repository.IRepository
{
    public interface IHinhThucThanhToanRepository
    {
        Task<IEnumerable<HinhThucThanhToan>> GetAllAsync();
        Task<HinhThucThanhToan?> GetByIdAsync(Guid id);
    }

}
