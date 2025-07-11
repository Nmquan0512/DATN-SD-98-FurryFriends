using FurryFriends.API.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FurryFriends.Web.Services.IService
{
    public interface INhanVienService
    {
        Task<IEnumerable<NhanVien>> GetAllAsync();
        Task<NhanVien?> GetByIdAsync(Guid nhanVienId); // Sửa từ taiKhoanId thành nhanVienId
        Task AddAsync(NhanVien nhanVien);
        Task UpdateAsync(NhanVien nhanVien);
        Task DeleteAsync(Guid nhanVienId); // Sửa từ taiKhoanId thành nhanVienId
        Task<IEnumerable<NhanVien>> FindByHoVaTenAsync(string hoVaTen);
        
        // Dashboard method
        Task<int> GetTotalEmployeesAsync();
    }
}