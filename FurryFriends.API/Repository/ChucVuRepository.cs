using FurryFriends.API.Data;
using FurryFriends.API.Models;
using FurryFriends.API.Repository.IRepository;
using Microsoft.EntityFrameworkCore;

namespace FurryFriends.API.Repository
{
    public class ChucVuRepository : IChucVuRepository
    {
        private readonly AppDbContext _context;

        public ChucVuRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ChucVu>> GetAllAsync()
        {
            return await _context.ChucVus
                .Include(cv => cv.NhanViens)
                .ToListAsync();
        }

        public async Task<ChucVu?> GetByIdAsync(Guid id)
        {
            return await _context.ChucVus
                .Include(cv => cv.NhanViens)
                .FirstOrDefaultAsync(cv => cv.ChucVuId == id);
        }

        public async Task AddAsync(ChucVu chucVu)
        {
            if (string.IsNullOrWhiteSpace(chucVu.TenChucVu))
            {
                throw new ArgumentException("Tên chức vụ không được để trống.");
            }
            if (string.IsNullOrWhiteSpace(chucVu.MoTaChucVu))
            {
                throw new ArgumentException("Mô tả chức vụ không được để trống.");
            }

            chucVu.ChucVuId = Guid.NewGuid();
            chucVu.NgayTao = DateTime.Now;
            chucVu.NgayCapNhat = DateTime.Now;

            _context.ChucVus.Add(chucVu);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(ChucVu chucVu)
        {
            var existing = await _context.ChucVus.FindAsync(chucVu.ChucVuId);
            if (existing == null)
            {
                throw new KeyNotFoundException("Chức vụ không tồn tại.");
            }

            if (string.IsNullOrWhiteSpace(chucVu.TenChucVu))
            {
                throw new ArgumentException("Tên chức vụ không được để trống.");
            }
            if (string.IsNullOrWhiteSpace(chucVu.MoTaChucVu))
            {
                throw new ArgumentException("Mô tả chức vụ không được để trống.");
            }

            existing.TenChucVu = chucVu.TenChucVu;
            existing.MoTaChucVu = chucVu.MoTaChucVu;
            existing.TrangThai = chucVu.TrangThai;
            existing.NgayCapNhat = DateTime.Now;

            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var chucVu = await _context.ChucVus.FindAsync(id);
            if (chucVu == null)
            {
                throw new KeyNotFoundException("Chức vụ không tồn tại.");
            }

            if (await _context.NhanViens.AnyAsync(nv => nv.ChucVuId == id))
            {
                throw new InvalidOperationException("Không thể xóa chức vụ vì nó đang được sử dụng bởi nhân viên.");
            }

            _context.ChucVus.Remove(chucVu);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<ChucVu>> FindByTenChucVuAsync(string tenChucVu)
        {
            if (string.IsNullOrWhiteSpace(tenChucVu))
                return await GetAllAsync();

            // Cách 1: Dùng EF.Functions.Like – chuẩn SQL, tối ưu
            return await _context.ChucVus
                .Include(cv => cv.NhanViens)
                .Where(cv => EF.Functions.Like(cv.TenChucVu, $"%{tenChucVu}%"))
                .ToListAsync();
        }
    }
}