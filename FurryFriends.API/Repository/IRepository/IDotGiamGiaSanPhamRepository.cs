using FurryFriends.API.Models;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace FurryFriends.API.Repository.IRepository
{
    public interface IDotGiamGiaSanPhamRepository
    {
        Task<IEnumerable<DotGiamGiaSanPham>> GetAllAsync();
        Task<DotGiamGiaSanPham?> GetByIdAsync(Guid id);
        Task<IEnumerable<DotGiamGiaSanPham>> FindAsync(Expression<Func<DotGiamGiaSanPham, bool>> predicate);
        Task AddAsync(DotGiamGiaSanPham entity);
        void Update(DotGiamGiaSanPham entity);
        void Delete(DotGiamGiaSanPham entity);
        Task SaveAsync();
    }
}