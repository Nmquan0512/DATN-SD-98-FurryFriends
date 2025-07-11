using FurryFriends.API.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FurryFriends.API.Repository.IRepository
{
    public interface IThanhPhanRepository
    {
        Task<IEnumerable<ThanhPhan>> GetAllAsync();
        Task<ThanhPhan> GetByIdAsync(Guid id);
        Task AddAsync(ThanhPhan entity);
        Task UpdateAsync(ThanhPhan entity);
        Task DeleteAsync(Guid id);
        Task<bool> ExistsAsync(Guid id);
    }
}