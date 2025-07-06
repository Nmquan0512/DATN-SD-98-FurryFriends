using FurryFriends.API.Models;

namespace FurryFriends.API.Repository.IRepository
{
    public interface ITaiKhoanRepository
    {
        Task<IEnumerable<TaiKhoan>> GetAllAsync();
        Task<TaiKhoan?> GetByIdAsync(Guid id);
        Task AddAsync(TaiKhoan taiKhoan);
        Task UpdateAsync(TaiKhoan taiKhoan);
        Task DeleteAsync(Guid id);
        Task<TaiKhoan?> FindByUserNameAsync(string userName);
    }
}