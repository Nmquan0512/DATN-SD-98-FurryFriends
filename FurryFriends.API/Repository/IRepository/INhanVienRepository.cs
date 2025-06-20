using FurryFriends.API.Models;

namespace FurryFriends.API.Repository.IRepository
{
    public interface INhanVienRepository
    {
        Task<IEnumerable<NhanVien>> GetAllAsync();
        Task<NhanVien?> GetByIdAsync(Guid nhanVienId);
        Task AddAsync(NhanVien nhanVien);
        Task UpdateAsync(NhanVien nhanVien);
        Task DeleteAsync(Guid nhanVienId);
        Task<IEnumerable<NhanVien>> FindByNameAsync(string hoVaTen);
        Task<bool> CheckTaiKhoanExistsAsync(Guid taiKhoanId);
        Task<bool> CheckChucVuExistsAsync(Guid chucVuId);
        Task<bool> CheckTaiKhoanLinkedAsync(Guid taiKhoanId, Guid? nhanVienId = null);
    }
}