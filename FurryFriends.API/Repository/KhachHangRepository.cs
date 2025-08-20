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
            return await _context.KhachHangs
                                 .Include(k => k.DiaChiKhachHangs) // đúng tên
                                 .Where(k => k.TrangThai != 0) // Chỉ lấy những khách hàng chưa bị xóa
                                 .ToListAsync();
        }

        public async Task<KhachHang> GetByIdAsync(Guid id)
        {
            return await _context.KhachHangs
                                 .Include(k => k.DiaChiKhachHangs)
                                 .Where(k => k.TrangThai != 0) // Chỉ lấy những khách hàng chưa bị xóa
                                 .FirstOrDefaultAsync(k => k.KhachHangId == id);
        }

        public async Task AddAsync(KhachHang khachHang)
        {
            await _context.KhachHangs.AddAsync(khachHang);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(KhachHang khachHang)
        {
            _context.KhachHangs.Update(khachHang);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var khachHang = await _context.KhachHangs.FindAsync(id);
            if (khachHang != null)
            {
                // Soft delete: Thay đổi trạng thái thay vì xóa thực sự
                khachHang.TrangThai = 0; // 0 = Đã xóa/Inactive
                khachHang.NgayCapNhatCuoiCung = DateTime.Now;
                _context.KhachHangs.Update(khachHang);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<KhachHang?> FindByEmailAsync(string email)
        {
            return await _context.KhachHangs
                                 .Include(k => k.DiaChiKhachHangs)
                                 .Where(k => k.TrangThai != 0) // Chỉ lấy những khách hàng chưa bị xóa
                                 .FirstOrDefaultAsync(k => k.EmailCuaKhachHang == email);
        }

        public async Task<KhachHang?> FindByPhoneAsync(string phone)
        {
            return await _context.KhachHangs
                                 .Include(k => k.DiaChiKhachHangs)
                                 .Where(k => k.TrangThai != 0) // Chỉ lấy những khách hàng chưa bị xóa
                                 .FirstOrDefaultAsync(k => k.SDT == phone);
        }

        // Method để lấy tất cả khách hàng (bao gồm cả đã xóa) - dùng cho admin
        public async Task<IEnumerable<KhachHang>> GetAllIncludingDeletedAsync()
        {
            return await _context.KhachHangs
                                 .Include(k => k.DiaChiKhachHangs)
                                 .ToListAsync();
        }
    }
}

