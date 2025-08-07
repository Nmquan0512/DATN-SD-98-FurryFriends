using FurryFriends.API.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FurryFriends.API.Repository.IRepository
{
    public interface IDotGiamGiaSanPhamRepository
    {
        // Các hàm không thay đổi
        Task<IEnumerable<DotGiamGiaSanPham>> GetAllAsync();
        Task<DotGiamGiaSanPham> GetByIdAsync(Guid id);
        Task<IEnumerable<DotGiamGiaSanPham>> GetByGiamGiaIdAsync(Guid giamGiaId);
        Task<bool> ExistsAsync(Guid id);

        // Các hàm thay đổi
        Task AddRangeAsync(IEnumerable<DotGiamGiaSanPham> entities);
        void DeleteRange(IEnumerable<DotGiamGiaSanPham> entities); // Xóa một danh sách
        Task SaveAsync();
    }
}