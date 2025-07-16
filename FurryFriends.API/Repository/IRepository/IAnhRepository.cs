using FurryFriends.API.Models;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace FurryFriends.API.Repository.IRepository
{
    public interface IAnhRepository
    {
        Task<IEnumerable<Anh>> GetAllAsync();
        Task<Anh?> GetByIdAsync(Guid id);
        Task<IEnumerable<Anh>> FindAsync(Expression<Func<Anh, bool>> predicate);
        Task<Anh?> FindOneAsync(Expression<Func<Anh, bool>> predicate);
        Task<bool> ExistsAsync(Guid id);

        Task AddAsync(Anh entity);
        void Update(Anh entity);
        void Delete(Anh entity);
        Task SaveAsync();
    }
}
