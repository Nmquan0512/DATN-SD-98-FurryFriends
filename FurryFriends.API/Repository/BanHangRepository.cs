using AutoMapper;
using FurryFriends.API.Models;
using FurryFriends.API.Models.DTO.BanHang;
using FurryFriends.API.Models.DTO.BanHang.Requests;
using FurryFriends.API.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FurryFriends.API.Repository
{
    public class BanHangRepository : IBanHangRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public BanHangRepository(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<HoaDonBanHangDto> TaoHoaDon(TaoHoaDonRequest request)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var hoaDon = new HoaDon
                {
                    HoaDonId = Guid.NewGuid(),
                    NgayTao = DateTime.Now,
                    TrangThai = 0, // Chưa thanh toán
                    LoaiHoaDon = "BanTaiQuay",
                    GhiChu = request.GhiChu,
                    NhanVienId = Guid.Parse("ID_NHAN_VIEN_HIEN_TAI") // Lấy từ auth
                };

                if (!request.LaKhachLe && request.KhachHangId.HasValue)
                {
                    hoaDon.KhachHangId = request.KhachHangId.Value;
                    var khachHang = await _context.KhachHangs.FindAsync(request.KhachHangId.Value);
                    if (khachHang != null)
                    {
                        hoaDon.TenCuaKhachHang = khachHang.TenKhachHang;
                        hoaDon.SdtCuaKhachHang = khachHang.SDT;
                        hoaDon.EmailCuaKhachHang = khachHang.EmailCuaKhachHang;
                    }
                }
                else
                {
                    hoaDon.TenCuaKhachHang = "Khách lẻ";
                }

                await _context.HoaDons.AddAsync(hoaDon);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return _mapper.Map<HoaDonBanHangDto>(hoaDon);
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<HoaDonBanHangDto> ThemSanPhamVaoHoaDon(ThemSanPhamVaoHoaDonRequest request)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var hoaDon = await _context.HoaDons
                    .Include(h => h.HoaDonChiTiets)
                    .ThenInclude(hct => hct.SanPham)
                    .FirstOrDefaultAsync(h => h.HoaDonId == request.HoaDonId);

                if (hoaDon == null)
                    throw new Exception("Hóa đơn không tồn tại");

                var sanPhamChiTiet = await _context.SanPhamChiTiets
                    .Include(spct => spct.SanPham)
                    .FirstOrDefaultAsync(spct => spct.SanPhamChiTietId == request.SanPhamChiTietId);

                if (sanPhamChiTiet == null)
                    throw new Exception("Sản phẩm không tồn tại");

                if (sanPhamChiTiet.SoLuong < request.SoLuong)
                    throw new Exception("Số lượng sản phẩm không đủ");

                var existingItem = hoaDon.HoaDonChiTiets
                    .FirstOrDefault(hct => hct.SanPhamId == sanPhamChiTiet.SanPhamId);

                if (existingItem != null)
                {
                    existingItem.SoLuongSanPham += request.SoLuong;
                }
                else
                {
                    var newItem = new HoaDonChiTiet
                    {
                        HoaDonChiTietId = Guid.NewGuid(),
                        HoaDonId = hoaDon.HoaDonId,
                        SanPhamId = sanPhamChiTiet.SanPhamId,
                        SoLuongSanPham = request.SoLuong,
                        Gia = sanPhamChiTiet.Gia
                    };
                    await _context.HoaDonChiTiets.AddAsync(newItem);
                }

                // Trừ số lượng tồn kho
                sanPhamChiTiet.SoLuong -= request.SoLuong;
                _context.SanPhamChiTiets.Update(sanPhamChiTiet);

                // Cập nhật tổng tiền
                hoaDon.TongTien = hoaDon.HoaDonChiTiets.Sum(hct => hct.SoLuongSanPham * hct.Gia);
                hoaDon.TongTienSauKhiGiam = hoaDon.TongTien;

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return await GetHoaDonById(hoaDon.HoaDonId);
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<HoaDonBanHangDto> XoaSanPhamKhoiHoaDon(Guid hoaDonId, Guid sanPhamChiTietId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var hoaDon = await _context.HoaDons
                    .Include(h => h.HoaDonChiTiets)
                    .ThenInclude(hct => hct.SanPham)
                    .FirstOrDefaultAsync(h => h.HoaDonId == hoaDonId);

                if (hoaDon == null)
                    throw new Exception("Hóa đơn không tồn tại");

                var sanPhamChiTiet = await _context.SanPhamChiTiets
                    .FirstOrDefaultAsync(spct => spct.SanPhamChiTietId == sanPhamChiTietId);

                if (sanPhamChiTiet == null)
                    throw new Exception("Sản phẩm không tồn tại");

                var itemToRemove = hoaDon.HoaDonChiTiets
                    .FirstOrDefault(hct => hct.SanPhamId == sanPhamChiTiet.SanPhamId);

                if (itemToRemove == null)
                    throw new Exception("Sản phẩm không có trong hóa đơn");

                // Hoàn trả số lượng tồn kho
                sanPhamChiTiet.SoLuong += itemToRemove.SoLuongSanPham;
                _context.SanPhamChiTiets.Update(sanPhamChiTiet);

                // Xóa sản phẩm khỏi hóa đơn
                _context.HoaDonChiTiets.Remove(itemToRemove);

                // Cập nhật tổng tiền
                hoaDon.TongTien = hoaDon.HoaDonChiTiets
                    .Where(hct => hct.HoaDonChiTietId != itemToRemove.HoaDonChiTietId)
                    .Sum(hct => hct.SoLuongSanPham * hct.Gia);
                hoaDon.TongTienSauKhiGiam = hoaDon.TongTien;

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return await GetHoaDonById(hoaDon.HoaDonId);
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<HoaDonBanHangDto> CapNhatSoLuongSanPham(Guid hoaDonId, Guid sanPhamChiTietId, int soLuong)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var hoaDon = await _context.HoaDons
                    .Include(h => h.HoaDonChiTiets)
                    .ThenInclude(hct => hct.SanPham)
                    .FirstOrDefaultAsync(h => h.HoaDonId == hoaDonId);

                if (hoaDon == null)
                    throw new Exception("Hóa đơn không tồn tại");

                var sanPhamChiTiet = await _context.SanPhamChiTiets
                    .FirstOrDefaultAsync(spct => spct.SanPhamChiTietId == sanPhamChiTietId);

                if (sanPhamChiTiet == null)
                    throw new Exception("Sản phẩm không tồn tại");

                var itemToUpdate = hoaDon.HoaDonChiTiets
                    .FirstOrDefault(hct => hct.SanPhamId == sanPhamChiTiet.SanPhamId);

                if (itemToUpdate == null)
                    throw new Exception("Sản phẩm không có trong hóa đơn");

                // Tính toán chênh lệch số lượng
                int soLuongThayDoi = soLuong - itemToUpdate.SoLuongSanPham;

                // Kiểm tra tồn kho
                if (sanPhamChiTiet.SoLuong < soLuongThayDoi)
                    throw new Exception("Số lượng sản phẩm không đủ");

                // Cập nhật số lượng tồn kho
                sanPhamChiTiet.SoLuong -= soLuongThayDoi;
                _context.SanPhamChiTiets.Update(sanPhamChiTiet);

                // Cập nhật số lượng trong hóa đơn
                itemToUpdate.SoLuongSanPham = soLuong;

                // Cập nhật tổng tiền
                hoaDon.TongTien = hoaDon.HoaDonChiTiets.Sum(hct => hct.SoLuongSanPham * hct.Gia);
                hoaDon.TongTienSauKhiGiam = hoaDon.TongTien;

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return await GetHoaDonById(hoaDon.HoaDonId);
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<HoaDonBanHangDto> ApDungVoucher(Guid hoaDonId, Guid voucherId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var hoaDon = await _context.HoaDons
                    .FirstOrDefaultAsync(h => h.HoaDonId == hoaDonId);

                if (hoaDon == null)
                    throw new Exception("Hóa đơn không tồn tại");

                var voucher = await _context.Vouchers.FindAsync(voucherId);
                if (voucher == null)
                    throw new Exception("Voucher không tồn tại");

                if (voucher.NgayHetHan < DateTime.Now)
                    throw new Exception("Voucher đã hết hạn");

                if (voucher.SoLuong <= 0)
                    throw new Exception("Voucher đã hết số lượng");

                // Áp dụng voucher
                hoaDon.VoucherId = voucher.VoucherId;
                hoaDon.TongTienSauKhiGiam = hoaDon.TongTien * (1 - voucher.PhanTramGiamGia / 100);

                // Giảm số lượng voucher
                voucher.SoLuong--;
                _context.Vouchers.Update(voucher);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return await GetHoaDonById(hoaDon.HoaDonId);
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<HoaDonBanHangDto> ThanhToan(ThanhToanRequest request)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var hoaDon = await _context.HoaDons
                    .Include(h => h.HoaDonChiTiets)
                    .ThenInclude(hct => hct.SanPham)
                    .FirstOrDefaultAsync(h => h.HoaDonId == request.HoaDonId);

                if (hoaDon == null)
                    throw new Exception("Hóa đơn không tồn tại");

                // Kiểm tra trạng thái hóa đơn
                if (hoaDon.TrangThai == 1)
                    throw new Exception("Hóa đơn đã được thanh toán");

                // Cập nhật hình thức thanh toán
                hoaDon.HinhThucThanhToanId = request.HinhThucThanhToanId;
                hoaDon.TrangThai = 1; // Đã thanh toán
                hoaDon.NgayTao = DateTime.Now;

                // Xử lý thanh toán tiền mặt
                var hinhThucTT = await _context.HinhThucThanhToans.FindAsync(request.HinhThucThanhToanId);
                if (hinhThucTT != null && hinhThucTT.TenHinhThuc == "Tiền mặt")
                {
                    if (request.TienKhachDua < hoaDon.TongTienSauKhiGiam)
                        throw new Exception("Số tiền khách đưa không đủ");
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return await GetHoaDonById(hoaDon.HoaDonId);
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<HoaDonBanHangDto> GetHoaDonById(Guid id)
        {
            var hoaDon = await _context.HoaDons
                .Include(h => h.HoaDonChiTiets)
                .ThenInclude(hct => hct.SanPham)
                .Include(h => h.KhachHang)
                .Include(h => h.HinhThucThanhToan)
                .Include(h => h.Voucher)
                .FirstOrDefaultAsync(h => h.HoaDonId == id);

            if (hoaDon == null)
                throw new Exception("Hóa đơn không tồn tại");

            return _mapper.Map<HoaDonBanHangDto>(hoaDon);
        }

        public async Task<IEnumerable<SanPhamBanHangDto>> TimKiemSanPham(string keyword)
        {
            var query = _context.SanPhams
                .Include(sp => sp.SanPhamChiTiets)
                .ThenInclude(spct => spct.MauSac)
                .Include(sp => sp.SanPhamChiTiets)
                .ThenInclude(spct => spct.KichCo)
                .AsQueryable();

            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(sp =>
                    sp.TenSanPham.Contains(keyword) ||
                    sp.MaSanPham.Contains(keyword) ||
                    sp.SanPhamChiTiets.Any(spct =>
                        spct.MauSac.TenMau.Contains(keyword) ||
                        spct.KichCo.TenKichCo.Contains(keyword)));
            }

            var sanPhams = await query.ToListAsync();
            return _mapper.Map<IEnumerable<SanPhamBanHangDto>>(sanPhams);
        }

        public async Task<IEnumerable<KhachHangDto>> TimKiemKhachHang(string keyword)
        {
            var query = _context.KhachHangs.AsQueryable();

            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(kh =>
                    kh.TenKhachHang.Contains(keyword) ||
                    kh.SDT.Contains(keyword) ||
                    kh.EmailCuaKhachHang.Contains(keyword));
            }

            var khachHangs = await query
                .OrderBy(kh => kh.TenKhachHang)
                .Take(20)
                .ToListAsync();

            return _mapper.Map<IEnumerable<KhachHangDto>>(khachHangs);
        }

        public async Task<KhachHangDto> TaoKhachHang(TaoKhachHangRequest request)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // Kiểm tra SDT đã tồn tại chưa
                var existingKhachHang = await _context.KhachHangs
                    .FirstOrDefaultAsync(kh => kh.SDT == request.SDT);

                if (existingKhachHang != null)
                    throw new Exception("Số điện thoại đã được sử dụng");

                var khachHang = new KhachHang
                {
                    KhachHangId = Guid.NewGuid(),
                    TenKhachHang = request.TenKhachHang,
                    SDT = request.SDT,
                    EmailCuaKhachHang = request.Email,
                    DiemKhachHang = 0,
                    NgayTaoTaiKhoan = DateTime.Now,
                    NgayCapNhatCuoiCung = DateTime.Now,
                    TrangThai = 1 // Hoạt động
                };

                await _context.KhachHangs.AddAsync(khachHang);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return _mapper.Map<KhachHangDto>(khachHang);
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}