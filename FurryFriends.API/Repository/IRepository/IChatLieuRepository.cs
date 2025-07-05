using FurryFriends.API.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FurryFriends.API.Repository.IRepository
{
    public interface IChatLieuRepository
    {
        Task<IEnumerable<ChatLieu>> GetAllAsync();
        Task<ChatLieu> GetByIdAsync(Guid id);
        Task AddAsync(ChatLieu entity);
        Task UpdateAsync(ChatLieu entity);
        Task DeleteAsync(Guid id);
        Task<bool> ExistsAsync(Guid id);
    }
}