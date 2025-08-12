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

        // ... Các hàm Get không thay đổi ...
        public async Task<IEnumerable<DotGiamGiaSanPham>> GetAllAsync()
        {
            return await _context.DotGiamGiaSanPhams.AsNoTracking().ToListAsync();
        }

        public async Task<DotGiamGiaSanPham> GetByIdAsync(Guid id)
        {
            return await _context.DotGiamGiaSanPhams.FirstOrDefaultAsync(d => d.DotGiamGiaSanPhamId == id);
        }

        public async Task<IEnumerable<DotGiamGiaSanPham>> GetByGiamGiaIdAsync(Guid giamGiaId)
        {
            return await _context.DotGiamGiaSanPhams
                .Where(d => d.GiamGiaId == giamGiaId)
                .ToListAsync(); // Dùng tracking để có thể xóa
        }

        public async Task<bool> ExistsAsync(Guid id)
        {
            return await _context.DotGiamGiaSanPhams.AnyAsync(d => d.DotGiamGiaSanPhamId == id);
        }

        // Các hàm thay đổi
        public async Task AddRangeAsync(IEnumerable<DotGiamGiaSanPham> entities)
        {
            await _context.DotGiamGiaSanPhams.AddRangeAsync(entities);
        }

        public void DeleteRange(IEnumerable<DotGiamGiaSanPham> entities)
        {
            _context.DotGiamGiaSanPhams.RemoveRange(entities);
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}