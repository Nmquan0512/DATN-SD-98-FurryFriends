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
    public class SanPhamChiTietRepository : ISanPhamChiTietRepository
    {
        private readonly AppDbContext _context;
        private readonly DbSet<SanPhamChiTiet> _dbSet;

        public SanPhamChiTietRepository(AppDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<SanPhamChiTiet>();
        }

        // Hàm dùng chung để include toàn bộ các navigation
        private IQueryable<SanPhamChiTiet> IncludeAll()
        {
            return _dbSet
                .Include(s => s.SanPham)
                .Include(s => s.KichCo)
                .Include(s => s.MauSac)
                .Include(s => s.Anh);
        }

        public async Task<IEnumerable<SanPhamChiTiet>> GetAllAsync()
        {
            return await IncludeAll().ToListAsync();
        }

        public async Task<SanPhamChiTiet?> GetByIdAsync(Guid id)
        {
            return await IncludeAll().FirstOrDefaultAsync(x => x.SanPhamChiTietId == id);
        }

        public async Task<IEnumerable<SanPhamChiTiet>> FindAsync(Expression<Func<SanPhamChiTiet, bool>> predicate)
        {
            return await IncludeAll().Where(predicate).ToListAsync();
        }

        public async Task<SanPhamChiTiet?> FindOneAsync(Expression<Func<SanPhamChiTiet, bool>> predicate)
        {
            return await IncludeAll().FirstOrDefaultAsync(predicate);
        }

        public async Task<bool> ExistsAsync(Guid id)
        {
            return await _dbSet.AnyAsync(x => x.SanPhamChiTietId == id);
        }

        public async Task AddAsync(SanPhamChiTiet entity)
        {
            await _dbSet.AddAsync(entity);
        }

        public void Update(SanPhamChiTiet entity)
        {
            _dbSet.Update(entity);
        }

        public void Delete(SanPhamChiTiet entity)
        {
            _dbSet.Remove(entity);
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
