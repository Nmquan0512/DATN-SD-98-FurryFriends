using FurryFriends.API.Data;
using FurryFriends.API.Models;
using FurryFriends.API.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

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
                .Include(sp => sp.SanPhamThanhPhans).ThenInclude(tp => tp.ThanhPhan)
                .Include(sp => sp.SanPhamChatLieus).ThenInclude(cl => cl.ChatLieu)
                .Include(sp => sp.ThuongHieu)
                .ToListAsync();
        }

        public async Task<SanPham?> GetByIdAsync(Guid id)
        {
            return await _context.SanPhams
                .Include(sp => sp.SanPhamThanhPhans).ThenInclude(tp => tp.ThanhPhan)
                .Include(sp => sp.SanPhamChatLieus).ThenInclude(cl => cl.ChatLieu)
                .Include(sp => sp.ThuongHieu)
                .FirstOrDefaultAsync(sp => sp.SanPhamId == id);
        }

        public async Task<IEnumerable<SanPham>> FindAsync(Expression<Func<SanPham, bool>> predicate)
        {
            return await _context.SanPhams
                .Where(predicate)
                .Include(sp => sp.SanPhamThanhPhans).ThenInclude(tp => tp.ThanhPhan)
                .Include(sp => sp.SanPhamChatLieus).ThenInclude(cl => cl.ChatLieu)
                .Include(sp => sp.ThuongHieu)
                .ToListAsync();
        }

        public async Task AddAsync(SanPham entity)
        {
            await _context.SanPhams.AddAsync(entity);
        }

        public void Update(SanPham entity)
        {
            _context.SanPhams.Update(entity);
        }

        public async Task UpdateAsync(SanPham entity)
        {
            _context.SanPhams.Update(entity);
            await _context.SaveChangesAsync();
        }

        public void Delete(SanPham entity)
        {
            _context.SanPhams.Remove(entity);
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
            return await _context.SanPhams.AnyAsync(sp => sp.SanPhamId == id);
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
