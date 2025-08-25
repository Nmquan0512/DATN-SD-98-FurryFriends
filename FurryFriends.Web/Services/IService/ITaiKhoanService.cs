using FurryFriends.API.Models;
using LoginRequest = FurryFriends.API.Models.LoginRequest;
using LoginResponse = FurryFriends.API.Models.LoginResponse;

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
        Task<(LoginResponse? Response, string? ErrorMessage)> DangNhapAdminAsync(LoginRequest model);

        Task<(LoginResponse? Response, string? ErrorMessage)> DangNhapKhachHangAsync(LoginRequest model);
    }
}