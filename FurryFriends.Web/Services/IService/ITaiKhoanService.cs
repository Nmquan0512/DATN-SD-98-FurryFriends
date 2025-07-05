using FurryFriends.API.Models;

namespace FurryFriends.Web.Services.IService
{
    public interface ITaiKhoanService
    {
        Task<IEnumerable<TaiKhoan>> GetAllAsync();
        Task<TaiKhoan?> GetByIdAsync(Guid taiKhoanId);
        Task AddAsync(TaiKhoan taiKhoan);
        Task UpdateAsync(TaiKhoan taiKhoan);
        Task DeleteAsync(Guid taiKhoanId);
        Task<IEnumerable<TaiKhoan>> FindByUserNameAsync(string userName);
        Task<IEnumerable<TaiKhoan>> GetAllTaiKhoanAsync();
    }
}