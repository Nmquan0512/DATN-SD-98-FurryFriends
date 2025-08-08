using AutoMapper;
using AutoMapper.QueryableExtensions;
using FurryFriends.API.Data;
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
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<BanHangRepository> _logger;

        public BanHangRepository(AppDbContext context, IMapper mapper, ILogger<BanHangRepository> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        #region Hóa Đơn

        public async Task<IEnumerable<HoaDonBanHangDto>> GetAllHoaDonsAsync()
        {
            return await _context.HoaDons
                .OrderByDescending(h => h.NgayTao)
                .ProjectTo<HoaDonBanHangDto>(_mapper.ConfigurationProvider) // Dùng ProjectTo để tối ưu query
                .ToListAsync();
        }

        public async Task<HoaDonBanHangDto> GetHoaDonByIdAsync(Guid id)
        {
            var hoaDon = await GetFullHoaDonQuery()
                                 .FirstOrDefaultAsync(h => h.HoaDonId == id);

            if (hoaDon == null) throw new KeyNotFoundException("Hóa đơn không tồn tại.");

            return await MapToHoaDonDto(hoaDon);
        }

        public async Task<HoaDonBanHangDto> TaoHoaDonAsync(TaoHoaDonRequest request)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var hoaDon = new HoaDon
                {
                    HoaDonId = Guid.NewGuid(),
                    NgayTao = DateTime.Now,
                    TrangThai = (int)TrangThaiHoaDon.ChuaThanhToan,
                    GhiChu = request.GhiChu,
                    NhanVienId = request.NhanVienId,
                    // Bỏ trống các ID không cần thiết khi mới tạo
                    HinhThucThanhToanId = Guid.Empty,
                };

                // Gán khách hàng
                if (!request.LaKhachLe && request.KhachHangId.HasValue)
                {
                    await GanKhachHangNoSave(hoaDon, request.KhachHangId.Value);
                }
                else
                {
                    // Tìm hoặc tạo khách lẻ
                    var khachLe = await _context.KhachHangs.FirstOrDefaultAsync(k => k.TenKhachHang == "Khách lẻ");
                    if (khachLe == null)
                    {
                        khachLe = new KhachHang { KhachHangId = Guid.NewGuid(), TenKhachHang = "Khách lẻ", NgayTaoTaiKhoan = DateTime.Now, TrangThai = 1 };
                        await _context.KhachHangs.AddAsync(khachLe);
                    }
                    hoaDon.KhachHangId = khachLe.KhachHangId;
                    hoaDon.TenCuaKhachHang = "Khách lẻ";
                }

                // Xử lý đơn giao hàng
                if (request.GiaoHang)
                {
                    hoaDon.LoaiHoaDon = "GiaoHang";
                    if (request.DiaChiMoi == null) throw new InvalidOperationException("Đơn giao hàng phải có thông tin địa chỉ mới.");

                    var newDiaChi = _mapper.Map<DiaChiKhachHang>(request.DiaChiMoi);
                    newDiaChi.DiaChiId = Guid.NewGuid();
                    newDiaChi.KhachHangId = hoaDon.KhachHangId;
                    newDiaChi.NgayTao = DateTime.Now;
                    newDiaChi.NgayCapNhat = DateTime.Now;

                    await _context.DiaChiKhachHangs.AddAsync(newDiaChi);
                    hoaDon.DiaChiGiaoHangId = newDiaChi.DiaChiId;
                }
                else
                {
                    hoaDon.LoaiHoaDon = "BanTaiQuay";
                }

                await _context.HoaDons.AddAsync(hoaDon);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return await GetHoaDonByIdAsync(hoaDon.HoaDonId);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Lỗi khi tạo hóa đơn.");
                throw;
            }
        }

        public async Task<HoaDonBanHangDto> HuyHoaDonAsync(Guid hoaDonId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var hoaDon = await GetEditableHoaDon(hoaDonId);

                // Hoàn trả số lượng sản phẩm
                foreach (var item in hoaDon.HoaDonChiTiets)
                {
                    var sanPhamChiTiet = await _context.SanPhamChiTiets.FindAsync(item.SanPhamChiTietId);
                    if (sanPhamChiTiet != null) sanPhamChiTiet.SoLuong += item.SoLuongSanPham;
                }

                // Hoàn trả voucher
                if (hoaDon.VoucherId.HasValue)
                {
                    var voucher = await _context.Vouchers.FindAsync(hoaDon.VoucherId.Value);
                    if (voucher != null) voucher.SoLuong++;
                }

                hoaDon.TrangThai = (int)TrangThaiHoaDon.DaHuy;
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return await GetHoaDonByIdAsync(hoaDonId);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, $"Lỗi khi hủy hóa đơn {hoaDonId}");
                throw;
            }
        }

        #endregion

        #region Quản lý Sản phẩm trong Hóa đơn (LOGIC ĐÃ SỬA ĐÚNG)

        public async Task<HoaDonBanHangDto> ThemSanPhamVaoHoaDonAsync(ThemSanPhamVaoHoaDonRequest request)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var hoaDon = await GetEditableHoaDon(request.HoaDonId);
                var sanPhamChiTiet = await _context.SanPhamChiTiets.FindAsync(request.SanPhamChiTietId);

                if (sanPhamChiTiet == null) throw new KeyNotFoundException("Sản phẩm không tồn tại.");
                if (sanPhamChiTiet.SoLuong < request.SoLuong) throw new InvalidOperationException("Số lượng sản phẩm trong kho không đủ.");

                var existingItem = hoaDon.HoaDonChiTiets.FirstOrDefault(hct => hct.SanPhamChiTietId == request.SanPhamChiTietId);
                if (existingItem != null)
                {
                    existingItem.SoLuongSanPham += request.SoLuong;
                }
                else
                {
                    var newItem = new HoaDonChiTiet
                    {
                        HoaDonId = hoaDon.HoaDonId,
                        SanPhamChiTietId = sanPhamChiTiet.SanPhamChiTietId,
                        SoLuongSanPham = request.SoLuong,
                        Gia = sanPhamChiTiet.Gia
                    };
                    await _context.HoaDonChiTiets.AddAsync(newItem);
                }

                sanPhamChiTiet.SoLuong -= request.SoLuong;
                await TinhToanLaiTienHoaDon(hoaDon);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return await GetHoaDonByIdAsync(hoaDon.HoaDonId);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Lỗi khi thêm sản phẩm vào hóa đơn.");
                throw;
            }
        }

        public async Task<HoaDonBanHangDto> XoaSanPhamKhoiHoaDonAsync(Guid hoaDonId, Guid sanPhamChiTietId)
        {
            // Tương tự CapNhatSoLuongSanPhamAsync với số lượng là 0
            return await CapNhatSoLuongSanPhamAsync(hoaDonId, sanPhamChiTietId, 0);
        }

        public async Task<HoaDonBanHangDto> CapNhatSoLuongSanPhamAsync(Guid hoaDonId, Guid sanPhamChiTietId, int soLuongMoi)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var hoaDon = await GetEditableHoaDon(hoaDonId);
                var itemToUpdate = hoaDon.HoaDonChiTiets.FirstOrDefault(hct => hct.SanPhamChiTietId == sanPhamChiTietId);
                if (itemToUpdate == null) throw new KeyNotFoundException("Sản phẩm không có trong hóa đơn.");

                var sanPhamChiTiet = await _context.SanPhamChiTiets.FindAsync(sanPhamChiTietId);
                if (sanPhamChiTiet == null) throw new KeyNotFoundException("Sản phẩm không tồn tại.");

                int soLuongCu = itemToUpdate.SoLuongSanPham;
                int soLuongTonKhoHienTai = sanPhamChiTiet.SoLuong;

                if (soLuongTonKhoHienTai + soLuongCu < soLuongMoi)
                    throw new InvalidOperationException("Số lượng sản phẩm trong kho không đủ.");

                sanPhamChiTiet.SoLuong = soLuongTonKhoHienTai + soLuongCu - soLuongMoi;

                if (soLuongMoi <= 0)
                {
                    _context.HoaDonChiTiets.Remove(itemToUpdate);
                }
                else
                {
                    itemToUpdate.SoLuongSanPham = soLuongMoi;
                }

                await TinhToanLaiTienHoaDon(hoaDon);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return await GetHoaDonByIdAsync(hoaDonId);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Lỗi khi cập nhật số lượng sản phẩm.");
                throw;
            }
        }

        #endregion

        #region Voucher & Khách hàng

        public async Task<HoaDonBanHangDto> ApDungVoucherAsync(Guid hoaDonId, string maVoucher)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var hoaDon = await GetEditableHoaDon(hoaDonId);
                var voucher = await _context.Vouchers.FirstOrDefaultAsync(v => v.TenVoucher.ToLower() == maVoucher.ToLower());

                if (voucher == null) throw new KeyNotFoundException("Mã voucher không tồn tại.");
                if (voucher.NgayKetThuc < DateTime.Now) throw new InvalidOperationException("Voucher đã hết hạn.");
                if (voucher.SoLuong <= 0) throw new InvalidOperationException("Voucher đã hết lượt sử dụng.");

                // Gỡ voucher cũ nếu có
                if (hoaDon.VoucherId.HasValue) await GoBoVoucherNoSave(hoaDon);

                hoaDon.VoucherId = voucher.VoucherId;
                voucher.SoLuong--;

                await TinhToanLaiTienHoaDon(hoaDon);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return await GetHoaDonByIdAsync(hoaDonId);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<HoaDonBanHangDto> GoBoVoucherAsync(Guid hoaDonId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            var hoaDon = await GetEditableHoaDon(hoaDonId);

            await GoBoVoucherNoSave(hoaDon);
            await TinhToanLaiTienHoaDon(hoaDon);

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
            return await GetHoaDonByIdAsync(hoaDonId);
        }

        public async Task<HoaDonBanHangDto> GanKhachHangAsync(Guid hoaDonId, Guid khachHangId)
        {
            var hoaDon = await GetEditableHoaDon(hoaDonId);
            await GanKhachHangNoSave(hoaDon, khachHangId);
            await _context.SaveChangesAsync();
            return await GetHoaDonByIdAsync(hoaDonId);
        }

        #endregion

        #region Thanh Toán

        public async Task<HoaDonBanHangDto> ThanhToanHoaDonAsync(ThanhToanRequest request)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var hoaDon = await _context.HoaDons
                    .Include(h => h.HoaDonChiTiets)
                    .FirstOrDefaultAsync(h => h.HoaDonId == request.HoaDonId);

                if (hoaDon == null) throw new KeyNotFoundException("Hóa đơn không tồn tại.");
                if (hoaDon.TrangThai != (int)TrangThaiHoaDon.ChuaThanhToan)
                    throw new InvalidOperationException("Hóa đơn đã được xử lý (thanh toán/hủy).");
                if (!hoaDon.HoaDonChiTiets.Any())
                    throw new InvalidOperationException("Không thể thanh toán hóa đơn rỗng.");

                var hinhThucTT = await _context.HinhThucThanhToans.FindAsync(request.HinhThucThanhToanId);
                if (hinhThucTT == null) throw new KeyNotFoundException("Hình thức thanh toán không tồn tại.");

                await TinhToanLaiTienHoaDon(hoaDon); // Tính lại tiền lần cuối cho chắc

                if (hinhThucTT.TenHinhThuc.Contains("Tiền mặt") && request.TienKhachDua < hoaDon.TongTienSauKhiGiam)
                    throw new InvalidOperationException("Số tiền khách đưa không đủ.");

                hoaDon.HinhThucThanhToanId = hinhThucTT.HinhThucThanhToanId;
                hoaDon.TrangThai = (int)TrangThaiHoaDon.DaThanhToan;
                hoaDon.NgayNhanHang = DateTime.Now; // Coi như ngày thanh toán là ngày nhận tại quầy
                hoaDon.GhiChu = string.IsNullOrEmpty(hoaDon.GhiChu) ? request.GhiChuThanhToan : hoaDon.GhiChu + " | " + request.GhiChuThanhToan;
                if (hoaDon.KhachHangId != Guid.Empty)
                {
                    var khachHang = await _context.KhachHangs.FindAsync(hoaDon.KhachHangId);
                    if (khachHang != null && khachHang.TenKhachHang != "Khách lẻ")
                    {
                        khachHang.DiemKhachHang = (khachHang.DiemKhachHang ?? 0) + (int)(hoaDon.TongTienSauKhiGiam / 10000);
                    }
                }


                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return await GetHoaDonByIdAsync(hoaDon.HoaDonId);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
        #endregion

        #region Tìm kiếm và Khách hàng

        public Task<IEnumerable<SanPhamBanHangDto>> TimKiemSanPhamAsync(string keyword)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<KhachHangDto>> TimKiemKhachHangAsync(string keyword)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<VoucherDto>> TimKiemVoucherHopLeAsync(Guid hoaDonId)
        {
            throw new NotImplementedException();
        }

        public async Task<KhachHangDto> TaoKhachHangMoiAsync(TaoKhachHangRequest request)
        {
            var sdtExists = await _context.KhachHangs.AnyAsync(k => k.SDT == request.SDT && k.SDT != null);
            if (sdtExists) throw new InvalidOperationException("Số điện thoại đã tồn tại.");

            var khachHang = _mapper.Map<KhachHang>(request);
            khachHang.KhachHangId = Guid.NewGuid();
            khachHang.NgayTaoTaiKhoan = DateTime.Now;
            khachHang.TrangThai = 1;

            await _context.KhachHangs.AddAsync(khachHang);
            await _context.SaveChangesAsync();

            return _mapper.Map<KhachHangDto>(khachHang);
        }

        #endregion

        #region Private Helper Methods

        private async Task<HoaDon> GetEditableHoaDon(Guid hoaDonId)
        {
            var hoaDon = await _context.HoaDons
                .Include(h => h.HoaDonChiTiets)
                .FirstOrDefaultAsync(h => h.HoaDonId == hoaDonId);

            if (hoaDon == null) throw new KeyNotFoundException("Hóa đơn không tồn tại.");
            if (hoaDon.TrangThai != (int)TrangThaiHoaDon.ChuaThanhToan)
                throw new InvalidOperationException("Không thể chỉnh sửa hóa đơn đã thanh toán hoặc đã hủy.");

            return hoaDon;
        }

        private async Task TinhToanLaiTienHoaDon(HoaDon hoaDon)
        {
            hoaDon.TongTien = hoaDon.HoaDonChiTiets.Sum(hct => hct.SoLuongSanPham * hct.Gia);
            decimal tienGiam = 0;
            if (hoaDon.VoucherId.HasValue && hoaDon.VoucherId != Guid.Empty)
            {
                var voucher = await _context.Vouchers.FindAsync(hoaDon.VoucherId);
                if (voucher != null)
                {
                    tienGiam = hoaDon.TongTien * (voucher.PhanTramGiam / 100);
                    if (voucher.GiaTriGiamToiDa.HasValue && tienGiam > voucher.GiaTriGiamToiDa.Value)
                    {
                        tienGiam = voucher.GiaTriGiamToiDa.Value;
                    }
                }
            }
            hoaDon.TongTienSauKhiGiam = hoaDon.TongTien - tienGiam;
        }

        private async Task GoBoVoucherNoSave(HoaDon hoaDon)
        {
            if (hoaDon.VoucherId.HasValue)
            {
                var oldVoucher = await _context.Vouchers.FindAsync(hoaDon.VoucherId.Value);
                if (oldVoucher != null) oldVoucher.SoLuong++;
                hoaDon.VoucherId = null;
            }
        }

        private async Task GanKhachHangNoSave(HoaDon hoaDon, Guid khachHangId)
        {
            var khachHang = await _context.KhachHangs.FindAsync(khachHangId);
            if (khachHang == null) throw new KeyNotFoundException("Khách hàng không tồn tại.");
            hoaDon.KhachHangId = khachHang.KhachHangId;
            hoaDon.TenCuaKhachHang = khachHang.TenKhachHang;
            hoaDon.SdtCuaKhachHang = khachHang.SDT;
            hoaDon.EmailCuaKhachHang = khachHang.EmailCuaKhachHang;
        }

        private IQueryable<HoaDon> GetFullHoaDonQuery()
        {
            return _context.HoaDons
               .AsNoTracking()
               .Include(h => h.HoaDonChiTiets)
                   .ThenInclude(hct => hct.SanPhamChiTiet)
                       .ThenInclude(spct => spct.SanPham)
               .Include(h => h.HoaDonChiTiets)
                   .ThenInclude(hct => hct.SanPhamChiTiet)
                   .ThenInclude(spct => spct.MauSac)
               .Include(h => h.HoaDonChiTiets)
                   .ThenInclude(hct => hct.SanPhamChiTiet)
                   .ThenInclude(spct => spct.KichCo)
               .Include(h => h.KhachHang)
               .Include(h => h.HinhThucThanhToan)
               .Include(h => h.Voucher);
        }

        private async Task<HoaDonBanHangDto> MapToHoaDonDto(HoaDon hoaDon)
        {
            var dto = _mapper.Map<HoaDonBanHangDto>(hoaDon);
            await TinhToanLaiTienHoaDon(hoaDon);
            dto.TongTien = hoaDon.TongTien;
            dto.ThanhTien = hoaDon.TongTienSauKhiGiam;
            dto.TienGiam = dto.TongTien - dto.ThanhTien;
            return dto;
        }
        #endregion
    }

    public enum TrangThaiHoaDon
    {
        ChuaThanhToan = 0,
        DaThanhToan = 1,
        DaHuy = 2,
        DangGiaoHang = 3,
        DaGiaoThanhCong = 4,
        HoanTra = 5
    }
}