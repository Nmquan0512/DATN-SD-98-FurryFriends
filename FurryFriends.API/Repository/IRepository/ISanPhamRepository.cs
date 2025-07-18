using FurryFriends.API.Models;
using System.Linq.Expressions;

namespace FurryFriends.API.Repository.IRepository
{
    public interface ISanPhamRepository
    {
        Task<IEnumerable<SanPham>> GetAllAsync();
        Task<SanPham?> GetByIdAsync(Guid id);
        Task<IEnumerable<SanPham>> FindAsync(Expression<Func<SanPham, bool>> predicate);
        Task AddAsync(SanPham entity);
        void Update(SanPham entity);
        Task UpdateAsync(SanPham entity);
        void Delete(SanPham entity);
        Task DeleteAsync(Guid id);
        Task<bool> ExistsAsync(Guid id);
        Task SaveAsync();
    }
}
