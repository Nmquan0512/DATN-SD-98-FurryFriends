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
                // Include cả SanPhamChiTiet để có thể lấy tên sản phẩm nếu cần
                query = query.Include(g => g.DotGiamGiaSanPhams)
                             .ThenInclude(dggsp => dggsp.SanPhamChiTiet);
            }

            return await query.AsNoTracking().ToListAsync();
        }

        public async Task<GiamGia> GetByIdAsync(Guid id, bool includeProducts = false)
        {
            var query = _context.GiamGias.AsQueryable();

            if (includeProducts)
            {
                // Khi lấy chi tiết, nên dùng tracking để có thể cập nhật
                query = query.Include(g => g.DotGiamGiaSanPhams);
            }
            else
            {
                query = query.AsNoTracking();
            }

            return await query.FirstOrDefaultAsync(g => g.GiamGiaId == id);
        }

        public async Task AddAsync(GiamGia entity)
        {
            await _context.GiamGias.AddAsync(entity);
            // Không SaveChanges ở đây
        }

        public void Update(GiamGia entity)
        {
            entity.NgayCapNhat = DateTime.UtcNow;
            // Đánh dấu đối tượng là đã bị thay đổi, không cần Save
            _context.Entry(entity).State = EntityState.Modified;
        }

        public void Delete(GiamGia entity)
        {
            if (entity != null)
            {
                _context.GiamGias.Remove(entity);
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

        // Phương thức Commit duy nhất
        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}