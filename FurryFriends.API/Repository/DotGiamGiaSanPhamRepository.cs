using FurryFriends.API.Data;
using FurryFriends.API.Models;
using FurryFriends.API.Repository.IRepository;
using Microsoft.EntityFrameworkCore;

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
                .Include(d => d.GiamGia)
                .Include(d => d.SanPhamChiTiet)
                .ToListAsync();
        }

        public async Task<DotGiamGiaSanPham?> GetByIdAsync(Guid id)
        {
            return await _context.DotGiamGiaSanPhams
                .Include(d => d.GiamGia)
                .Include(d => d.SanPhamChiTiet)
                .FirstOrDefaultAsync(d => d.DotGiamGiaSanPhamId == id);
        }

        public async Task AddAsync(DotGiamGiaSanPham entity)
        {
            await _context.DotGiamGiaSanPhams.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(DotGiamGiaSanPham entity)
        {
            _context.DotGiamGiaSanPhams.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var dot = await GetByIdAsync(id);
            if (dot != null)
            {
                _context.DotGiamGiaSanPhams.Remove(dot);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExistsAsync(Guid id)
        {
            return await _context.DotGiamGiaSanPhams.AnyAsync(d => d.DotGiamGiaSanPhamId == id);
        }

        public async Task<IEnumerable<DotGiamGiaSanPham>> GetBySanPhamIdAsync(Guid sanPhamId)
        {
            return await _context.DotGiamGiaSanPhams
                .Where(d => d.SanPhamChiTiet != null && d.SanPhamChiTiet.SanPhamId == sanPhamId)
                .Include(d => d.GiamGia)
                .Include(d => d.SanPhamChiTiet)
                .ToListAsync();
        }
        public async Task DeleteByGiamGiaIdAsync(Guid giamGiaId)
        {
            var list = await _context.DotGiamGiaSanPhams
                .Where(d => d.GiamGiaId == giamGiaId)
                .ToListAsync();

            _context.DotGiamGiaSanPhams.RemoveRange(list);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<DotGiamGiaSanPham>> GetByGiamGiaIdAsync(Guid giamGiaId)
        {
            return await _context.DotGiamGiaSanPhams
                .Where(d => d.GiamGiaId == giamGiaId)
                .Include(d => d.GiamGia)
                .Include(d => d.SanPhamChiTiet)
                .ToListAsync();
        }

        public async Task AddRangeAsync(IEnumerable<DotGiamGiaSanPham> entities)
        {
            await _context.DotGiamGiaSanPhams.AddRangeAsync(entities);
            await _context.SaveChangesAsync();
        }
    }
}
