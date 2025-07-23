using FurryFriends.API.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FurryFriends.API.Repository.IRepository
{
    public interface IGiamGiaRepository
    {
        Task<IEnumerable<GiamGia>> GetAllAsync();
        Task<GiamGia?> GetByIdAsync(Guid id);
        Task AddAsync(GiamGia entity);
        Task UpdateAsync(GiamGia entity);
        Task DeleteAsync(Guid id);
        Task<IEnumerable<GiamGia>> GetAllWithSanPhamChiTietAsync();
        Task<GiamGia?> GetByIdWithSanPhamChiTietAsync(Guid id);

        Task<bool> ExistsAsync(Guid id);
        Task<bool> TenGiamGiaExistsAsync(string tenGiamGia, Guid? excludeId = null);
        Task<IEnumerable<GiamGia>> GetActiveDiscountsAsync();
    }
}
