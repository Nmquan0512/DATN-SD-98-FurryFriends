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
    public class GiamGiaRepository : IGiamGiaRepository
    {
        private readonly AppDbContext _context;

        public GiamGiaRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<GiamGia>> GetAllAsync(bool includeProducts = false)
        {
            var query = _context.GiamGias.AsQueryable();

            if (includeProducts)
            {
                query = query.Include(g => g.DotGiamGiaSanPhams);
            }

            return await query.AsNoTracking().ToListAsync();
        }

        public async Task<GiamGia> GetByIdAsync(Guid id, bool includeProducts = false)
        {
            var query = _context.GiamGias.Where(g => g.GiamGiaId == id);

            if (includeProducts)
            {
                query = query.Include(g => g.DotGiamGiaSanPhams);
            }

            return await query.FirstOrDefaultAsync();
        }

        public async Task AddAsync(GiamGia entity)
        {
            await _context.GiamGias.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(GiamGia entity)
        {
            entity.NgayCapNhat = DateTime.UtcNow;
            _context.GiamGias.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var entity = await _context.GiamGias.FindAsync(id);
            if (entity != null)
            {
                _context.GiamGias.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExistsAsync(Guid id)
        {
            return await _context.GiamGias.AnyAsync(g => g.GiamGiaId == id);
        }

        public async Task<bool> TenGiamGiaExistsAsync(string tenGiamGia, Guid? excludeId = null)
        {
            return await _context.GiamGias.AnyAsync(g =>
                g.TenGiamGia.ToLower() == tenGiamGia.ToLower() &&
                (!excludeId.HasValue || g.GiamGiaId != excludeId.Value));
        }
    }
}