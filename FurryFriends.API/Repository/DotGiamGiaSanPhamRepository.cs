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
    public class DotGiamGiaSanPhamRepository : IDotGiamGiaSanPhamRepository
    {
        private readonly AppDbContext _context;
        private readonly DbSet<DotGiamGiaSanPham> _dbSet;

        public DotGiamGiaSanPhamRepository(AppDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<DotGiamGiaSanPham>();
        }

        public async Task<IEnumerable<DotGiamGiaSanPham>> GetAllAsync()
        {
            return await _dbSet
                .Include(d => d.GiamGias)
                .Include(d => d.SanPhams)
                .ToListAsync();
        }

        public async Task<DotGiamGiaSanPham?> GetByIdAsync(Guid id)
        {
            return await _dbSet
                .Include(d => d.GiamGias)
                .Include(d => d.SanPhams)
                .FirstOrDefaultAsync(d => d.DotGiamGiaSanPhamId == id);
        }

        public async Task<IEnumerable<DotGiamGiaSanPham>> FindAsync(Expression<Func<DotGiamGiaSanPham, bool>> predicate)
        {
            return await _dbSet
                .Include(d => d.GiamGias)
                .Include(d => d.SanPhams)
                .Where(predicate)
                .ToListAsync();
        }

        public async Task AddAsync(DotGiamGiaSanPham entity)
        {
            await _dbSet.AddAsync(entity);
        }

        public void Update(DotGiamGiaSanPham entity)
        {
            _dbSet.Update(entity);
        }

        public void Delete(DotGiamGiaSanPham entity)
        {
            _dbSet.Remove(entity);
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}