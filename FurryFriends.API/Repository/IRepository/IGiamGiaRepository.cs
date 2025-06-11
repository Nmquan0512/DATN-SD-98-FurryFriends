using FurryFriends.API.Models;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace FurryFriends.API.Repository.IRepository
{
    public interface IGiamGiaRepository
    {
        Task<IEnumerable<GiamGia>> GetAllAsync();
        Task<GiamGia?> GetByIdAsync(Guid id);
        Task<IEnumerable<GiamGia>> FindAsync(Expression<Func<GiamGia, bool>> predicate);
        Task AddAsync(GiamGia entity);
        void Update(GiamGia entity);
        void Delete(GiamGia entity);
        Task SaveAsync();
    }
}