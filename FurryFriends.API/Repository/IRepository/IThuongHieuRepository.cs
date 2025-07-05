using FurryFriends.API.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FurryFriends.API.Repository.IRepository
{
    public interface IThuongHieuRepository
    {
        Task<IEnumerable<ThuongHieu>> GetAllAsync();
        Task<ThuongHieu> GetByIdAsync(Guid id);
        Task AddAsync(ThuongHieu entity);
        Task UpdateAsync(ThuongHieu entity);
        Task DeleteAsync(Guid id);
        Task<bool> ExistsAsync(Guid id);
    }
}
