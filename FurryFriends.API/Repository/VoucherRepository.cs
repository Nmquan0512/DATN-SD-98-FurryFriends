using FurryFriends.API.Data;
using FurryFriends.API.Models;
using FurryFriends.API.Repository.IRepository;
using Microsoft.EntityFrameworkCore;

namespace FurryFriends.API.Repository
{
    public class VoucherRepository : IVoucherRepository
    {
        private readonly AppDbContext _context;

        public VoucherRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Voucher>> GetAllAsync()
        {
            return await _context.Vouchers
                .Include(v => v.TaiKhoan)
                .ToListAsync();
        }

        public async Task<Voucher?> GetByIdAsync(Guid id)
        {
            return await _context.Vouchers
                .Include(v => v.TaiKhoan)
                .FirstOrDefaultAsync(v => v.VoucherId == id);
        }

        public async Task<Voucher> CreateAsync(Voucher voucher)
        {
            voucher.VoucherId = Guid.NewGuid();
            voucher.NgayTao = DateTime.UtcNow;

            _context.Vouchers.Add(voucher);
            await _context.SaveChangesAsync();

            return voucher;
        }

        public async Task<Voucher?> UpdateAsync(Guid id, Voucher voucher)
        {
            var existing = await _context.Vouchers.FindAsync(id);
            if (existing == null) return null;

            existing.TenVoucher = voucher.TenVoucher;
            existing.NgayBatDau = voucher.NgayBatDau;
            existing.NgayKetThuc = voucher.NgayKetThuc;
            existing.PhanTramGiam = voucher.PhanTramGiam;
            existing.TrangThai = voucher.TrangThai;
            existing.SoLuong = voucher.SoLuong;
            existing.TaiKhoanId = voucher.TaiKhoanId;
            existing.NgayCapNhat = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return existing;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var voucher = await _context.Vouchers.FindAsync(id);
            if (voucher == null) return false;

            _context.Vouchers.Remove(voucher);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
