using FurryFriends.API.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FurryFriends.API.Repository.IRepository
{
    public interface IDotGiamGiaSanPhamRepository
    {
        Task<IEnumerable<DotGiamGiaSanPham>> GetAllAsync();
        Task<DotGiamGiaSanPham?> GetByIdAsync(Guid id);
        Task AddAsync(DotGiamGiaSanPham entity);
        Task UpdateAsync(DotGiamGiaSanPham entity);
        Task DeleteByGiamGiaIdAsync(Guid giamGiaId);
        Task DeleteAsync(Guid id);
        Task<bool> ExistsAsync(Guid id);
        Task<IEnumerable<DotGiamGiaSanPham>> GetBySanPhamIdAsync(Guid sanPhamId);
        Task<IEnumerable<DotGiamGiaSanPham>> GetByGiamGiaIdAsync(Guid giamGiaId);
    }
}
