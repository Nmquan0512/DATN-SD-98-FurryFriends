using FurryFriends.API.Data;
using FurryFriends.API.Models;
using FurryFriends.API.Repository.IRepository;
using Microsoft.EntityFrameworkCore;

namespace FurryFriends.API.Repositories
{
    public class GiamGiaRepository : IGiamGiaRepository
    {
        private readonly AppDbContext _context;

        public GiamGiaRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<GiamGia>> GetAllAsync()
        {
            return await _context.GiamGias.ToListAsync();
        }

        public async Task<GiamGia?> GetByIdAsync(Guid id)
        {
            return await _context.GiamGias
                .Include(g => g.DotGiamGiaSanPhams)
                .FirstOrDefaultAsync(g => g.GiamGiaId == id);
        }

        public async Task AddAsync(GiamGia entity)
        {
            await _context.GiamGias.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(GiamGia entity)
        {
            _context.GiamGias.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var giamGia = await GetByIdAsync(id);
            if (giamGia != null)
            {
                _context.GiamGias.Remove(giamGia);
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
                g.TenGiamGia.ToLower().Trim() == tenGiamGia.ToLower().Trim() &&
                (!excludeId.HasValue || g.GiamGiaId != excludeId.Value));
        }

        public async Task<IEnumerable<GiamGia>> GetActiveDiscountsAsync()
        {
            var today = DateTime.Today;
            return await _context.GiamGias
                .Where(g => g.TrangThai && g.NgayBatDau <= today && g.NgayKetThuc >= today)
                .ToListAsync();
        }
        public async Task<IEnumerable<GiamGia>> GetAllWithSanPhamChiTietAsync()
        {
            return await _context.GiamGias
                .Include(g => g.DotGiamGiaSanPhams)
                .ToListAsync();
        }

        public async Task<GiamGia?> GetByIdWithSanPhamChiTietAsync(Guid id)
        {
            return await _context.GiamGias
                .Include(g => g.DotGiamGiaSanPhams)
                .FirstOrDefaultAsync(g => g.GiamGiaId == id);
        }

    }
}
