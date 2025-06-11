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
                .Include(tk => tk.SanPhams)
                .Include(tk => tk.Vouchers)
                .Include(tk => tk.KhachHang)
                .Include(tk => tk.HoaDons)
                .ToListAsync();
        }

        public async Task<TaiKhoan?> GetByIdAsync(Guid id)
        {
            return await _context.TaiKhoans
                .Include(tk => tk.NhanVien)
                .Include(tk => tk.SanPhams)
                .Include(tk => tk.Vouchers)
                .Include(tk => tk.KhachHang)
                .Include(tk => tk.HoaDons)
                .FirstOrDefaultAsync(tk => tk.TaiKhoanId == id);
        }

        public async Task AddAsync(TaiKhoan taiKhoan)
        {
            taiKhoan.NgayTaoTaiKhoan = DateTime.Now;
            taiKhoan.NgayCapNhatCuoiCung = DateTime.Now;
            if (taiKhoan.KhachHangId == Guid.Empty)
            {
                throw new ArgumentException("KhachHangId is required.");
            }
            _context.TaiKhoans.Add(taiKhoan);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(TaiKhoan taiKhoan)
        {
            var existing = await _context.TaiKhoans.FindAsync(taiKhoan.TaiKhoanId);
            if (existing == null)
            {
                throw new Exception("Tài khoản không tồn tại.");
            }
            existing.UserName = taiKhoan.UserName;
            existing.Password = taiKhoan.Password; // Ensure password is hashed in practice
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
                throw new Exception("Tài khoản không tồn tại.");
            }
            _context.TaiKhoans.Remove(taiKhoan);
            await _context.SaveChangesAsync();
        }

        public async Task<TaiKhoan?> FindByUserNameAsync(string userName)
        {
            if (string.IsNullOrWhiteSpace(userName))
            {
                return null;
            }
            return await _context.TaiKhoans
                .Include(tk => tk.NhanVien)
                .Include(tk => tk.SanPhams)
                .Include(tk => tk.Vouchers)
                .Include(tk => tk.KhachHang)
                .Include(tk => tk.HoaDons)
                .FirstOrDefaultAsync(tk => tk.UserName.Contains(userName, StringComparison.OrdinalIgnoreCase));
        }
    }
}