using FurryFriends.API.Data;
using FurryFriends.API.Models;
using FurryFriends.API.Repository.IRepository;
using Microsoft.EntityFrameworkCore;

namespace FurryFriends.API.Repository
{
    public class NhanVIenRepository : INhanVienRepository
    {
        private readonly AppDbContext _context;

        public NhanVIenRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<NhanVien?> GetByIdAsync(Guid id)
        {
            return await _context.NhanViens
                .Include(nv => nv.ChucVu)
                .Include(nv => nv.TaiKhoan)
                .FirstOrDefaultAsync(nv => nv.TaiKhoanId == id);
        }
        public async Task<IEnumerable<NhanVien>> GetAllAsync()
        {
            return await _context.NhanViens
                .Include(nv => nv.ChucVu) // Bao gồm thông tin ChucVu
                .Include(nv => nv.TaiKhoan) // Bao gồm thông tin TaiKhoan
                .ToListAsync();
        }
        public async Task AddAsync(NhanVien nhanVien)
        {
			nhanVien.NgayTao = DateTime.Now;
			nhanVien.NgayCapNhat = DateTime.Now;
			if (nhanVien.ChucVuId == Guid.Empty)
			{
				throw new ArgumentException("ChucVuId is required.");
			}
			if (!await _context.ChucVus.AnyAsync(cv => cv.ChucVuId == nhanVien.ChucVuId))
			{
				throw new ArgumentException("ChucVuId does not exist.");
			}
			// TaiKhoanId is both PK and FK, ensure it matches an existing TaiKhoan
			var taiKhoan = await _context.TaiKhoans.FindAsync(nhanVien.TaiKhoanId);
			if (taiKhoan == null)
			{
				throw new ArgumentException("TaiKhoanId does not exist.");
			}
			_context.NhanViens.Add(nhanVien);
			await _context.SaveChangesAsync();
		}
        public async Task UpdateAsync(NhanVien nhanVien)
        {
            var existingNhanVien = await _context.NhanViens.FindAsync(nhanVien.TaiKhoanId);
            if (existingNhanVien == null)
            {
                throw new Exception("Nhan vien khong ton tai");
            }
            existingNhanVien.HoVaTen = nhanVien.HoVaTen;
            existingNhanVien.NgaySinh = nhanVien.NgaySinh;
            existingNhanVien.DiaChi = nhanVien.DiaChi;
            existingNhanVien.SDT = nhanVien.SDT;
            existingNhanVien.Email = nhanVien.Email;
            existingNhanVien.GioiTinh = nhanVien.GioiTinh;
            existingNhanVien.ChucVuId = nhanVien.ChucVuId;
            existingNhanVien.TrangThai = nhanVien.TrangThai;
            existingNhanVien.NgayCapNhat = DateTime.Now;
			if (!await _context.ChucVus.AnyAsync(cv => cv.ChucVuId == nhanVien.ChucVuId))
			{
				throw new ArgumentException("ChucVuId does not exist.");
			}
			await _context.SaveChangesAsync();
		}
        public async Task DeleteAsync(Guid id)
        {
            var nhanVien = await _context.NhanViens.FindAsync(id);
            if (nhanVien == null)
            {
                throw new Exception("Nhan vien khong ton tai");
            }
            _context.NhanViens.Remove(nhanVien);
            await _context.SaveChangesAsync();
        }
        public async Task<IEnumerable<NhanVien>> FindByNameAsync(string hoVaTen)
        {
            if (string.IsNullOrWhiteSpace(hoVaTen))
            {
				return await GetAllAsync();
			}

            return await _context.NhanViens
                .Include(nv => nv.ChucVu) // Bao gồm thông tin ChucVu
                .Include(nv => nv.TaiKhoan) // Bao gồm thông tin TaiKhoan
                .Where(nv => nv.HoVaTen.Contains(hoVaTen, StringComparison.OrdinalIgnoreCase))
                .ToListAsync();
        }

    }
}