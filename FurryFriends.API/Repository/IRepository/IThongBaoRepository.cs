using FurryFriends.API.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FurryFriends.API.Repository.IRepository
{
    public interface IThongBaoRepository
    {
        Task<IEnumerable<ThongBao>> GetAllAsync();
        Task<ThongBao> GetByIdAsync(Guid id);
        Task AddAsync(ThongBao thongBao);
        Task UpdateAsync(ThongBao thongBao);
        Task DeleteAsync(Guid id);
        Task MarkAsReadAsync(Guid id);
    }
} 