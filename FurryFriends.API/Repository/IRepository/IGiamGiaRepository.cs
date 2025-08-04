using FurryFriends.API.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FurryFriends.API.Repository.IRepository
{
    public interface IGiamGiaRepository
    {
        Task<IEnumerable<GiamGia>> GetAllAsync(bool includeProducts = false);
        Task<GiamGia> GetByIdAsync(Guid id, bool includeProducts = false);
        Task AddAsync(GiamGia entity);
        Task UpdateAsync(GiamGia entity);
        Task DeleteAsync(Guid id);
        Task<bool> ExistsAsync(Guid id);
        Task<bool> TenGiamGiaExistsAsync(string tenGiamGia, Guid? excludeId = null);
    }
}