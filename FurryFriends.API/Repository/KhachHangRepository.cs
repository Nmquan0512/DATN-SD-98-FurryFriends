using FurryFriends.API.Data;
using FurryFriends.API.Models;
using FurryFriends.API.Repository.IRepository;
using Microsoft.EntityFrameworkCore;

namespace FurryFriends.API.Repository
{

    public class KhachHangRepository : IKhachHangRepository
    {
        private readonly AppDbContext _context;

        public KhachHangRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<KhachHang>> GetAllAsync()
        {
            return await _context.KhachHangs.ToListAsync();
        }

        public async Task<KhachHang?> GetByIdAsync(Guid id)
        {
            return await _context.KhachHangs.FindAsync(id);
        }

        public async Task<KhachHang> AddAsync(KhachHang khachHang)
        {
            khachHang.KhachHangId = Guid.NewGuid();
            khachHang.NgayTaoTaiKhoan = DateTime.Now;
            await _context.KhachHangs.AddAsync(khachHang);
            await _context.SaveChangesAsync();
            return khachHang;
        }

        public async Task<KhachHang?> UpdateAsync(Guid id, KhachHang khachHang)
        {
            var existing = await _context.KhachHangs.FindAsync(id);
            if (existing == null) return null;

            existing.TenKhachHang = khachHang.TenKhachHang;
            existing.SDT = khachHang.SDT;
            existing.EmailCuaKhachHang = khachHang.EmailCuaKhachHang;
            existing.DiemKhachHang = khachHang.DiemKhachHang;
            existing.TrangThai = khachHang.TrangThai;
            existing.NgayCapNhatCuoiCung = DateTime.Now;
            existing.TaiKhoanId = khachHang.TaiKhoanId;

            await _context.SaveChangesAsync();
            return existing;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var existing = await _context.KhachHangs.FindAsync(id);
            if (existing == null) return false;

            _context.KhachHangs.Remove(existing);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}

