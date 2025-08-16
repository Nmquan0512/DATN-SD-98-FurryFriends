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
            // 1. Tải các đối tượng HoaDon và các dữ liệu liên quan cần thiết
            var hoaDons = await _context.HoaDons
                .AsNoTracking() // Dùng AsNoTracking để tăng hiệu năng cho truy vấn chỉ đọc
                .Include(h => h.KhachHang)
                .Include(h => h.Voucher)
                .OrderByDescending(h => h.NgayTao)
                .ToListAsync();

            // 2. Dùng _mapper.Map() để ánh xạ trong bộ nhớ. 
            //    Cách này sẽ sử dụng cấu hình đầy đủ của BanHangMappingProfile mà không bị lỗi.
            return _mapper.Map<IEnumerable<HoaDonBanHangDto>>(hoaDons);
        }
           public async Task<HoaDonBanHangDto> GanKhachLeAsync(Guid hoaDonId)
        {
            var hoaDon = await GetEditableHoaDon(hoaDonId);
            await GanKhachLeNoSave(hoaDon);
            await _context.SaveChangesAsync();
            return await GetHoaDonByIdAsync(hoaDonId);
        }

        public async Task<HoaDonBanHangDto> GetHoaDonByIdAsync(Guid id)
        {
            var hoaDon = await GetFullHoaDonQuery().FirstOrDefaultAsync(h => h.HoaDonId == id);
            if (hoaDon == null) throw new KeyNotFoundException("Hóa đơn không tồn tại.");

            // 1. Dùng AutoMapper để map các thuộc tính cơ bản
            var dto = _mapper.Map<HoaDonBanHangDto>(hoaDon);

            // 2. Map chi tiết hóa đơn
            dto.ChiTietHoaDon = hoaDon.HoaDonChiTiets.Select(hct => new ChiTietHoaDonDto
            {
                SanPhamChiTietId = hct.SanPhamChiTietId,
                TenSanPham = hct.SanPhamChiTiet.SanPham.TenSanPham,
                MauSac = hct.SanPhamChiTiet.MauSac.TenMau,
                KichCo = hct.SanPhamChiTiet.KichCo.TenKichCo,
                Gia = hct.SanPhamChiTiet.Gia, // Giá gốc từ sản phẩm để hiển thị
                GiaBan = hct.Gia,             // Giá bán thực tế tại thời điểm mua
                SoLuong = hct.SoLuongSanPham,
                ThanhTien = hct.Gia * hct.SoLuongSanPham, // Thành tiền của dòng này
                HinhAnh = hct.SanPhamChiTiet.Anh?.DuongDan,
                SoLuongTon = hct.SanPhamChiTiet.SoLuong
            }).ToList();

            // 3. Lấy trực tiếp các giá trị đã được tính toán và lưu trong DB
            //    Không cần tính toán lại ở đây!
            dto.TongTien = hoaDon.TongTien;
            dto.ThanhTien = hoaDon.TongTienSauKhiGiam;
            dto.TienGiam = hoaDon.TongTien - hoaDon.TongTienSauKhiGiam;

            return dto;
        }
        // Bạn có thể xóa phương thức MapToHoaDonDto đi
        // File: BanHangRepository.cs (API)

        public async Task<HoaDonBanHangDto> TaoHoaDonAsync(TaoHoaDonRequest request)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var oldPendingInvoices = await _context.HoaDons
            .Where(h => h.TrangThai == (int)TrangThaiHoaDon.ChuaThanhToan && h.NgayTao < DateTime.UtcNow.AddHours(-2))
            .ToListAsync();

                if (oldPendingInvoices.Any())
                {
                    _logger.LogInformation($"Dọn dẹp {oldPendingInvoices.Count} hóa đơn chờ cũ.");
                    // Với mỗi hóa đơn cũ, hoàn trả lại số lượng sản phẩm
                    foreach (var oldInvoice in oldPendingInvoices)
                    {
                        var details = await _context.HoaDonChiTiets
                                                    .Where(d => d.HoaDonId == oldInvoice.HoaDonId)
                                                    .ToListAsync();
                        foreach (var item in details)
                        {
                            var productDetail = await _context.SanPhamChiTiets.FindAsync(item.SanPhamChiTietId);
                            if (productDetail != null)
                            {
                                productDetail.SoLuong += item.SoLuongSanPham;
                            }
                        }
                        // Bạn cũng có thể hoàn trả voucher nếu cần
                    }
                    _context.HoaDons.RemoveRange(oldPendingInvoices);
                    await _context.SaveChangesAsync();
                }
                // 1. Xử lý Hình thức thanh toán
                var defaultHttt = await _context.HinhThucThanhToans.FirstOrDefaultAsync(h => h.TenHinhThuc == "Chưa xác định");
                if (defaultHttt == null)
                {
                    defaultHttt = new HinhThucThanhToan
                    {
                        HinhThucThanhToanId = Guid.NewGuid(),
                        TenHinhThuc = "Chưa xác định",
                        MoTa = "Thanh toán khi nhận hàng tại quầy" // SỬA LỖI 1
                    };
                    await _context.HinhThucThanhToans.AddAsync(defaultHttt);
                }

                // 2. Xử lý Khách hàng và tạo Hóa đơn
                var hoaDon = new HoaDon
                {
                    HoaDonId = Guid.NewGuid(),
                    NgayTao = DateTime.UtcNow,
                    TrangThai = (int)TrangThaiHoaDon.ChuaThanhToan,
                    GhiChu = request.GhiChu ?? "",
                    NhanVienId = request.NhanVienId,
                    HinhThucThanhToanId = defaultHttt.HinhThucThanhToanId,
                    TongTien = 0,
                    TongTienSauKhiGiam = 0,

                    LoaiHoaDon = request.GiaoHang ? "GiaoHang" : "BanTaiQuay"

                };

                if (!request.LaKhachLe && request.KhachHangId.HasValue)
                {
                    await GanKhachHangNoSave(hoaDon, request.KhachHangId.Value);
                }
                else
                {
                    var khachLe = await _context.KhachHangs.FirstOrDefaultAsync(k => k.TenKhachHang == "Khách lẻ");
                    if (khachLe == null)
                    {
                        khachLe = new KhachHang
                        {
                            TenKhachHang = "Khách lẻ",
                            NgayTaoTaiKhoan = DateTime.UtcNow,
                            TrangThai = 1,
                            EmailCuaKhachHang = "khachle@furryfriends.local", // SỬA LỖI 2
                            SDT = "0000000000"
                        };
                        await _context.KhachHangs.AddAsync(khachLe);
                    }
                    
                    hoaDon.KhachHangId = khachLe.KhachHangId;
                    hoaDon.TenCuaKhachHang = khachLe.TenKhachHang;
                    hoaDon.SdtCuaKhachHang = khachLe.SDT;
                    hoaDon.EmailCuaKhachHang = khachLe.EmailCuaKhachHang; // SỬA LỖI 3
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

        // File: API/Repository/BanHangRepository.cs
        public async Task<HoaDonBanHangDto> ThemSanPhamVaoHoaDonAsync(ThemSanPhamVaoHoaDonRequest request)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var hoaDon = await GetEditableHoaDon(request.HoaDonId);
                var sanPhamChiTiet = await _context.SanPhamChiTiets
                    .Include(spct => spct.DotGiamGiaSanPhams)
                        .ThenInclude(dggsp => dggsp.GiamGia)
                    .FirstOrDefaultAsync(spct => spct.SanPhamChiTietId == request.SanPhamChiTietId);

                if (sanPhamChiTiet == null) throw new KeyNotFoundException("Sản phẩm không tồn tại.");

                // <<< LOGIC MỚI: TÍNH GIÁ BÁN THỰC TẾ >>>
                var now = DateTime.UtcNow;
                var activeSale = sanPhamChiTiet.DotGiamGiaSanPhams
                    .Select(d => d.GiamGia)
                    .FirstOrDefault(gg => gg.TrangThai && gg.NgayBatDau <= now && gg.NgayKetThuc >= now);

                decimal actualSalePrice = sanPhamChiTiet.Gia; // Mặc định là giá gốc
                if (activeSale != null)
                {
                    actualSalePrice = sanPhamChiTiet.Gia - (sanPhamChiTiet.Gia * (activeSale.PhanTramKhuyenMai / 100));
                }
                // <<< KẾT THÚC LOGIC MỚI >>>

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
                        Gia = actualSalePrice // << SỬA LỖI: Lưu giá bán thực tế
                    };
                    await _context.HoaDonChiTiets.AddAsync(newItem);
                }

                sanPhamChiTiet.SoLuong -= request.SoLuong;
                await _context.SaveChangesAsync(); // Lưu thay đổi trước khi tính toán
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
        // Trong BanHangRepository.cs
        public async Task<IEnumerable<SanPhamBanHangDto>> TimKiemSanPhamAsync(string keyword)
        {
            var now = DateTime.UtcNow;
            var query = _context.SanPhamChiTiets.AsNoTracking().Where(spct => spct.TrangThai == 1 && spct.SoLuong > 0);
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                var lowerKeyword = keyword.ToLower();
                query = query.Where(spct => spct.SanPham.TenSanPham.ToLower().Contains(lowerKeyword));
            }

            var products = await query.Include(spct => spct.SanPham).Include(spct => spct.MauSac).Include(spct => spct.KichCo).Include(spct => spct.Anh)
                .Include(spct => spct.DotGiamGiaSanPhams).ThenInclude(dggsp => dggsp.GiamGia)
                .OrderByDescending(spct => spct.NgayTao).Take(20).ToListAsync();

            var result = products.Select(spct =>
            {
                var activeSale = spct.DotGiamGiaSanPhams.Select(d => d.GiamGia).FirstOrDefault(gg => gg.TrangThai && gg.NgayBatDau <= now && gg.NgayKetThuc >= now);
                decimal actualSalePrice = spct.Gia;
                if (activeSale != null) { actualSalePrice = spct.Gia - (spct.Gia * (activeSale.PhanTramKhuyenMai / 100)); }

                return new SanPhamBanHangDto
                {
                    SanPhamChiTietId = spct.SanPhamChiTietId,
                    TenSanPham = spct.SanPham.TenSanPham,
                    TenMauSac = spct.MauSac.TenMau,
                    TenKichCo = spct.KichCo.TenKichCo,
                    Gia = spct.Gia, // << Gán giá gốc
                    GiaBan = actualSalePrice, // << Gán giá bán thực tế
                    SoLuongTon = spct.SoLuong,
                    HinhAnh = spct.Anh?.DuongDan
                };
            }).ToList();
            return result;
        }
        public async Task<IEnumerable<KhachHangDto>> TimKiemKhachHangAsync(string keyword)
        {
            var query = _context.KhachHangs
                .AsNoTracking()
                .Where(k => k.TrangThai == 1 && k.TenKhachHang != "Khách lẻ");

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                // Logic tìm kiếm khi có từ khóa (giữ nguyên)
                var lowerKeyword = keyword.ToLower().Trim();
                query = query.Where(k =>
                    k.TenKhachHang.ToLower().Contains(lowerKeyword) ||
                    k.SDT.Contains(lowerKeyword)
                );
            }

            // Luôn sắp xếp và giới hạn số lượng kết quả
            return await query
                .OrderByDescending(k => k.NgayTaoTaiKhoan) // Sắp xếp theo khách hàng mới nhất
                .Take(10) // Chỉ lấy 10 kết quả để danh sách không quá dài
                .ProjectTo<KhachHangDto>(_mapper.ConfigurationProvider)
                .ToListAsync();
        }

        public async Task<IEnumerable<VoucherDto>> TimKiemVoucherHopLeAsync(Guid hoaDonId)
        {
            var now = DateTime.UtcNow;

            // Lấy thông tin hóa đơn để kiểm tra các điều kiện (nếu cần trong tương lai)
            // Ví dụ: var hoaDon = await _context.HoaDons.FindAsync(hoaDonId);

            // Lọc các voucher hợp lệ dựa trên các điều kiện chung
            var validVouchers = await _context.Vouchers
                .AsNoTracking()
                .Where(v =>
                    v.TrangThai == 1 &&       // Phải đang hoạt động
                    v.SoLuong > 0 &&          // Phải còn lượt sử dụng
                    v.NgayBatDau <= now &&    // Phải trong thời gian hiệu lực
                    v.NgayKetThuc >= now
                )
                .OrderBy(v => v.NgayKetThuc) // Ưu tiên các voucher sắp hết hạn
                .ToListAsync(); // Lấy ra danh sách để xử lý logic phức tạp hơn nếu cần

            // Chuyển đổi sang DTO
            // Ở đây chúng ta có thể thêm các logic kiểm tra điều kiện phức tạp hơn
            // Ví dụ: kiểm tra hóa đơn tối thiểu, khách hàng áp dụng...
            // Nhưng với cấu trúc hiện tại, chúng ta sẽ map trực tiếp.

            var voucherDtos = _mapper.Map<IEnumerable<VoucherDto>>(validVouchers);

            return voucherDtos;
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

               // <<< THÊM MỚI: DÒNG QUAN TRỌNG NHẤT ĐỂ LẤY DỮ LIỆU ẢNH >>>
               .Include(h => h.HoaDonChiTiets)
                   .ThenInclude(hct => hct.SanPhamChiTiet)
                   .ThenInclude(spct => spct.Anh)
               // <<< KẾT THÚC THÊM MỚI >>>

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
        public async Task<IEnumerable<SanPhamBanHangDto>> GetSuggestedProductsAsync(int count)
        {
            return await _context.SanPhamChiTiets
                .AsNoTracking()
                .Where(spct => spct.TrangThai == 1 && spct.SoLuong > 0) // Chỉ lấy sản phẩm đang bán, còn hàng
                .OrderByDescending(spct => spct.NgayTao) // Lấy sản phẩm mới nhất
                .Take(count) // Giới hạn số lượng sản phẩm
                .ProjectTo<SanPhamBanHangDto>(_mapper.ConfigurationProvider) // Dùng ProjectTo để tối ưu
                .ToListAsync();
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