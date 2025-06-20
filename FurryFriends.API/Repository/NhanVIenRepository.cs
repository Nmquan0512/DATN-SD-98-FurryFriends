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
                .FirstOrDefaultAsync(nv => nv.NhanVienId == id);
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
            nhanVien.NhanVienId = Guid.NewGuid();
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

            if (nhanVien.TaiKhoanId != Guid.Empty)
            {
                var taiKhoan = await _context.TaiKhoans.FindAsync(nhanVien.TaiKhoanId);
                if (taiKhoan == null)
                {
                    throw new ArgumentException("TaiKhoanId does not exist.");
                }
                if (!taiKhoan.TrangThai)
                {
                    throw new ArgumentException("Tài khoản không ở trạng thái hoạt động.");
                }
                if (await _context.NhanViens.AnyAsync(nv => nv.TaiKhoanId == nhanVien.TaiKhoanId))
                {
                    throw new ArgumentException("TaiKhoanId is already associated with another NhanVien.");
                }
            }

            _context.NhanViens.Add(nhanVien);
            await _context.SaveChangesAsync();
        }
        public async Task UpdateAsync(NhanVien nhanVien)
        {
            var existing = await _context.NhanViens.FindAsync(nhanVien.NhanVienId);
            if (existing == null)
            {
                throw new KeyNotFoundException("Nhân viên không tồn tại.");
            }

            existing.HoVaTen = nhanVien.HoVaTen;
            existing.NgaySinh = nhanVien.NgaySinh;
            existing.DiaChi = nhanVien.DiaChi;
            existing.SDT = nhanVien.SDT;
            existing.Email = nhanVien.Email;
            existing.GioiTinh = nhanVien.GioiTinh;
            existing.ChucVuId = nhanVien.ChucVuId;
            existing.TaiKhoanId = nhanVien.TaiKhoanId;
            existing.TrangThai = nhanVien.TrangThai;
            existing.NgayCapNhat = DateTime.Now;

            if (!await _context.ChucVus.AnyAsync(cv => cv.ChucVuId == nhanVien.ChucVuId))
            {
                throw new ArgumentException("ChucVuId does not exist.");
            }

            if (nhanVien.TaiKhoanId != Guid.Empty)
            {
                var taiKhoan = await _context.TaiKhoans.FindAsync(nhanVien.TaiKhoanId);
                if (taiKhoan == null)
                {
                    throw new ArgumentException("TaiKhoanId does not exist.");
                }
                if (!taiKhoan.TrangThai)
                {
                    throw new ArgumentException("Tài khoản không ở trạng thái hoạt động.");
                }
                if (await _context.NhanViens.AnyAsync(nv => nv.TaiKhoanId == nhanVien.TaiKhoanId && nv.NhanVienId != nhanVien.NhanVienId))
                {
                    throw new ArgumentException("TaiKhoanId is already associated with another NhanVien.");
                }
            }

            await _context.SaveChangesAsync();
        }
        public async Task DeleteAsync(Guid id)
        {
            var nhanVien = await _context.NhanViens.FindAsync(id);
            if (nhanVien == null)
            {
                throw new KeyNotFoundException("Nhân viên không tồn tại.");
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
                .Where(nv => EF.Functions.Like(nv.HoVaTen, $"%{hoVaTen}%"))
                .ToListAsync();
        }
        public async Task<bool> CheckTaiKhoanExistsAsync(Guid taiKhoanId)
        {
            return await _context.TaiKhoans.AnyAsync(tk => tk.TaiKhoanId == taiKhoanId);
        }

        public async Task<bool> CheckChucVuExistsAsync(Guid chucVuId)
        {
            return await _context.ChucVus.AnyAsync(cv => cv.ChucVuId == chucVuId);
        }

        public async Task<bool> CheckTaiKhoanLinkedAsync(Guid taiKhoanId, Guid? nhanVienId = null)
        {
            return await _context.NhanViens.AnyAsync(nv => nv.TaiKhoanId == taiKhoanId && (nhanVienId == null || nv.NhanVienId != nhanVienId));
        }
    }
}