using FurryFriends.API.Models;
using Microsoft.AspNetCore.Components.Forms;

namespace FurryFriends.API.Repository.IRepository
{

    public interface IAnhRepository
    {
        Task<IEnumerable<Anh>> GetAllAsync();
        Task<Anh> GetByIdAsync(Guid id);
        Task AddAsync(Anh entity);
        Task UpdateAsync(Anh entity);
        Task DeleteAsync(Guid id);
        Task<bool> ExistsAsync(Guid id);
    }
}