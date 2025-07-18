using FurryFriends.API.Data;
using FurryFriends.API.Models;
using FurryFriends.API.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace FurryFriends.API.Repository
{
    public class AnhRepository : IAnhRepository
    {
        private readonly AppDbContext _context;
        private readonly DbSet<Anh> _dbSet;

        public AnhRepository(AppDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<Anh>();
        }

        public async Task<IEnumerable<Anh>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public async Task<Anh?> GetByIdAsync(Guid id)
        {
            return await _dbSet.FirstOrDefaultAsync(x => x.AnhId == id);
        }

        public async Task<IEnumerable<Anh>> FindAsync(Expression<Func<Anh, bool>> predicate)
        {
            return await _dbSet.Where(predicate).ToListAsync();
        }

        public async Task<Anh?> FindOneAsync(Expression<Func<Anh, bool>> predicate)
        {
            return await _dbSet.FirstOrDefaultAsync(predicate);
        }

        public async Task<bool> ExistsAsync(Guid id)
        {
            return await _dbSet.AnyAsync(x => x.AnhId == id);
        }

        public async Task AddAsync(Anh entity)
        {
            await _dbSet.AddAsync(entity);
        }

        public void Update(Anh entity)
        {
            _dbSet.Update(entity);
        }

        public void Delete(Anh entity)
        {
            _dbSet.Remove(entity);
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
