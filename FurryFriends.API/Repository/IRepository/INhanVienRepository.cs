using FurryFriends.API.Models;

namespace FurryFriends.API.Repository.IRepository
{
    public interface INhanVienRepository
    {
        Task<NhanVien?> GetByIdAsync(Guid id);
        Task<IEnumerable<NhanVien>> GetAllAsync();
        Task AddAsync(NhanVien nhanVien);
        Task UpdateAsync(NhanVien nhanVien);
        Task DeleteAsync(Guid id);
        Task<IEnumerable<NhanVien>> FindByNameAsync(string hoVaTen);
    }
}