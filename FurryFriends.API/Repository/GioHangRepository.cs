﻿using FurryFriends.API.Data;
using FurryFriends.API.Models;
using FurryFriends.API.Models.DTO;
using FurryFriends.API.Repository.IRepository;
using Microsoft.EntityFrameworkCore;

namespace FurryFriends.API.Repository
{
    public class GioHangRepository : IGioHangRepository
    {
        private readonly AppDbContext _context;

        public GioHangRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<GioHangChiTietDTO> ConvertToDTOAsync(GioHangChiTiet entity)
        {
            var sanPhamChiTiet = await _context.SanPhamChiTiets
                .Include(s => s.SanPham)
                .FirstOrDefaultAsync(s => s.SanPhamChiTietId == entity.SanPhamChiTietId);

            return new GioHangChiTietDTO
            {
                GioHangChiTietId = entity.GioHangChiTietId,
                SanPhamChiTietId = entity.SanPhamChiTietId,
                SanPhamId = entity.SanPhamId,
                SoLuong = entity.SoLuong,
                DonGia = entity.DonGia,
                ThanhTien = entity.ThanhTien,
                HinhAnh = "",
                TenSanPham = sanPhamChiTiet?.SanPham?.TenSanPham ?? "Không rõ"
            };
        }


        public async Task<SanPhamChiTiet> GetSanPhamChiTietByIdAsync(Guid id)
        {
            return await _context.SanPhamChiTiets
                .Include(sp => sp.SanPham) // 👈 BẮT BUỘC để lấy tên sản phẩm
                .FirstOrDefaultAsync(sp => sp.SanPhamChiTietId == id);
        }

        public async Task<GioHangDTO> GetGioHangByKhachHangIdAsync(Guid khachHangId)
        {
            var gioHang = await _context.GioHangs
                .Include(g => g.GioHangChiTiets)
                    .ThenInclude(ct => ct.SanPhamChiTiet)
                        .ThenInclude(spct => spct.SanPham)
                .Include(g => g.GioHangChiTiets)
                    .ThenInclude(ct => ct.SanPhamChiTiet)
                        .ThenInclude(spct => spct.MauSac)
                .Include(g => g.GioHangChiTiets)
                    .ThenInclude(ct => ct.SanPhamChiTiet)
                        .ThenInclude(spct => spct.KichCo)
                .Include(g => g.GioHangChiTiets)
                    .ThenInclude(ct => ct.SanPhamChiTiet)
                        .ThenInclude(spct => spct.Anh)
                .FirstOrDefaultAsync(g => g.KhachHangId == khachHangId);

            if (gioHang == null) return null;
            var tongTien = gioHang.GioHangChiTiets.Sum(ct => ct.ThanhTien);

            var dto = new GioHangDTO
            {
                GioHangId = gioHang.GioHangId,
                KhachHangId = gioHang.KhachHangId,
                NgayTao = gioHang.NgayTao,
                TrangThai = gioHang.TrangThai ? 1 : 0,
                GioHangChiTiets = gioHang.GioHangChiTiets.Select(ct => new GioHangChiTietDTO
                {
                    GioHangChiTietId = ct.GioHangChiTietId,
                    SanPhamChiTietId = ct.SanPhamChiTietId,
                    SanPhamId = ct.SanPhamId,
                    TenSanPham = ct.SanPhamChiTiet?.SanPham?.TenSanPham ?? "Không rõ",
                    SoLuong = ct.SoLuong,
                    DonGia = ct.DonGia,
                    ThanhTien = ct.ThanhTien,
                    MauSac = ct.SanPhamChiTiet?.MauSac?.TenMau ?? "",
                    KichCo = ct.SanPhamChiTiet?.KichCo?.TenKichCo ?? "",
                    AnhSanPham = ct.SanPhamChiTiet?.Anh?.DuongDan ?? ""
                }).ToList(),
                TongTienSauGiam = tongTien
            };

            return dto;
        }


        public async Task<GioHangChiTiet> AddSanPhamVaoGioAsync(Guid khachHangId, Guid sanPhamChiTietId, int soLuong)
        {
            var gioHang = await _context.GioHangs
                .FirstOrDefaultAsync(g => g.KhachHangId == khachHangId);

            if (gioHang == null)
            {
                gioHang = new GioHang
                {
                    GioHangId = Guid.NewGuid(),
                    KhachHangId = khachHangId,
                    NgayTao = DateTime.UtcNow,
                    TrangThai = true
                };
                await _context.GioHangs.AddAsync(gioHang);
                await _context.SaveChangesAsync(); // ❗ Thêm dòng này
            }

            // ❗️Tìm theo SanPhamChiTietId
            var chiTiet = await _context.GioHangChiTiets
                .FirstOrDefaultAsync(ct => ct.GioHangId == gioHang.GioHangId && ct.SanPhamChiTietId == sanPhamChiTietId);

            if (chiTiet != null)
            {
                chiTiet.SoLuong += soLuong;
                chiTiet.ThanhTien = chiTiet.SoLuong * chiTiet.DonGia;
                chiTiet.NgayCapNhat = DateTime.UtcNow;
            }
            else
            {
                var spct = await _context.SanPhamChiTiets
                    .FirstOrDefaultAsync(sp => sp.SanPhamChiTietId == sanPhamChiTietId);

                if (spct == null)
                    throw new Exception("Không tìm thấy sản phẩm chi tiết.");

                chiTiet = new GioHangChiTiet
                {
                    GioHangChiTietId = Guid.NewGuid(),
                    GioHangId = gioHang.GioHangId,
                    SanPhamChiTietId = sanPhamChiTietId,
                    SanPhamId = spct.SanPhamId, // ✅ Sửa ở đây
                    SoLuong = soLuong,
                    DonGia = spct.Gia,
                    ThanhTien = spct.Gia * soLuong,
                    TrangThai = true,
                    NgayTao = DateTime.UtcNow
                };
                await _context.GioHangChiTiets.AddAsync(chiTiet);
            }

            await _context.SaveChangesAsync();
            return chiTiet;
        }


        public async Task<GioHangChiTiet> UpdateSoLuongAsync(Guid gioHangChiTietId, int soLuong)
        {
            var ct = await _context.GioHangChiTiets.FindAsync(gioHangChiTietId);
            if (ct == null) return null;

            ct.SoLuong = soLuong;
            ct.ThanhTien = ct.DonGia * soLuong;
            ct.NgayCapNhat = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return ct;
        }

        public async Task<bool> RemoveSanPhamKhoiGioAsync(Guid gioHangChiTietId)
        {
            var ct = await _context.GioHangChiTiets.FindAsync(gioHangChiTietId);
            if (ct == null) return false;

            _context.GioHangChiTiets.Remove(ct);
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<GioHang> GetGioHangEntityByKhachHangIdAsync(Guid khachHangId)
        {
            return await _context.GioHangs
                .Include(g => g.GioHangChiTiets)
                    .ThenInclude(ct => ct.SanPhamChiTiet)
                .FirstOrDefaultAsync(g => g.KhachHangId == khachHangId);
        }

        public async Task<object> ThanhToanAsync(ThanhToanDTO dto)
        {
            var gioHang = await _context.GioHangs
                .Include(g => g.GioHangChiTiets)
                .FirstOrDefaultAsync(g => g.KhachHangId == dto.KhachHangId);

            if (gioHang == null || !gioHang.GioHangChiTiets.Any())
                return new { Success = false, Message = "Giỏ hàng trống." };

            decimal tongTienGoc = gioHang.GioHangChiTiets.Sum(ct => ct.ThanhTien);
            decimal? soTienGiam = null;
            decimal tongThanhToan = tongTienGoc;

            if (dto.VoucherId.HasValue)
            {
                var voucher = await _context.Vouchers.FirstOrDefaultAsync(v =>
                    v.VoucherId == dto.VoucherId.Value &&
                    v.TrangThai == 1 &&
                    v.NgayBatDau <= DateTime.Now &&
                    v.NgayKetThuc >= DateTime.Now &&
                    v.SoLuong > 0);

                if (voucher != null)
                {
                    soTienGiam = tongTienGoc * (voucher.PhanTramGiam / 100m);

                    // Giới hạn mức giảm nếu có
                    if (voucher.GiaTriGiamToiDa.HasValue && soTienGiam > voucher.GiaTriGiamToiDa.Value)
                    {
                        soTienGiam = voucher.GiaTriGiamToiDa.Value;
                    }

                    tongThanhToan -= soTienGiam ?? 0;
                    voucher.SoLuong -= 1;
                }
            }

            var taiKhoan = await _context.TaiKhoans.FirstOrDefaultAsync(tk => tk.TaiKhoanId == dto.TaiKhoanId);
            if (taiKhoan == null)
            {
                Console.WriteLine("❌ Không tìm thấy tài khoản với ID: " + dto.TaiKhoanId);
                return new { Success = false, Message = "Tài khoản không tồn tại." };
            }
            else
            {
                Console.WriteLine("✅ Đã tìm thấy tài khoản: " + taiKhoan.UserName);
            }


            var hoaDonId = Guid.NewGuid();

            var diaChi = await _context.DiaChiKhachHangs
                .FirstOrDefaultAsync(d => d.DiaChiId == dto.DiaChiId && d.KhachHangId == dto.KhachHangId);

            if (diaChi == null)
            {
                return new { Success = false, Message = "Địa chỉ không tồn tại." };
            }

            var diaChiDayDu = $"{diaChi.ThanhPho} - {diaChi.PhuongXa} - {diaChi.MoTa} - {diaChi.TenDiaChi}";

            var hoaDon = new HoaDon
            {
                HoaDonId = hoaDonId,
                KhachHangId = dto.KhachHangId,
                TaiKhoanId = dto.TaiKhoanId,
                NhanVienId = dto.NhanVienId,
                HinhThucThanhToanId = dto.HinhThucThanhToanId,
                NgayTao = DateTime.UtcNow,
                TongTien = tongTienGoc,
                TongTienSauKhiGiam = tongThanhToan,
                VoucherId = dto.VoucherId,
                TrangThai = 1,
                TenCuaKhachHang = dto.TenCuaKhachHang,
                SdtCuaKhachHang = dto.SdtCuaKhachHang,
                EmailCuaKhachHang = dto.EmailCuaKhachHang,
                LoaiHoaDon = dto.LoaiHoaDon,
                GhiChu = dto.GhiChu,
                DiaChiCuaKhachHang = diaChiDayDu // 👉 GÁN ĐỊA CHỈ Ở ĐÂY
            };


            // ✅ Gán chi tiết hóa đơn sau khi đã có biến `hoaDonId`
            hoaDon.HoaDonChiTiets = new List<HoaDonChiTiet>();

            foreach (var ct in gioHang.GioHangChiTiets)
            {
                var spct = await _context.SanPhamChiTiets
                    .Include(sp => sp.SanPham)
                        .ThenInclude(sp => sp.ThuongHieu)
                    .Include(sp => sp.SanPham)
                        .ThenInclude(sp => sp.SanPhamChatLieus)
                            .ThenInclude(cl => cl.ChatLieu)
                    .Include(sp => sp.SanPham)
                        .ThenInclude(sp => sp.SanPhamThanhPhans)
                            .ThenInclude(tp => tp.ThanhPhan)
                    .Include(sp => sp.MauSac)
                    .Include(sp => sp.KichCo)
                    .Include(sp => sp.Anh)
                    .FirstOrDefaultAsync(sp => sp.SanPhamChiTietId == ct.SanPhamChiTietId);

                if (spct == null) continue;

                var sp = spct.SanPham;

                var chiTietHoaDon = new HoaDonChiTiet
                {
                    HoaDonChiTietId = Guid.NewGuid(),
                    HoaDonId = hoaDonId,
                    SanPhamId = ct.SanPhamId,
                    SoLuongSanPham = ct.SoLuong,
                    Gia = ct.DonGia,
                    GiaLucMua = ct.DonGia,

                    TenSanPhamLucMua = sp?.TenSanPham ?? "Không rõ",
                    MoTaSanPhamLucMua = null, // Bỏ hoặc thêm property MoTa vào SanPham nếu cần
                    ThuongHieuLucMua = sp?.ThuongHieu?.TenThuongHieu,
                    ChatLieuLucMua = sp?.SanPhamChatLieus != null
                        ? string.Join(", ", sp.SanPhamChatLieus.Select(cl => cl.ChatLieu.TenChatLieu))
                        : null,
                    ThanhPhanLucMua = sp?.SanPhamThanhPhans != null
                        ? string.Join(", ", sp.SanPhamThanhPhans.Select(tp => tp.ThanhPhan.TenThanhPhan))
                        : null,

                    KichCoLucMua = spct.KichCo?.TenKichCo,
                    MauSacLucMua = spct.MauSac?.TenMau,
                    AnhSanPhamLucMua = spct.Anh?.DuongDan
                };

                hoaDon.HoaDonChiTiets.Add(chiTietHoaDon);
            }

            //hoaDon.HoaDonChiTiets = gioHang.GioHangChiTiets.Select(ct => new HoaDonChiTiet
            //{
            //    HoaDonChiTietId = Guid.NewGuid(),
            //    HoaDonId = hoaDonId,
            //    SanPhamId = ct.SanPhamId,
            //    SoLuongSanPham = ct.SoLuong,
            //    Gia = ct.DonGia
            //}).ToList();

            await _context.HoaDons.AddAsync(hoaDon);

            // Nếu cần đảm bảo tracking từng item (tuỳ cấu hình cascade):
            foreach (var item in hoaDon.HoaDonChiTiets)
            {
                _context.HoaDonChiTiets.Add(item);
            }

            _context.GioHangChiTiets.RemoveRange(gioHang.GioHangChiTiets);
            try
            {
                var result = await _context.SaveChangesAsync();
                Console.WriteLine($"[LOG] SaveChanges thành công: {result}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ SaveChanges lỗi: {ex.Message}");
                return new { Success = false, Message = "Lỗi khi lưu hóa đơn.", Error = ex.Message };
            }

            Console.WriteLine($"👉 HoaDonId trả về: {hoaDon.HoaDonId}");
            Console.WriteLine($"HinhThucThanhToanId: {dto.HinhThucThanhToanId}");
            Console.WriteLine($"[LOG] ✅ Hóa đơn tạo: Id = {hoaDon.HoaDonId}, Tổng tiền = {hoaDon.TongTien}");

            var hinhThuc = await _context.HinhThucThanhToans
                .Where(ht => ht.HinhThucThanhToanId == dto.HinhThucThanhToanId)
                .Select(ht => ht.TenHinhThuc)
                .FirstOrDefaultAsync();


            return new ThanhToanResultDTO
            {
                HoaDonId = hoaDonId,
                TenCuaKhachHang = dto.TenCuaKhachHang,
                SdtCuaKhachHang = dto.SdtCuaKhachHang,
                EmailCuaKhachHang = dto.EmailCuaKhachHang,
                DiaChiCuaKhachHang = diaChiDayDu,
                NgayTao = hoaDon.NgayTao,
                HinhThucThanhToan = hinhThuc,// lấy từ lookup nào đó,
                GhiChu = dto.GhiChu,
                TongTien = tongTienGoc,
                TongTienSauKhiGiam = tongThanhToan,
                ChiTietSanPham = hoaDon.HoaDonChiTiets.Select(ct => new HoaDonChiTietOutputDTO
                {
                    TenSanPhamLucMua = ct.TenSanPhamLucMua,
                    MoTaSanPhamLucMua = ct.MoTaSanPhamLucMua,
                    ThuongHieuLucMua = ct.ThuongHieuLucMua,
                    KichCoLucMua = ct.KichCoLucMua,
                    MauSacLucMua = ct.MauSacLucMua,
                    ChatLieuLucMua = ct.ChatLieuLucMua,
                    ThanhPhanLucMua = ct.ThanhPhanLucMua,
                    AnhSanPhamLucMua = ct.AnhSanPhamLucMua,
                    SoLuong = ct.SoLuongSanPham,
                    GiaLucMua = ct.GiaLucMua
                }).ToList()
            };

        }


    }

}
