using FurryFriends.API.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FurryFriends.API.Repository.IRepository
{
    public interface IDotGiamGiaSanPhamRepository
    {
        Task<IEnumerable<DotGiamGiaSanPham>> GetAllAsync();
        Task<DotGiamGiaSanPham> GetByIdAsync(Guid id);
        Task AddAsync(DotGiamGiaSanPham entity);
        Task AddRangeAsync(IEnumerable<DotGiamGiaSanPham> entities);
        Task UpdateAsync(DotGiamGiaSanPham entity);
        Task DeleteAsync(Guid id);
        Task DeleteByGiamGiaIdAsync(Guid giamGiaId);
        Task<bool> ExistsAsync(Guid id);
        Task<IEnumerable<DotGiamGiaSanPham>> GetByGiamGiaIdAsync(Guid giamGiaId);
        Task<IEnumerable<DotGiamGiaSanPham>> GetBySanPhamChiTietIdAsync(Guid sanPhamChiTietId);
    }
}