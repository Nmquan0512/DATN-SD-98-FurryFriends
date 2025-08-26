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
using FurryFriends.API.Models;

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
            // ✅ Kiểm tra và sửa dữ liệu hóa đơn bị lỗi trước khi lấy danh sách
            await FixInvoiceDataAsync();
            
            // 1. Tải các đối tượng HoaDon và các dữ liệu liên quan cần thiết
            var hoaDons = await _context.HoaDons
                .AsNoTracking() // Dùng AsNoTracking để tăng hiệu năng cho truy vấn chỉ đọc
                .Include(h => h.KhachHang)
                .Include(h => h.Voucher)
                .Include(h => h.HinhThucThanhToan)
                .OrderByDescending(h => h.NgayTao)
                .ToListAsync();

            // 2. Dùng _mapper.Map() để ánh xạ trong bộ nhớ. 
            //    AutoMapper sẽ tự động map các trường TongTien, ThanhTien, TienGiam
            var dtos = _mapper.Map<IEnumerable<HoaDonBanHangDto>>(hoaDons);
            
            // ✅ Map thủ công DiaChiGiaoHangLucMua để đảm bảo dữ liệu đúng
            var hoaDonList = hoaDons.ToList();
            var dtoList = dtos.ToList();
            
            for (int i = 0; i < hoaDonList.Count; i++)
            {
                dtoList[i].DiaChiGiaoHangLucMua = hoaDonList[i].DiaChiGiaoHangLucMua;
            }
            
            return dtoList;
        }


        public async Task<HoaDonBanHangDto> GetHoaDonByIdAsync(Guid id)
        {
            // ✅ Sử dụng tracking để có thể cập nhật dữ liệu nếu cần
            var hoaDon = await GetFullHoaDonQueryWithTracking().FirstOrDefaultAsync(h => h.HoaDonId == id);
            if (hoaDon == null) throw new KeyNotFoundException("Hóa đơn không tồn tại.");

            // ✅ Kiểm tra và sửa dữ liệu nếu cần
            if (hoaDon.TongTien == 0 && hoaDon.TongTienSauKhiGiam == 0 && hoaDon.HoaDonChiTiets.Any())
            {
                _logger.LogInformation("Phát hiện hóa đơn {HoaDonId} có tổng tiền = 0 nhưng có sản phẩm, đang tính toán lại...", hoaDon.HoaDonId);
                await TinhToanLaiTienHoaDon(hoaDon);
                await _context.SaveChangesAsync(); // Lưu thay đổi vào database
            }

            // 1. Dùng AutoMapper để map các thuộc tính cơ bản
            var dto = _mapper.Map<HoaDonBanHangDto>(hoaDon);

            // 2. Map chi tiết hóa đơn
            dto.ChiTietHoaDon = hoaDon.HoaDonChiTiets.Select(hct => {
                var giaLucMua = hct.GiaLucMua ?? 0m;
                var thanhTien = giaLucMua * hct.SoLuongSanPham;
                
                // Debug logging
                _logger.LogInformation("Debug - SanPham: {TenSanPham}, SoLuong: {SoLuong}, GiaLucMua: {GiaLucMua}, ThanhTien: {ThanhTien}", 
                    hct.SanPhamChiTiet.SanPham.TenSanPham, hct.SoLuongSanPham, giaLucMua, thanhTien);
                
                return new ChiTietHoaDonDto
            {
                SanPhamChiTietId = hct.SanPhamChiTietId,
                TenSanPham = hct.SanPhamChiTiet.SanPham.TenSanPham,
                MauSac = hct.SanPhamChiTiet.MauSac.TenMau,
                KichCo = hct.SanPhamChiTiet.KichCo.TenKichCo,
                    Gia = hct.SanPhamChiTiet.Gia, // Giá gốc từ sản phẩm để hiển thị (không nullable)
                    GiaBan = giaLucMua,       // Giá bán thực tế tại thời điểm mua (sửa từ hct.Gia thành hct.GiaLucMua)
                SoLuong = hct.SoLuongSanPham,
                    ThanhTien = thanhTien, // Thành tiền của dòng này (sửa từ hct.Gia thành hct.GiaLucMua)
                    HinhAnh = hct.SanPhamChiTiet.Anh?.DuongDan
                };
            }).ToList();

            // 3. Lấy trực tiếp các giá trị đã được tính toán và lưu trong DB
            dto.TongTien = hoaDon.TongTien;
            dto.ThanhTien = hoaDon.TongTienSauKhiGiam;
            dto.TienGiam = hoaDon.TongTien - hoaDon.TongTienSauKhiGiam;
            
            // ✅ Map thủ công DiaChiGiaoHangLucMua để đảm bảo dữ liệu đúng
            dto.DiaChiGiaoHangLucMua = hoaDon.DiaChiGiaoHangLucMua;
            
            // ✅ Thêm log để debug dữ liệu
            _logger.LogInformation("Hóa đơn {HoaDonId}: TongTien={TongTien}, TongTienSauKhiGiam={TongTienSauKhiGiam}, ThanhTien={ThanhTien}, DiaChiGiaoHangLucMua={DiaChiGiaoHangLucMua}", 
                hoaDon.HoaDonId, dto.TongTien, dto.ThanhTien, dto.TienGiam, dto.DiaChiGiaoHangLucMua);

            return dto;
        }
        // Bạn có thể xóa phương thức MapToHoaDonDto đi
        // File: BanHangRepository.cs (API)

        public async Task<HoaDonBanHangDto> TaoHoaDonAsync(TaoHoaDonRequest request)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // ✅ Loại bỏ logic cleanup trùng lặp - đã có InvoiceCleanupService xử lý
                // Chỉ giữ lại logic dọn dẹp hóa đơn rất cũ (sau 2 giờ) để tránh đầy database
                var veryOldInvoices = await _context.HoaDons
                    .Where(h => h.TrangThai == (int)TrangThaiHoaDon.Offline_ChuaThanhToan && 
                               h.NgayTao < DateTime.Now.AddHours(-2))
                    .ToListAsync();

                if (veryOldInvoices.Any())
                {
                    _logger.LogInformation($"Dọn dẹp {veryOldInvoices.Count} hóa đơn rất cũ (sau 2 giờ).");
                    // Với mỗi hóa đơn rất cũ, hoàn trả lại số lượng sản phẩm
                    foreach (var oldInvoice in veryOldInvoices)
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
                        // Hoàn trả voucher nếu có
                        if (oldInvoice.VoucherId.HasValue)
                        {
                            var voucher = await _context.Vouchers.FindAsync(oldInvoice.VoucherId.Value);
                            if (voucher != null)
                            {
                                voucher.SoLuong++;
                            }
                        }
                    }
                    _context.HoaDons.RemoveRange(veryOldInvoices);
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
                    NgayTao = DateTime.Now, // ✅ Sử dụng giờ Việt Nam thay vì UTC
                    TrangThai = (int)TrangThaiHoaDon.Offline_ChuaThanhToan, // ✅ Sử dụng trạng thái offline mới
                    GhiChu = request.GhiChu ?? "",
                    NhanVienId = request.NhanVienId,
                    HinhThucThanhToanId = defaultHttt.HinhThucThanhToanId,
                    TongTien = 0,
                    TongTienSauKhiGiam = 0,

                    LoaiHoaDon = request.GiaoHang ? "GiaoHang" : "BanTaiQuay" // ✅ Cập nhật loại hóa đơn theo yêu cầu giao hàng

                };

                if (!request.LaKhachLe && request.KhachHangId.HasValue)
                {
                    await GanKhachHangNoSave(hoaDon, request.KhachHangId);
                }
                else
                {
                    var khachLe = await _context.KhachHangs.FirstOrDefaultAsync(k => k.TenKhachHang == "Khách lẻ");
                    if (khachLe == null)
                    {
                        khachLe = new KhachHang
                        {
                            KhachHangId = Guid.NewGuid(),
                            TenKhachHang = "Khách lẻ",
                            NgayTaoTaiKhoan = DateTime.Now, // ✅ Sử dụng giờ Việt Nam
                            TrangThai = 1,
                            EmailCuaKhachHang = "khachle@furryfriends.local",
                            SDT = "0000000000"
                        };
                        await _context.KhachHangs.AddAsync(khachLe);
                        await _context.SaveChangesAsync(); // Lưu khách hàng trước
                    }
                    
                    hoaDon.KhachHangId = khachLe.KhachHangId;
                    
                    // ✅ Cập nhật thông tin khách hàng từ form địa chỉ giao hàng nếu có
                    if (request.GiaoHang && request.DiaChiMoi != null)
                    {
                        hoaDon.TenCuaKhachHang = request.DiaChiMoi.TenNguoiNhan ?? khachLe.TenKhachHang ?? "";
                        hoaDon.SdtCuaKhachHang = request.DiaChiMoi.SoDienThoai ?? khachLe.SDT ?? "";
                        hoaDon.EmailCuaKhachHang = khachLe.EmailCuaKhachHang ?? ""; // Giữ email mặc định
                    }
                    else
                    {
                        hoaDon.TenCuaKhachHang = khachLe.TenKhachHang ?? "";
                        hoaDon.SdtCuaKhachHang = khachLe.SDT ?? "";
                        hoaDon.EmailCuaKhachHang = khachLe.EmailCuaKhachHang ?? "";
                    }
                }

                // ✅ Xử lý thông tin địa chỉ giao hàng nếu có
                if (request.GiaoHang && request.DiaChiMoi != null)
                {
                    // Lưu snapshot địa chỉ giao hàng lúc mua (chỉ lưu địa chỉ, không ghi đè thông tin khách hàng)
                    hoaDon.DiaChiGiaoHangLucMua = $"{request.DiaChiMoi.TenDiaChi}, {request.DiaChiMoi.PhuongXa}, {request.DiaChiMoi.ThanhPho}";
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

                hoaDon.TrangThai = (int)TrangThaiHoaDon.Offline_DaHuy; // ✅ Sử dụng trạng thái offline mới
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
                _logger.LogInformation("Bắt đầu thêm sản phẩm {SanPhamChiTietId} vào hóa đơn {HoaDonId}", request.SanPhamChiTietId, request.HoaDonId);
                
                var hoaDon = await GetEditableHoaDon(request.HoaDonId);
                var sanPhamChiTiet = await _context.SanPhamChiTiets
                    .Include(spct => spct.SanPham)
                        .ThenInclude(sp => sp.ThuongHieu)
                    .Include(spct => spct.MauSac)
                    .Include(spct => spct.KichCo)
                    .Include(spct => spct.Anh)
                    .Include(spct => spct.DotGiamGiaSanPhams)
                        .ThenInclude(dggsp => dggsp.GiamGia)
                    .FirstOrDefaultAsync(spct => spct.SanPhamChiTietId == request.SanPhamChiTietId);

                if (sanPhamChiTiet == null) 
                {
                    _logger.LogWarning("Sản phẩm {SanPhamChiTietId} không tồn tại", request.SanPhamChiTietId);
                    throw new KeyNotFoundException("Sản phẩm không tồn tại.");
                }

                _logger.LogInformation("Tìm thấy sản phẩm: {TenSanPham}, Giá: {Gia}, Tồn kho: {SoLuong}", 
                    sanPhamChiTiet.SanPham?.TenSanPham, sanPhamChiTiet.Gia, sanPhamChiTiet.SoLuong);

                // <<< LOGIC MỚI: BÁN HÀNG OFFLINE KHÔNG ÁP DỤNG GIẢM GIÁ - KHUYẾN KHÍCH MUA ONLINE >>>
                // ✅ Bán hàng offline luôn sử dụng giá gốc, không áp dụng giảm giá
                decimal actualSalePrice = sanPhamChiTiet.Gia; // Luôn sử dụng giá gốc
                _logger.LogInformation("Bán hàng offline - Sử dụng giá gốc: {GiaGoc}", actualSalePrice);
                // <<< KẾT THÚC LOGIC MỚI >>>

                if (sanPhamChiTiet.SoLuong < request.SoLuong) 
                {
                    _logger.LogWarning("Số lượng sản phẩm trong kho không đủ. Yêu cầu: {RequestSoLuong}, Tồn kho: {SoLuong}", 
                        request.SoLuong, sanPhamChiTiet.SoLuong);
                    throw new InvalidOperationException("Số lượng sản phẩm trong kho không đủ.");
                }

                var existingItem = hoaDon.HoaDonChiTiets.FirstOrDefault(hct => hct.SanPhamChiTietId == request.SanPhamChiTietId);
                if (existingItem != null)
                {
                    existingItem.SoLuongSanPham += request.SoLuong;
                    // Cập nhật giá để đảm bảo tính toán chính xác
                    existingItem.Gia = actualSalePrice;
                    existingItem.GiaLucMua = actualSalePrice;
                    _logger.LogInformation("Cập nhật số lượng sản phẩm hiện có. Số lượng mới: {SoLuongMoi}, Giá mới: {GiaMoi}", existingItem.SoLuongSanPham, actualSalePrice);
                }
                else
                {
                    var newItem = new HoaDonChiTiet
                    {
                        HoaDonChiTietId = Guid.NewGuid(),
                        HoaDonId = hoaDon.HoaDonId,
                        SanPhamChiTietId = sanPhamChiTiet.SanPhamChiTietId,
                        SoLuongSanPham = request.SoLuong,
                        Gia = actualSalePrice,
                        GiaLucMua = actualSalePrice,
                        TenSanPhamLucMua = sanPhamChiTiet.SanPham?.TenSanPham ?? "",
                        MoTaSanPhamLucMua = sanPhamChiTiet.MoTa ?? "",
                        ThuongHieuLucMua = sanPhamChiTiet.SanPham?.ThuongHieu?.TenThuongHieu ?? "",
                        KichCoLucMua = sanPhamChiTiet.KichCo?.TenKichCo ?? "",
                        MauSacLucMua = sanPhamChiTiet.MauSac?.TenMau ?? "",
                        AnhSanPhamLucMua = sanPhamChiTiet.Anh?.DuongDan ?? "",
                        ChatLieuLucMua = "", // Có thể để trống hoặc lấy từ SanPhamChatLieus
                        ThanhPhanLucMua = "" // Có thể để trống hoặc lấy từ SanPhamThanhPhans
                    };
                    await _context.HoaDonChiTiets.AddAsync(newItem);
                    _logger.LogInformation("Thêm sản phẩm mới vào hóa đơn. Số lượng: {SoLuong}, Giá: {Gia}", 
                        newItem.SoLuongSanPham, newItem.Gia);
                }

                sanPhamChiTiet.SoLuong -= request.SoLuong;
                _logger.LogInformation("Cập nhật tồn kho sản phẩm. Tồn kho mới: {SoLuongMoi}", sanPhamChiTiet.SoLuong);
                
                await TinhToanLaiTienHoaDon(hoaDon);
                await _context.SaveChangesAsync(); // Lưu thay đổi trước khi tính toán
                await transaction.CommitAsync();

                _logger.LogInformation("Thêm sản phẩm thành công vào hóa đơn {HoaDonId}", hoaDon.HoaDonId);
                return await GetHoaDonByIdAsync(hoaDon.HoaDonId);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Lỗi khi thêm sản phẩm {SanPhamChiTietId} vào hóa đơn {HoaDonId}", 
                    request.SanPhamChiTietId, request.HoaDonId);
                throw;
            }
        }
        public async Task<HoaDonBanHangDto> XoaSanPhamKhoiHoaDonAsync(Guid hoaDonId, Guid sanPhamChiTietId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                _logger.LogInformation("Bắt đầu xóa sản phẩm {SanPhamId} khỏi hóa đơn {HoaDonId}", sanPhamChiTietId, hoaDonId);
                
                var hoaDon = await GetEditableHoaDon(hoaDonId);
                var itemToDelete = hoaDon.HoaDonChiTiets.FirstOrDefault(hct => hct.SanPhamChiTietId == sanPhamChiTietId);
                
                if (itemToDelete == null) 
                {
                    _logger.LogWarning("Sản phẩm {SanPhamId} không có trong hóa đơn {HoaDonId}", sanPhamChiTietId, hoaDonId);
                    throw new KeyNotFoundException("Sản phẩm không có trong hóa đơn.");
                }

                var sanPhamChiTiet = await _context.SanPhamChiTiets.FindAsync(sanPhamChiTietId);
                if (sanPhamChiTiet == null) 
                {
                    _logger.LogWarning("Sản phẩm {SanPhamId} không tồn tại", sanPhamChiTietId);
                    throw new KeyNotFoundException("Sản phẩm không tồn tại.");
                }

                // Hoàn trả số lượng sản phẩm về kho
                int soLuongHoanTra = itemToDelete.SoLuongSanPham;
                sanPhamChiTiet.SoLuong += soLuongHoanTra;
                _logger.LogInformation("Hoàn trả {SoLuong} sản phẩm về kho", soLuongHoanTra);

                // Xóa item khỏi hóa đơn
                hoaDon.HoaDonChiTiets.Remove(itemToDelete);
                _context.HoaDonChiTiets.Remove(itemToDelete);
                
                _logger.LogInformation("Đã xóa sản phẩm khỏi hóa đơn. Số lượng items còn lại: {Count}", hoaDon.HoaDonChiTiets.Count);

                // Tính toán lại tổng tiền hóa đơn
                await TinhToanLaiTienHoaDon(hoaDon);
                await _context.SaveChangesAsync();
                
                await transaction.CommitAsync();

                // Clear EF cache và query lại để đảm bảo dữ liệu mới nhất
                _context.ChangeTracker.Clear();
                return await GetHoaDonByIdAsync(hoaDonId);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Lỗi khi xóa sản phẩm khỏi hóa đơn.");
                throw;
            }
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
                    // Cập nhật lại giá để đảm bảo tính toán đúng
                    itemToUpdate.Gia = sanPhamChiTiet.Gia;
                    itemToUpdate.GiaLucMua = sanPhamChiTiet.Gia;
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

        public async Task<HoaDonBanHangDto> GanKhachHangAsync(Guid hoaDonId, Guid? khachHangId)
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
                if (hoaDon.TrangThai != (int)TrangThaiHoaDon.Offline_ChuaThanhToan)
                    throw new InvalidOperationException("Hóa đơn đã được xử lý (thanh toán/hủy).");
                if (!hoaDon.HoaDonChiTiets.Any())
                    throw new InvalidOperationException("Không thể thanh toán hóa đơn rỗng.");

                // ✅ Validation: Kiểm tra thông tin giao hàng nếu đã tick giao hàng
                if (hoaDon.LoaiHoaDon == "GiaoHang")
                {
                    if (string.IsNullOrWhiteSpace(hoaDon.DiaChiGiaoHangLucMua))
                    {
                        throw new InvalidOperationException("Đơn hàng yêu cầu giao hàng phải có địa chỉ giao hàng. Vui lòng cập nhật thông tin giao hàng trước khi thanh toán.");
                    }
                    
                    if (string.IsNullOrWhiteSpace(hoaDon.TenCuaKhachHang) || string.IsNullOrWhiteSpace(hoaDon.SdtCuaKhachHang))
                    {
                        throw new InvalidOperationException("Đơn hàng yêu cầu giao hàng phải có thông tin người nhận hàng. Vui lòng cập nhật thông tin giao hàng trước khi thanh toán.");
                    }
                }

                var hinhThucTT = await _context.HinhThucThanhToans.FindAsync(request.HinhThucThanhToanId);
                if (hinhThucTT == null) throw new KeyNotFoundException("Hình thức thanh toán không tồn tại.");

                await TinhToanLaiTienHoaDon(hoaDon); // Tính lại tiền lần cuối cho chắc

                if (hinhThucTT.TenHinhThuc.Contains("Tiền mặt") && request.TienKhachDua < hoaDon.TongTienSauKhiGiam)
                    throw new InvalidOperationException("Số tiền khách đưa không đủ.");

                hoaDon.HinhThucThanhToanId = hinhThucTT.HinhThucThanhToanId;
                
                // ✅ Logic trạng thái mới cho bán hàng offline:
                // - Không giao hàng: Offline_ChuaThanhToan (6) → Offline_DaThanhToan (7) - Hoàn thành
                // - Có giao hàng: Offline_ChuaThanhToan (6) → DaThanhToan (1) - Đã thanh toán, chờ giao
                if (hoaDon.LoaiHoaDon == "GiaoHang")
                {
                    hoaDon.TrangThai = (int)TrangThaiHoaDon.DaThanhToan; // ✅ Chuyển về trạng thái 1 - Đã thanh toán
                    hoaDon.LoaiHoaDon = "BanTaiQuay"; // ✅ Đổi thành BanTaiQuay vì đây là bán tại quầy
                }
                else
                {
                    hoaDon.TrangThai = (int)TrangThaiHoaDon.Offline_DaThanhToan; // ✅ Chuyển về trạng thái 7
                }
                
                hoaDon.NgayNhanHang = DateTime.Now; // ✅ Sử dụng giờ Việt Nam - ngày thanh toán là ngày nhận tại quầy
                hoaDon.GhiChu = string.IsNullOrEmpty(hoaDon.GhiChu) ? request.GhiChuThanhToan : hoaDon.GhiChu + " | " + request.GhiChuThanhToan;
                
                // Cập nhật điểm tích lũy cho khách hàng thành viên
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
            var now = DateTime.Now; // ✅ Sử dụng giờ Việt Nam
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
                // ✅ Bán hàng offline không áp dụng giảm giá - khuyến khích mua online
                decimal actualSalePrice = spct.Gia; // Luôn sử dụng giá gốc

                return new SanPhamBanHangDto
                {
                    SanPhamChiTietId = spct.SanPhamChiTietId,
                    TenSanPham = spct.SanPham.TenSanPham,
                    TenMauSac = spct.MauSac.TenMau,
                    TenKichCo = spct.KichCo.TenKichCo,
                    Gia = spct.Gia, // << Gán giá gốc
                    GiaBan = actualSalePrice, // << Gán giá bán thực tế (luôn bằng giá gốc)
                    SoLuongTon = spct.SoLuong,
                    HinhAnh = spct.Anh?.DuongDan
                };
            }).ToList();
            return result;
        }
        public async Task<IEnumerable<KhachHangDto>> TimKiemKhachHangAsync(string? keyword)
        {
            try
            {
                _logger.LogInformation("Bắt đầu tìm kiếm khách hàng với từ khóa: '{Keyword}'", keyword ?? "null");
                
            var query = _context.KhachHangs
                .AsNoTracking()
                .Where(k => k.TrangThai == 1 && k.TenKhachHang != "Khách lẻ");

                _logger.LogInformation("Query cơ bản: {Count} khách hàng", await query.CountAsync());

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                // Logic tìm kiếm khi có từ khóa (giữ nguyên)
                var lowerKeyword = keyword.ToLower().Trim();
                query = query.Where(k =>
                    k.TenKhachHang.ToLower().Contains(lowerKeyword) ||
                        (k.SDT != null && k.SDT.Contains(lowerKeyword))
                );
                    _logger.LogInformation("Sau khi filter với keyword '{Keyword}': {Count} khách hàng", keyword, await query.CountAsync());
            }

                // Lấy dữ liệu trước, sau đó map
                var khachHangs = await query
                .OrderByDescending(k => k.NgayTaoTaiKhoan) // Sắp xếp theo khách hàng mới nhất
                .Take(10) // Chỉ lấy 10 kết quả để danh sách không quá dài
                .ToListAsync();

                _logger.LogInformation("Lấy được {Count} khách hàng từ database", khachHangs.Count);

                // Map thủ công để tránh lỗi AutoMapper
                var result = khachHangs.Select(k => new KhachHangDto
                {
                    KhachHangId = k.KhachHangId,
                    TenKhachHang = k.TenKhachHang,
                    SDT = k.SDT,
                    Email = k.EmailCuaKhachHang,
                    DiemTichLuy = k.DiemKhachHang ?? 0,
                    LaKhachLe = k.TenKhachHang == "Khách lẻ"
                }).ToList();

                _logger.LogInformation("Map thành công {Count} khách hàng", result.Count);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tìm kiếm khách hàng với từ khóa: '{Keyword}'", keyword ?? "null");
                throw new InvalidOperationException("Lỗi khi tìm kiếm khách hàng. Vui lòng thử lại.");
            }
        }

        public async Task<IEnumerable<VoucherDto>> TimKiemVoucherHopLeAsync(Guid hoaDonId)
        {
            var now = DateTime.Now; // ✅ Sử dụng giờ Việt Nam

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

        // ✅ Sửa dữ liệu hóa đơn bị lỗi (tổng tiền = 0)
        public async Task FixInvoiceDataAsync()
        {
            try
            {
                var invoicesToFix = await _context.HoaDons
                    .Include(h => h.HoaDonChiTiets)
                    .Where(h => h.TongTien == 0 && h.TongTienSauKhiGiam == 0 && h.HoaDonChiTiets.Any())
                    .ToListAsync();

                if (invoicesToFix.Any())
                {
                    _logger.LogInformation($"Tìm thấy {invoicesToFix.Count} hóa đơn cần sửa dữ liệu");
                    
                    foreach (var invoice in invoicesToFix)
                    {
                        _logger.LogInformation("Sửa hóa đơn {HoaDonId} với {Count} sản phẩm", 
                            invoice.HoaDonId, invoice.HoaDonChiTiets.Count);
                        
                        await TinhToanLaiTienHoaDon(invoice);
                    }
                    
                    await _context.SaveChangesAsync();
                    _logger.LogInformation($"Đã sửa {invoicesToFix.Count} hóa đơn thành công");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi sửa dữ liệu hóa đơn");
            }
        }

        // ✅ Loại bỏ AutoCancelOldInvoices - đã có InvoiceCleanupService xử lý
        // Logic cleanup hóa đơn sau 30 phút được xử lý bởi InvoiceCleanupService

        private async Task<HoaDon> GetEditableHoaDon(Guid hoaDonId)
        {
            var hoaDon = await _context.HoaDons
                .Include(h => h.HoaDonChiTiets)
                .FirstOrDefaultAsync(h => h.HoaDonId == hoaDonId);

            if (hoaDon == null) throw new KeyNotFoundException("Hóa đơn không tồn tại.");
            
            // ✅ Thêm log để debug trạng thái hóa đơn
            _logger.LogInformation("Hóa đơn {HoaDonId} có trạng thái: {TrangThai}", hoaDonId, hoaDon.TrangThai);
            
            // ✅ Kiểm tra trạng thái để cho phép chỉnh sửa:
            // - Online: ChuaThanhToan (0)
            // - Offline: Offline_ChuaThanhToan (6) hoặc Offline_DaThanhToan (7) - cho phép chỉnh sửa
            if (hoaDon.TrangThai != (int)TrangThaiHoaDon.ChuaThanhToan && 
                hoaDon.TrangThai != (int)TrangThaiHoaDon.Offline_ChuaThanhToan &&
                hoaDon.TrangThai != (int)TrangThaiHoaDon.Offline_DaThanhToan)
            {
                _logger.LogWarning("Hóa đơn {HoaDonId} không thể chỉnh sửa vì trạng thái: {TrangThai}", hoaDonId, hoaDon.TrangThai);
                throw new InvalidOperationException("Không thể chỉnh sửa hóa đơn đã giao hàng hoặc đã hủy.");
            }

            return hoaDon;
        }

        private async Task TinhToanLaiTienHoaDon(HoaDon hoaDon)
        {
            try
            {
                _logger.LogInformation("Bắt đầu tính toán lại tiền hóa đơn {HoaDonId}", hoaDon.HoaDonId);
                
                // Log chi tiết từng item
                foreach (var item in hoaDon.HoaDonChiTiets)
                {
                    _logger.LogInformation("Item: {SanPhamId}, SoLuong: {SoLuong}, GiaLucMua: {GiaLucMua}, Gia: {Gia}", 
                        item.SanPhamChiTietId, item.SoLuongSanPham, item.GiaLucMua, item.Gia);
                }
                
                hoaDon.TongTien = hoaDon.HoaDonChiTiets.Sum(hct => hct.SoLuongSanPham * (hct.GiaLucMua ?? 0m));
                _logger.LogInformation("Tổng tiền tính được: {TongTien}", hoaDon.TongTien);
                
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
                        _logger.LogInformation("Tiền giảm voucher: {TienGiam}", tienGiam);
                    }
                }
                
                hoaDon.TongTienSauKhiGiam = hoaDon.TongTien - tienGiam;
                
                // ✅ Cộng phí ship cho đơn giao hàng trong BanHang
                if (hoaDon.LoaiHoaDon == "GiaoHang")
                {
                    // Logic freeship: Đơn hàng trên 500k được freeship
                    var shippingFee = hoaDon.TongTienSauKhiGiam >= 500000m ? 0m : 30000m;
                    hoaDon.TongTienSauKhiGiam += shippingFee;
                    _logger.LogInformation("Đơn giao hàng - Phí ship: {ShippingFee}, Tổng tiền cuối: {TongTienCuoi}", shippingFee, hoaDon.TongTienSauKhiGiam);
                }
                
                _logger.LogInformation("Tổng tiền sau khi giảm: {TongTienSauKhiGiam}", hoaDon.TongTienSauKhiGiam);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tính toán tiền hóa đơn {HoaDonId}", hoaDon.HoaDonId);
                throw;
            }
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

        private async Task GanKhachHangNoSave(HoaDon hoaDon, Guid? khachHangId)
        {
            if (khachHangId.HasValue)
            {
                var khachHang = await _context.KhachHangs.FindAsync(khachHangId.Value);
                if (khachHang == null) throw new KeyNotFoundException("Khách hàng không tồn tại.");
                
                // ✅ Lưu thông tin vào snapshot của hóa đơn
                hoaDon.KhachHangId = khachHang.KhachHangId;
                hoaDon.TenCuaKhachHang = khachHang.TenKhachHang;
                hoaDon.SdtCuaKhachHang = khachHang.SDT;
                hoaDon.EmailCuaKhachHang = khachHang.EmailCuaKhachHang;
                
                // ✅ Log để debug
                _logger.LogInformation("Gán khách hàng vào hóa đơn: KhachHangId={KhachHangId}, Ten={Ten}, SDT={SDT}, Email={Email}", 
                    khachHang.KhachHangId, khachHang.TenKhachHang, khachHang.SDT, khachHang.EmailCuaKhachHang);
            }
            else
            {
                var khachLe = await _context.KhachHangs.FirstOrDefaultAsync(k => k.TenKhachHang == "Khách lẻ");
                if (khachLe == null)
                {
                    khachLe = new KhachHang
                    {
                        KhachHangId = Guid.NewGuid(),
                        TenKhachHang = "Khách lẻ",
                        NgayTaoTaiKhoan = DateTime.Now, // ✅ Sử dụng giờ Việt Nam
                        TrangThai = 1,
                        EmailCuaKhachHang = "khachle@furryfriends.local",
                        SDT = "0000000000"
                    };
                    await _context.KhachHangs.AddAsync(khachLe);
                    await _context.SaveChangesAsync(); // Lưu khách hàng trước
                }
                hoaDon.KhachHangId = khachLe.KhachHangId;
                hoaDon.TenCuaKhachHang = khachLe.TenKhachHang ?? "";
                hoaDon.SdtCuaKhachHang = khachLe.SDT ?? "";
                hoaDon.EmailCuaKhachHang = khachLe.EmailCuaKhachHang ?? "";
                
                // ✅ Log để debug
                _logger.LogInformation("Gán khách lẻ vào hóa đơn: KhachHangId={KhachHangId}, Ten={Ten}, SDT={SDT}, Email={Email}", 
                    khachLe.KhachHangId, khachLe.TenKhachHang, khachLe.SDT, khachLe.EmailCuaKhachHang);
            }
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

        // ✅ Phương thức mới với tracking để có thể cập nhật dữ liệu
        private IQueryable<HoaDon> GetFullHoaDonQueryWithTracking()
        {
            return _context.HoaDons
               .Include(h => h.HoaDonChiTiets)
                   .ThenInclude(hct => hct.SanPhamChiTiet)
                       .ThenInclude(spct => spct.SanPham)
               .Include(h => h.HoaDonChiTiets)
                   .ThenInclude(hct => hct.SanPhamChiTiet)
                   .ThenInclude(spct => spct.MauSac)
               .Include(h => h.HoaDonChiTiets)
                   .ThenInclude(hct => hct.SanPhamChiTiet)
                   .ThenInclude(spct => spct.KichCo)
               .Include(h => h.HoaDonChiTiets)
                   .ThenInclude(hct => hct.SanPhamChiTiet)
                   .ThenInclude(spct => spct.Anh)
               .Include(h => h.KhachHang)
               .Include(h => h.HinhThucThanhToan)
               .Include(h => h.Voucher);
        }

        private async Task<HoaDonBanHangDto> MapToHoaDonDto(HoaDon hoaDon)
        {
            var dto = _mapper.Map<HoaDonBanHangDto>(hoaDon);
            
            // ✅ Kiểm tra nếu tổng tiền = 0 nhưng có sản phẩm thì tính toán lại
            if (hoaDon.TongTien == 0 && hoaDon.TongTienSauKhiGiam == 0 && hoaDon.HoaDonChiTiets.Any())
            {
                _logger.LogInformation("Phát hiện hóa đơn {HoaDonId} có tổng tiền = 0 nhưng có sản phẩm, đang tính toán lại...", hoaDon.HoaDonId);
                await TinhToanLaiTienHoaDon(hoaDon);
                await _context.SaveChangesAsync(); // Lưu thay đổi vào database
            }
            
            dto.TongTien = hoaDon.TongTien;
            dto.ThanhTien = hoaDon.TongTienSauKhiGiam;
            dto.TienGiam = dto.TongTien - dto.ThanhTien;
            
            // ✅ Map thủ công DiaChiGiaoHangLucMua để đảm bảo dữ liệu đúng
            dto.DiaChiGiaoHangLucMua = hoaDon.DiaChiGiaoHangLucMua;
            
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

        public async Task<HoaDonBanHangDto> CapNhatDiaChiGiaoHangAsync(Guid hoaDonId, DiaChiMoiDto diaChiMoi)
        {
            var hoaDon = await GetEditableHoaDon(hoaDonId);
            
            // ✅ Cập nhật thông tin khách hàng từ form địa chỉ giao hàng
            hoaDon.TenCuaKhachHang = diaChiMoi.TenNguoiNhan ?? hoaDon.TenCuaKhachHang ?? "";
            hoaDon.SdtCuaKhachHang = diaChiMoi.SoDienThoai ?? hoaDon.SdtCuaKhachHang ?? "";
            // Giữ nguyên email hiện tại
            
            // ✅ Cập nhật snapshot địa chỉ giao hàng lúc mua (chỉ lưu địa chỉ)
            hoaDon.DiaChiGiaoHangLucMua = $"{diaChiMoi.TenDiaChi}, {diaChiMoi.PhuongXa}, {diaChiMoi.ThanhPho}";
            
            // ✅ Cập nhật loại hóa đơn thành GiaoHang
            hoaDon.LoaiHoaDon = "GiaoHang";
            
            // ✅ Không thay đổi trạng thái khi thêm địa chỉ giao hàng
            // Giữ nguyên trạng thái Offline_ChuaThanhToan (6) để có thể thanh toán sau
            
            // ✅ Tính toán lại tổng tiền để cộng phí ship
            await TinhToanLaiTienHoaDon(hoaDon);
            
            await _context.SaveChangesAsync();
            
            return await GetHoaDonByIdAsync(hoaDonId);
        }
        #endregion
    }
}