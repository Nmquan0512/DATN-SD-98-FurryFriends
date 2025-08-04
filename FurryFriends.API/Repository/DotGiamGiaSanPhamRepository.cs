using FurryFriends.API.Data;
using FurryFriends.API.Models;
using FurryFriends.API.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FurryFriends.API.Repositories
{
    public class DotGiamGiaSanPhamRepository : IDotGiamGiaSanPhamRepository
    {
        private readonly AppDbContext _context;

        public DotGiamGiaSanPhamRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<DotGiamGiaSanPham>> GetAllAsync()
        {
            return await _context.DotGiamGiaSanPhams
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<DotGiamGiaSanPham> GetByIdAsync(Guid id)
        {
            return await _context.DotGiamGiaSanPhams
                .FirstOrDefaultAsync(d => d.DotGiamGiaSanPhamId == id);
        }

        public async Task AddAsync(DotGiamGiaSanPham entity)
        {
            await _context.DotGiamGiaSanPhams.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task AddRangeAsync(IEnumerable<DotGiamGiaSanPham> entities)
        {
            await _context.DotGiamGiaSanPhams.AddRangeAsync(entities);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(DotGiamGiaSanPham entity)
        {
            entity.NgayCapNhat = DateTime.UtcNow;
            _context.DotGiamGiaSanPhams.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var entity = await _context.DotGiamGiaSanPhams.FindAsync(id);
            if (entity != null)
            {
                _context.DotGiamGiaSanPhams.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteByGiamGiaIdAsync(Guid giamGiaId)
        {
            var entities = await _context.DotGiamGiaSanPhams
                .Where(d => d.GiamGiaId == giamGiaId)
                .ToListAsync();

            if (entities.Any())
            {
                _context.DotGiamGiaSanPhams.RemoveRange(entities);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExistsAsync(Guid id)
        {
            return await _context.DotGiamGiaSanPhams
                .AnyAsync(d => d.DotGiamGiaSanPhamId == id);
        }

        public async Task<IEnumerable<DotGiamGiaSanPham>> GetByGiamGiaIdAsync(Guid giamGiaId)
        {
            return await _context.DotGiamGiaSanPhams
                .Where(d => d.GiamGiaId == giamGiaId)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IEnumerable<DotGiamGiaSanPham>> GetBySanPhamChiTietIdAsync(Guid sanPhamChiTietId)
        {
            return await _context.DotGiamGiaSanPhams
                .Where(d => d.SanPhamChiTietId == sanPhamChiTietId)
                .AsNoTracking()
                .ToListAsync();
        }
    }
}