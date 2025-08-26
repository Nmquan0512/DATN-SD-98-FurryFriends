﻿using FurryFriends.API.Data;
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
            if (taiKhoan.NhanVienId.HasValue && !await _context.NhanViens.AnyAsync(nv => nv.NhanVienId == taiKhoan.NhanVienId))
            {
                throw new ArgumentException("NhanVienId does not exist.");
            }

            // Kiểm tra email không được trùng lặp
            if (taiKhoan.KhachHangId.HasValue)
            {
                var khachHang = await _context.KhachHangs.FindAsync(taiKhoan.KhachHangId.Value);
                if (khachHang != null && !string.IsNullOrWhiteSpace(khachHang.EmailCuaKhachHang))
                {
                    var normalizedEmail = khachHang.EmailCuaKhachHang.ToLower().Trim();
                    
                    // Kiểm tra với NhanVien
                    var existingNhanVien = await _context.NhanViens
                        .FirstOrDefaultAsync(nv => nv.Email != null && 
                                                  nv.Email.ToLower().Trim() == normalizedEmail);
                    if (existingNhanVien != null)
                    {
                        throw new ArgumentException($"Email '{khachHang.EmailCuaKhachHang}' đã được sử dụng bởi nhân viên '{existingNhanVien.HoVaTen}'.");
                    }

                    // Kiểm tra với KhachHang khác
                    var existingKhachHang = await _context.KhachHangs
                        .FirstOrDefaultAsync(kh => kh.EmailCuaKhachHang != null && 
                                                  kh.EmailCuaKhachHang.ToLower().Trim() == normalizedEmail &&
                                                  kh.KhachHangId != taiKhoan.KhachHangId);
                    if (existingKhachHang != null)
                    {
                        throw new ArgumentException($"Email '{khachHang.EmailCuaKhachHang}' đã được sử dụng bởi khách hàng '{existingKhachHang.TenKhachHang}'.");
                    }
                }
            }

            if (taiKhoan.NhanVienId.HasValue)
            {
                var nhanVien = await _context.NhanViens.FindAsync(taiKhoan.NhanVienId.Value);
                if (nhanVien != null && !string.IsNullOrWhiteSpace(nhanVien.Email))
                {
                    var normalizedEmail = nhanVien.Email.ToLower().Trim();
                    
                    // Kiểm tra với KhachHang
                    var existingKhachHang = await _context.KhachHangs
                        .FirstOrDefaultAsync(kh => kh.EmailCuaKhachHang != null && 
                                                  kh.EmailCuaKhachHang.ToLower().Trim() == normalizedEmail);
                    if (existingKhachHang != null)
                    {
                        throw new ArgumentException($"Email '{nhanVien.Email}' đã được sử dụng bởi khách hàng '{existingKhachHang.TenKhachHang}'.");
                    }

                    // Kiểm tra với NhanVien khác
                    var existingNhanVien = await _context.NhanViens
                        .FirstOrDefaultAsync(nv => nv.Email != null && 
                                                  nv.Email.ToLower().Trim() == normalizedEmail &&
                                                  nv.NhanVienId != taiKhoan.NhanVienId);
                    if (existingNhanVien != null)
                    {
                        throw new ArgumentException($"Email '{nhanVien.Email}' đã được sử dụng bởi nhân viên '{existingNhanVien.HoVaTen}'.");
                    }
                }
            }

            taiKhoan.TaiKhoanId = Guid.NewGuid();
            taiKhoan.NgayTaoTaiKhoan = DateTime.Now;
            taiKhoan.NgayCapNhatCuoiCung = DateTime.Now;
            // Lưu mật khẩu trực tiếp không mã hóa

            _context.TaiKhoans.Add(taiKhoan);
            await _context.SaveChangesAsync();

            // 🔄 Cập nhật liên kết ngược: Cập nhật KhachHang.TaiKhoanId
            if (taiKhoan.KhachHangId.HasValue)
            {
                var khachHang = await _context.KhachHangs.FindAsync(taiKhoan.KhachHangId.Value);
                if (khachHang != null)
                {
                    khachHang.TaiKhoanId = taiKhoan.TaiKhoanId;
                    khachHang.NgayCapNhatCuoiCung = DateTime.Now;
                    await _context.SaveChangesAsync();
                }
            }
        }

        public async Task UpdateAsync(TaiKhoan taiKhoan)
        {
            var existing = await _context.TaiKhoans
                .Include(tk => tk.NhanVien)
                .Include(tk => tk.KhachHang)
                .FirstOrDefaultAsync(tk => tk.TaiKhoanId == taiKhoan.TaiKhoanId);
                
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
            if (taiKhoan.NhanVienId.HasValue && !await _context.NhanViens.AnyAsync(nv => nv.NhanVienId == taiKhoan.NhanVienId))
            {
                throw new ArgumentException("NhanVienId does not exist.");
            }

            // Kiểm tra email không được trùng lặp khi thay đổi KhachHangId
            if (taiKhoan.KhachHangId != existing.KhachHangId && taiKhoan.KhachHangId.HasValue)
            {
                var khachHang = await _context.KhachHangs.FindAsync(taiKhoan.KhachHangId.Value);
                if (khachHang != null && !string.IsNullOrWhiteSpace(khachHang.EmailCuaKhachHang))
                {
                    var normalizedEmail = khachHang.EmailCuaKhachHang.ToLower().Trim();
                    
                    // Kiểm tra với NhanVien
                    var existingNhanVien = await _context.NhanViens
                        .FirstOrDefaultAsync(nv => nv.Email != null && 
                                                  nv.Email.ToLower().Trim() == normalizedEmail);
                    if (existingNhanVien != null)
                    {
                        throw new ArgumentException($"Email '{khachHang.EmailCuaKhachHang}' đã được sử dụng bởi nhân viên '{existingNhanVien.HoVaTen}'.");
                    }

                    // Kiểm tra với KhachHang khác
                    var existingKhachHang = await _context.KhachHangs
                        .FirstOrDefaultAsync(kh => kh.EmailCuaKhachHang != null && 
                                                  kh.EmailCuaKhachHang.ToLower().Trim() == normalizedEmail &&
                                                  kh.KhachHangId != taiKhoan.KhachHangId);
                    if (existingKhachHang != null)
                    {
                        throw new ArgumentException($"Email '{khachHang.EmailCuaKhachHang}' đã được sử dụng bởi khách hàng '{existingKhachHang.TenKhachHang}'.");
                    }
                }
            }

            // Kiểm tra email không được trùng lặp khi thay đổi NhanVienId
            if (taiKhoan.NhanVienId != existing.NhanVienId && taiKhoan.NhanVienId.HasValue)
            {
                var nhanVien = await _context.NhanViens.FindAsync(taiKhoan.NhanVienId.Value);
                if (nhanVien != null && !string.IsNullOrWhiteSpace(nhanVien.Email))
                {
                    var normalizedEmail = nhanVien.Email.ToLower().Trim();
                    
                    // Kiểm tra với KhachHang
                    var existingKhachHang = await _context.KhachHangs
                        .FirstOrDefaultAsync(kh => kh.EmailCuaKhachHang != null && 
                                                  kh.EmailCuaKhachHang.ToLower().Trim() == normalizedEmail);
                    if (existingKhachHang != null)
                    {
                        throw new ArgumentException($"Email '{nhanVien.Email}' đã được sử dụng bởi khách hàng '{existingKhachHang.TenKhachHang}'.");
                    }

                    // Kiểm tra với NhanVien khác
                    var existingNhanVien = await _context.NhanViens
                        .FirstOrDefaultAsync(nv => nv.Email != null && 
                                                  nv.Email.ToLower().Trim() == normalizedEmail &&
                                                  nv.NhanVienId != taiKhoan.NhanVienId);
                    if (existingNhanVien != null)
                    {
                        throw new ArgumentException($"Email '{nhanVien.Email}' đã được sử dụng bởi nhân viên '{existingNhanVien.HoVaTen}'.");
                    }
                }
            }

            // Lưu KhachHangId cũ để xử lý liên kết
            var oldKhachHangId = existing.KhachHangId;

            existing.UserName = taiKhoan.UserName;
            // Lưu mật khẩu trực tiếp không mã hóa
            existing.Password = taiKhoan.Password;
            existing.TrangThai = taiKhoan.TrangThai;
            existing.KhachHangId = taiKhoan.KhachHangId;
            existing.NhanVienId = taiKhoan.NhanVienId;
            existing.NgayCapNhatCuoiCung = DateTime.Now;

            await _context.SaveChangesAsync();

            // 🔄 Cập nhật liên kết ngược: Xử lý thay đổi KhachHangId
            if (oldKhachHangId != taiKhoan.KhachHangId)
            {
                // Xóa liên kết cũ
                if (oldKhachHangId.HasValue)
                {
                    var oldKhachHang = await _context.KhachHangs.FindAsync(oldKhachHangId.Value);
                    if (oldKhachHang != null)
                    {
                        oldKhachHang.TaiKhoanId = null;
                        oldKhachHang.NgayCapNhatCuoiCung = DateTime.Now;
                    }
                }

                // Tạo liên kết mới
                if (taiKhoan.KhachHangId.HasValue)
                {
                    var newKhachHang = await _context.KhachHangs.FindAsync(taiKhoan.KhachHangId.Value);
                    if (newKhachHang != null)
                    {
                        newKhachHang.TaiKhoanId = taiKhoan.TaiKhoanId;
                        newKhachHang.NgayCapNhatCuoiCung = DateTime.Now;
                    }
                }

                await _context.SaveChangesAsync();
            }
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

            // 🔄 Xóa liên kết với khách hàng trước khi xóa tài khoản
            if (taiKhoan.KhachHangId.HasValue)
            {
                var khachHang = await _context.KhachHangs.FindAsync(taiKhoan.KhachHangId.Value);
                if (khachHang != null)
                {
                    khachHang.TaiKhoanId = null;
                    khachHang.NgayCapNhatCuoiCung = DateTime.Now;
                }
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

        public async Task<TaiKhoan?> FindByEmailAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return null;

            var normalizedEmail = email.ToLower().Trim();
            Console.WriteLine($"=== DEBUG FindByEmailAsync ===");
            Console.WriteLine($"Email input: '{email}'");
            Console.WriteLine($"Normalized email: '{normalizedEmail}'");

            // Lấy tất cả KhachHang để debug
            var allKhachHangs = await _context.KhachHangs.ToListAsync();
            Console.WriteLine($"Tổng số KhachHang trong DB: {allKhachHangs.Count}");
            foreach (var kh in allKhachHangs)
            {
                Console.WriteLine($"KhachHang: ID={kh.KhachHangId}, Email='{kh.EmailCuaKhachHang}', TaiKhoanId={kh.TaiKhoanId}");
            }

            // Lấy tất cả NhanVien để debug
            var allNhanViens = await _context.NhanViens.ToListAsync();
            Console.WriteLine($"Tổng số NhanVien trong DB: {allNhanViens.Count}");
            foreach (var nv in allNhanViens)
            {
                Console.WriteLine($"NhanVien: ID={nv.NhanVienId}, Email='{nv.Email}', TaiKhoanId={nv.TaiKhoanId}");
            }

            // Tìm trong bảng KhachHang - ưu tiên tìm KhachHang có TaiKhoanId
            var khachHang = await _context.KhachHangs
                .Where(kh => kh.EmailCuaKhachHang != null && kh.EmailCuaKhachHang.ToLower().Trim() == normalizedEmail)
                .OrderByDescending(kh => kh.TaiKhoanId != null) // Ưu tiên KhachHang có TaiKhoanId
                .ThenBy(kh => kh.KhachHangId) // Nếu cùng có hoặc không có TaiKhoanId, lấy ID nhỏ hơn
                .FirstOrDefaultAsync();

            Console.WriteLine($"Tìm trong KhachHang: {(khachHang != null ? "TÌM THẤY" : "KHÔNG TÌM THẤY")}");
            if (khachHang != null)
            {
                Console.WriteLine($"KhachHang: ID={khachHang.KhachHangId}, Email='{khachHang.EmailCuaKhachHang}', TaiKhoanId={khachHang.TaiKhoanId}");
            }

            if (khachHang != null && khachHang.TaiKhoanId.HasValue)
            {
                var result = await GetByIdAsync(khachHang.TaiKhoanId.Value);
                Console.WriteLine($"Kết quả từ KhachHang: {(result != null ? "TÌM THẤY TÀI KHOẢN" : "KHÔNG TÌM THẤY TÀI KHOẢN")}");
                if (result != null)
                {
                    Console.WriteLine($"Tài khoản: ID={result.TaiKhoanId}, UserName={result.UserName}");
                }
                return result;
            }

            // Nếu không thấy, tìm trong bảng NhanVien
            var nhanVien = await _context.NhanViens
                .FirstOrDefaultAsync(nv => nv.Email != null && nv.Email.ToLower().Trim() == normalizedEmail);

            Console.WriteLine($"Tìm trong NhanVien: {(nhanVien != null ? "TÌM THẤY" : "KHÔNG TÌM THẤY")}");
            if (nhanVien != null)
            {
                Console.WriteLine($"NhanVien: ID={nhanVien.NhanVienId}, Email='{nhanVien.Email}', TaiKhoanId={nhanVien.TaiKhoanId}");
            }

            if (nhanVien != null && nhanVien.TaiKhoanId.HasValue)
            {
                var result = await GetByIdAsync(nhanVien.TaiKhoanId.Value);
                Console.WriteLine($"Kết quả từ NhanVien: {(result != null ? "TÌM THẤY TÀI KHOẢN" : "KHÔNG TÌM THẤY TÀI KHOẢN")}");
                if (result != null)
                {
                    Console.WriteLine($"Tài khoản: ID={result.TaiKhoanId}, UserName={result.UserName}");
                }
                return result;
            }

            Console.WriteLine("Không tìm thấy email ở đâu cả");
            Console.WriteLine($"=== KẾT THÚC DEBUG FindByEmailAsync ===");
            return null; // Không tìm thấy email ở đâu cả
        }

        public async Task UpdatePasswordAsync(Guid taiKhoanId, string newPassword)
        {
            var existingAccount = await _context.TaiKhoans.FindAsync(taiKhoanId);
            if (existingAccount == null)
            {
                throw new KeyNotFoundException("Tài khoản không tồn tại.");
            }

            // Lưu mật khẩu trực tiếp không mã hóa
            existingAccount.Password = newPassword;
            existingAccount.NgayCapNhatCuoiCung = DateTime.UtcNow;

            _context.TaiKhoans.Update(existingAccount);
            await _context.SaveChangesAsync();
        }
    }
}