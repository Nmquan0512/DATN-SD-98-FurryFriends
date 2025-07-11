using FurryFriends.API.Data;
using FurryFriends.API.Models;
using FurryFriends.API.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FurryFriends.API.Repository
{
    public class SanPhamRepository : ISanPhamRepository
    {
        private readonly AppDbContext _context;

        public SanPhamRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<SanPham>> GetAllAsync()
        {
            return await _context.SanPhams
                .Include(x => x.SanPhamThanhPhans)
                .Include(x => x.SanPhamChatLieus)
                .Include(x => x.ThuongHieu)
                .ToListAsync();
        }

        public async Task<SanPham> GetByIdAsync(Guid id)
        {
            return await _context.SanPhams
                .Include(x => x.SanPhamThanhPhans)
                .Include(x => x.SanPhamChatLieus)
                .Include(x => x.ThuongHieu)
                .FirstOrDefaultAsync(x => x.SanPhamId == id);
        }

        public async Task AddAsync(SanPham entity)
        {
            await _context.SanPhams.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(SanPham entity)
        {
            _context.SanPhams.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var entity = await _context.SanPhams.FindAsync(id);
            if (entity != null)
            {
                _context.SanPhams.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExistsAsync(Guid id)
        {
            return await _context.SanPhams.AnyAsync(x => x.SanPhamId == id);
        }
    }
}
