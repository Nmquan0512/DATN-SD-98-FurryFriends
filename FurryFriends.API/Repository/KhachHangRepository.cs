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
            // Kiểm tra email không được trùng với NhanVien
            if (!string.IsNullOrWhiteSpace(khachHang.EmailCuaKhachHang))
            {
                var normalizedEmail = khachHang.EmailCuaKhachHang.ToLower().Trim();
                var existingNhanVien = await _context.NhanViens
                    .FirstOrDefaultAsync(nv => nv.Email != null && 
                                              nv.Email.ToLower().Trim() == normalizedEmail);
                if (existingNhanVien != null)
                {
                    throw new ArgumentException($"Email '{khachHang.EmailCuaKhachHang}' đã được sử dụng bởi nhân viên '{existingNhanVien.HoVaTen}'.");
                }

                // Kiểm tra email không được trùng với KhachHang khác
                var existingKhachHang = await _context.KhachHangs
                    .FirstOrDefaultAsync(kh => kh.EmailCuaKhachHang != null && 
                                              kh.EmailCuaKhachHang.ToLower().Trim() == normalizedEmail &&
                                              kh.KhachHangId != khachHang.KhachHangId);
                if (existingKhachHang != null)
                {
                    throw new ArgumentException($"Email '{khachHang.EmailCuaKhachHang}' đã được sử dụng bởi khách hàng '{existingKhachHang.TenKhachHang}'.");
                }
            }

            await _context.KhachHangs.AddAsync(khachHang);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(KhachHang khachHang)
        {
            // Kiểm tra email không được trùng với NhanVien
            if (!string.IsNullOrWhiteSpace(khachHang.EmailCuaKhachHang))
            {
                var normalizedEmail = khachHang.EmailCuaKhachHang.ToLower().Trim();
                var existingNhanVien = await _context.NhanViens
                    .FirstOrDefaultAsync(nv => nv.Email != null && 
                                              nv.Email.ToLower().Trim() == normalizedEmail);
                if (existingNhanVien != null)
                {
                    throw new ArgumentException($"Email '{khachHang.EmailCuaKhachHang}' đã được sử dụng bởi nhân viên '{existingNhanVien.HoVaTen}'.");
                }

                // Kiểm tra email không được trùng với KhachHang khác (trừ chính nó)
                var existingKhachHang = await _context.KhachHangs
                    .FirstOrDefaultAsync(kh => kh.EmailCuaKhachHang != null && 
                                              kh.EmailCuaKhachHang.ToLower().Trim() == normalizedEmail &&
                                              kh.KhachHangId != khachHang.KhachHangId);
                if (existingKhachHang != null)
                {
                    throw new ArgumentException($"Email '{khachHang.EmailCuaKhachHang}' đã được sử dụng bởi khách hàng '{existingKhachHang.TenKhachHang}'.");
                }
            }

            _context.KhachHangs.Update(khachHang);
            await _context.SaveChangesAsync();
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

