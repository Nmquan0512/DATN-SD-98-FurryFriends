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
<<<<<<< Updated upstream
            _context.GiamGias.Update(entity);
            await _context.SaveChangesAsync();
=======
            entity.NgayCapNhat = DateTime.UtcNow;
            // Đánh dấu đối tượng là đã bị thay đổi
            _context.Entry(entity).State = EntityState.Modified;
>>>>>>> Stashed changes
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
<<<<<<< Updated upstream
            var today = DateTime.Today;
            return await _context.GiamGias
                .Where(g => g.TrangThai && g.NgayBatDau <= today && g.NgayKetThuc >= today)
                .ToListAsync();
=======
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        // Phương thức xóa sản phẩm khỏi chương trình giảm giá
        public async Task RemoveProductsFromDiscount(Guid discountId, List<Guid> productIds)
        {
            var productsToRemove = await _context.DotGiamGiaSanPhams
                .Where(dggsp => dggsp.GiamGiaId == discountId && productIds.Contains(dggsp.SanPhamChiTietId))
                .ToListAsync();

            _context.DotGiamGiaSanPhams.RemoveRange(productsToRemove);
        }

        // Phương thức thêm sản phẩm vào chương trình giảm giá
        public async Task AddProductsToDiscount(Guid discountId, List<Guid> productIds, decimal discountPercentage)
        {
            var productsToAdd = productIds.Select(productId => new DotGiamGiaSanPham
            {
                GiamGiaId = discountId,
                SanPhamChiTietId = productId,
                PhanTramGiamGia = discountPercentage,
                TrangThai = true
            }).ToList();

            await _context.DotGiamGiaSanPhams.AddRangeAsync(productsToAdd);
>>>>>>> Stashed changes
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
