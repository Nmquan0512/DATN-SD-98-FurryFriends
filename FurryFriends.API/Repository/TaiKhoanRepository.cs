using FurryFriends.API.Data;
using FurryFriends.API.Models;
using FurryFriends.API.Repository.IRepository;
using Microsoft.EntityFrameworkCore;

namespace FurryFriends.API.Repository
{
    public class TaiKhoanRepository : ITaiKhoanRepository
    {
        private readonly AppDbContext _context;

        public TaiKhoanRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<TaiKhoan>> GetAllAsync()
        {
            return await _context.TaiKhoans
                .Include(tk => tk.NhanVien)
                .Include(tk => tk.KhachHang)
                .ToListAsync();
        }

        public async Task<TaiKhoan?> GetByIdAsync(Guid id)
        {
            return await _context.TaiKhoans
                .Include(tk => tk.NhanVien)
                .Include(tk => tk.KhachHang)
                .FirstOrDefaultAsync(tk => tk.TaiKhoanId == id);
        }

        public async Task AddAsync(TaiKhoan taiKhoan)
        {
            if (string.IsNullOrWhiteSpace(taiKhoan.UserName))
            {
                throw new ArgumentException("UserName không được để trống.");
            }
            if (string.IsNullOrWhiteSpace(taiKhoan.Password))
            {
                throw new ArgumentException("Password không được để trống.");
            }
            if (await _context.TaiKhoans.AnyAsync(tk => tk.UserName == taiKhoan.UserName))
            {
                throw new ArgumentException("UserName đã tồn tại.");
            }
            if (taiKhoan.KhachHangId.HasValue && !await _context.KhachHangs.AnyAsync(kh => kh.KhachHangId == taiKhoan.KhachHangId))
            {
                throw new ArgumentException("KhachHangId does not exist.");
            }

            taiKhoan.TaiKhoanId = Guid.NewGuid();
            taiKhoan.NgayTaoTaiKhoan = DateTime.Now;
            taiKhoan.NgayCapNhatCuoiCung = DateTime.Now;
            // TODO: Mã hóa Password, ví dụ: taiKhoan.Password = BCrypt.Net.BCrypt.HashPassword(taiKhoan.Password);

            _context.TaiKhoans.Add(taiKhoan);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(TaiKhoan taiKhoan)
        {
            var existing = await _context.TaiKhoans.FindAsync(taiKhoan.TaiKhoanId);
            if (existing == null)
            {
                throw new KeyNotFoundException("Tài khoản không tồn tại.");
            }
            if (string.IsNullOrWhiteSpace(taiKhoan.UserName))
            {
                throw new ArgumentException("UserName không được để trống.");
            }
            if (string.IsNullOrWhiteSpace(taiKhoan.Password))
            {
                throw new ArgumentException("Password không được để trống.");
            }
            if (await _context.TaiKhoans.AnyAsync(tk => tk.UserName == taiKhoan.UserName && tk.TaiKhoanId != taiKhoan.TaiKhoanId))
            {
                throw new ArgumentException("UserName đã tồn tại.");
            }
            if (taiKhoan.KhachHangId.HasValue && !await _context.KhachHangs.AnyAsync(kh => kh.KhachHangId == taiKhoan.KhachHangId))
            {
                throw new ArgumentException("KhachHangId does not exist.");
            }

            existing.UserName = taiKhoan.UserName;
            // TODO: Mã hóa Password
            existing.Password = taiKhoan.Password;
            existing.TrangThai = taiKhoan.TrangThai;
            existing.KhachHangId = taiKhoan.KhachHangId;
            existing.NgayCapNhatCuoiCung = DateTime.Now;

            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var taiKhoan = await _context.TaiKhoans.FindAsync(id);
            if (taiKhoan == null)
            {
                throw new KeyNotFoundException("Tài khoản không tồn tại.");
            }
            if (taiKhoan.NhanVien != null)
            {
                throw new InvalidOperationException("Không thể xóa tài khoản vì nó đang liên kết với nhân viên.");
            }

            _context.TaiKhoans.Remove(taiKhoan);
            await _context.SaveChangesAsync();
        }

        public async Task<TaiKhoan?> FindByUserNameAsync(string userName)
        {
            if (string.IsNullOrWhiteSpace(userName))
                return null;

            return await _context.TaiKhoans
                .Include(tk => tk.NhanVien)
                    .ThenInclude(nv => nv.ChucVu)
                .Include(tk => tk.KhachHang)
                .FirstOrDefaultAsync(tk => tk.UserName == userName); // Khớp chính xác
        }
    }
}