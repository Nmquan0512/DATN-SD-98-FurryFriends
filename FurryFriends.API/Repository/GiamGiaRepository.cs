using FurryFriends.API.Data;
using FurryFriends.API.Models;
using FurryFriends.API.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace FurryFriends.API.Repositories
{
    public class GiamGiaRepository : IGiamGiaRepository
    {
        private readonly AppDbContext _context;
        private readonly DbSet<GiamGia> _dbSet;

        public GiamGiaRepository(AppDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<GiamGia>();
        }

        public async Task<IEnumerable<GiamGia>> GetAllAsync()
        {
            return await _dbSet
                .Include(g => g.DotGiamGiaSanPhams)
                .ToListAsync();
        }

        public async Task<GiamGia?> GetByIdAsync(Guid id)
        {
            return await _dbSet
                .Include(g => g.DotGiamGiaSanPhams)
                .FirstOrDefaultAsync(g => g.GiamGiaId == id);
        }

        public async Task<IEnumerable<GiamGia>> FindAsync(Expression<Func<GiamGia, bool>> predicate)
        {
            return await _dbSet
                .Include(g => g.DotGiamGiaSanPhams)
                .Where(predicate)
                .ToListAsync();
        }

        public async Task AddAsync(GiamGia entity)
        {
            await _dbSet.AddAsync(entity);
        }

        public void Update(GiamGia entity)
        {
            _dbSet.Update(entity);
        }

        public void Delete(GiamGia entity)
        {
            _dbSet.Remove(entity);
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}