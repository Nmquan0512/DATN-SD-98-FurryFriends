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
            return await _context.Vouchers.ToListAsync();
        }

        public async Task<Voucher?> GetByIdAsync(Guid id)
        {
            return await _context.Vouchers.FirstOrDefaultAsync(v => v.VoucherId == id);
        }

        public async Task AddAsync(Voucher voucher)
        {
            await _context.Vouchers.AddAsync(voucher);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Voucher voucher)
        {
            _context.Vouchers.Update(voucher);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var voucher = await _context.Vouchers.FindAsync(id);
            if (voucher != null)
            {
                _context.Vouchers.Remove(voucher);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> CheckVoucherValidAsync(Guid voucherId) //thêm cái này đề checkvoucher (sửa ở đây)
        {
            var voucher = await _context.Vouchers.FirstOrDefaultAsync(v => v.VoucherId == voucherId);
            if (voucher == null || voucher.TrangThai != 1 || voucher.SoLuong <= 0)
                return false;

            var now = DateTime.Now;
            return voucher.NgayBatDau <= now && voucher.NgayKetThuc >= now;
        }
    }
}