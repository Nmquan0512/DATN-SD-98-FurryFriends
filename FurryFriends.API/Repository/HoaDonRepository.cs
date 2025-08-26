using FurryFriends.API.Data;
using FurryFriends.API.Models;
using FurryFriends.API.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.Text;

namespace FurryFriends.API.Repository
{
    public class HoaDonRepository : IHoaDonRepository
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public HoaDonRepository(AppDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        public async Task<IEnumerable<HoaDon>> GetHoaDonListAsync()
        {
            try
            {
                var hoaDons = await _context.HoaDons
                    .Include(h => h.HoaDonChiTiets)
                        .ThenInclude(ct => ct.SanPhamChiTiet)
                    .Include(h => h.KhachHang)
                    .Include(h => h.Voucher)
                    .Include(h => h.HinhThucThanhToan)
                    .Include(h => h.DiaChiGiaoHang) // ✅ Include địa chỉ giao hàng
                    .OrderByDescending(h => h.NgayTao) // ✅ Sắp xếp theo thời gian gần nhất
                    .AsNoTracking() // Tối ưu performance
                    .ToListAsync();

                return hoaDons;
            }
            catch (Exception ex)
            {
                // Log error và trả về empty list thay vì throw exception
                Console.WriteLine($"Error in GetHoaDonListAsync: {ex.Message}");
                return new List<HoaDon>();
            }
        }

        // ✅ Method mới cho quản lý đơn hàng - chỉ lấy hóa đơn trạng thái 0-5
        public async Task<IEnumerable<HoaDon>> GetDonHangListAsync()
        {
            try
            {
                var hoaDons = await _context.HoaDons
                    .Include(h => h.HoaDonChiTiets)
                        .ThenInclude(ct => ct.SanPhamChiTiet)
                    .Include(h => h.KhachHang)
                    .Include(h => h.Voucher)
                    .Include(h => h.HinhThucThanhToan)
                    .Include(h => h.DiaChiGiaoHang)
                    .Where(h => h.TrangThai >= 0 && h.TrangThai <= 5) // ✅ Chỉ lấy hóa đơn trạng thái 0-5
                    .OrderByDescending(h => h.NgayTao) // ✅ Sắp xếp theo thời gian gần nhất
                    .AsNoTracking()
                    .ToListAsync();

                return hoaDons;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetDonHangListAsync: {ex.Message}");
                return new List<HoaDon>();
            }
        }


        public async Task<HoaDon> GetHoaDonByIdAsync(Guid hoaDonId)
        {
            if (hoaDonId == Guid.Empty)
            {
                throw new ArgumentException("ID hóa đơn không hợp lệ");
            }

            var hoaDon = await _context.Set<HoaDon>()
                                 .Include(h => h.HinhThucThanhToan)
                                 .Include(h => h.DiaChiGiaoHang)
                                 .Include(h => h.KhachHang)
                                 .Include(h => h.NhanVien)
                                 .Include(h => h.Voucher)
                                 .Include(h => h.HoaDonChiTiets)
                                    .ThenInclude(ct => ct.SanPhamChiTiet)
                                        .ThenInclude(spc => spc.SanPham)
                                 .Include(h => h.HoaDonChiTiets)
                                    .ThenInclude(ct => ct.SanPhamChiTiet)
                                        .ThenInclude(spc => spc.Anh)
                                 .Include(h => h.HoaDonChiTiets)
                                    .ThenInclude(ct => ct.SanPhamChiTiet)
                                        .ThenInclude(spc => spc.MauSac)
                                 .Include(h => h.HoaDonChiTiets)
                                    .ThenInclude(ct => ct.SanPhamChiTiet)
                                        .ThenInclude(spc => spc.KichCo)
                                 .Include(h => h.LichSuTrangThaiHoaDons)
                                    .ThenInclude(l => l.NhanVien)
                                 .FirstOrDefaultAsync(h => h.HoaDonId == hoaDonId);

            if (hoaDon == null)
            {
                throw new KeyNotFoundException($"Không tìm thấy hóa đơn với ID: {hoaDonId}");
            }

            return hoaDon;
        }

        public async Task<IEnumerable<HoaDon>> SearchHoaDonAsync(Func<HoaDon, bool> predicate)
        {
            return await Task.Run(() => _context.Set<HoaDon>()
                                                .Include(h => h.HoaDonChiTiets)
                                                .Where(predicate)
                                                .ToList());
        }

        public async Task<byte[]> ExportHoaDonToPdfAsync(Guid hoaDonId)
        {
            if (hoaDonId == Guid.Empty)
            {
                throw new ArgumentException("ID hóa đơn không hợp lệ");
            }

            var hoaDon = await GetHoaDonByIdAsync(hoaDonId);

            using (var memoryStream = new MemoryStream())
            {
                // Set encoding for Vietnamese characters
                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                var document = new Document(PageSize.A4, 40, 40, 40, 40);
                var writer = PdfWriter.GetInstance(document, memoryStream);
                document.Open();

                // Define enhanced color scheme - Cool tones
                var primaryColor = new BaseColor(70, 130, 180);      // Steel blue
                var accentColor = new BaseColor(100, 149, 237);      // Cornflower blue
                var darkGray = new BaseColor(47, 79, 79);            // Dark slate gray
                var lightGray = new BaseColor(230, 230, 250);        // Lavender
                var whiteColor = BaseColor.WHITE;
                var successColor = new BaseColor(95, 158, 160);      // Cadet blue (cool green-blue)

                // Create fonts with Vietnamese support
                string fontPath = Path.Combine(_environment.ContentRootPath, "Fonts", "arial.ttf");
                BaseFont baseFont;
                if (File.Exists(fontPath))
                {
                    baseFont = BaseFont.CreateFont(fontPath, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
                }
                else
                {
                    baseFont = BaseFont.CreateFont("C:\\Windows\\Fonts\\arial.ttf", BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
                }

                var titleFont = new Font(baseFont, 26, Font.BOLD, whiteColor);
                var companyFont = new Font(baseFont, 16, Font.BOLD, darkGray);
                var subtitleFont = new Font(baseFont, 14, Font.BOLD, primaryColor);
                var headerFont = new Font(baseFont, 12, Font.BOLD, whiteColor);
                var normalFont = new Font(baseFont, 11, Font.NORMAL, darkGray);
                var boldFont = new Font(baseFont, 11, Font.BOLD, darkGray);
                var smallFont = new Font(baseFont, 9, Font.NORMAL, darkGray);
                var totalFont = new Font(baseFont, 14, Font.BOLD, successColor);

                // Add company header - Logo left, Company info right
                var headerTable = new PdfPTable(2);
                headerTable.WidthPercentage = 100;
                headerTable.SetWidths(new float[] { 2, 1 });

                // Left side - Logo only
                var logoCell = new PdfPCell();
                logoCell.Border = Rectangle.NO_BORDER;
                logoCell.PaddingRight = 20f;
                logoCell.VerticalAlignment = Element.ALIGN_MIDDLE;

                try
                {
                    string webProjectPath = Path.GetFullPath(Path.Combine(_environment.ContentRootPath, "..", "FurryFriends.Web"));
                    string logoPath = Path.Combine(webProjectPath, "wwwroot", "images", "hihihi.png");

                    if (File.Exists(logoPath))
                    {
                        var logo = Image.GetInstance(logoPath);
                        logo.ScaleToFit(100f, 100f);
                        logoCell.AddElement(logo);
                    }
                    else
                    {
                        // Fallback: Company name as logo with cool styling
                        var companyName = new Paragraph("FURRY FRIENDS", new Font(baseFont, 20, Font.BOLD, primaryColor));
                        companyName.Alignment = Element.ALIGN_LEFT;
                        logoCell.AddElement(companyName);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Could not load logo: {ex.Message}");
                    var companyName = new Paragraph("FURRY FRIENDS", new Font(baseFont, 20, Font.BOLD, primaryColor));
                    companyName.Alignment = Element.ALIGN_LEFT;
                    logoCell.AddElement(companyName);
                }

                // Right side - Company info block
                var companyInfoCell = new PdfPCell();
                companyInfoCell.Border = Rectangle.NO_BORDER;
                companyInfoCell.VerticalAlignment = Element.ALIGN_TOP;
                companyInfoCell.HorizontalAlignment = Element.ALIGN_RIGHT;

                var companyInfo = new Paragraph();
                companyInfo.Add(new Chunk("FURRY FRIENDS STORE\n", companyFont));
                companyInfo.Add(new Chunk("142 Nguyễn Đổng Chi , Nam Từ Liêm\n", normalFont));
                companyInfo.Add(new Chunk("TP. Hà Nội, Việt Nam\n", normalFont));
                companyInfo.Add(new Chunk("Tel: 0968596808\n", normalFont));
                companyInfo.Add(new Chunk("Email: info@furryfriends.vn", normalFont));
                companyInfo.Alignment = Element.ALIGN_RIGHT;
                companyInfoCell.AddElement(companyInfo);

                headerTable.AddCell(logoCell);
                headerTable.AddCell(companyInfoCell);
                document.Add(headerTable);

                // Add stylized invoice title with background
                var titleTable = new PdfPTable(1);
                titleTable.WidthPercentage = 100;
                titleTable.SpacingBefore = 20f;
                titleTable.SpacingAfter = 30f;

                var titleCell = new PdfPCell(new Phrase("HÓA ĐƠN BÁN HÀNG", titleFont));
                titleCell.BackgroundColor = primaryColor;
                titleCell.HorizontalAlignment = Element.ALIGN_CENTER;
                titleCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                titleCell.Border = Rectangle.NO_BORDER;
                titleCell.PaddingTop = 15f;
                titleCell.PaddingBottom = 15f;
                titleTable.AddCell(titleCell);
                document.Add(titleTable);

                // Invoice information with modern card design
                var infoMainTable = new PdfPTable(2);
                infoMainTable.WidthPercentage = 100;
                infoMainTable.SetWidths(new float[] { 1, 1 });
                infoMainTable.SpacingAfter = 25f;

                // Left side - Invoice details
                var leftInfoTable = new PdfPTable(2);
                leftInfoTable.WidthPercentage = 100;
                leftInfoTable.SetWidths(new float[] { 1, 1.5f });

                var invoiceHeaderCell = new PdfPCell(new Phrase("THÔNG TIN HÓA ĐƠN", subtitleFont));
                invoiceHeaderCell.Colspan = 2;
                invoiceHeaderCell.BackgroundColor = lightGray;
                invoiceHeaderCell.Border = Rectangle.NO_BORDER;
                invoiceHeaderCell.Padding = 8f;
                leftInfoTable.AddCell(invoiceHeaderCell);

                AddModernInfoRow(leftInfoTable, "Số hóa đơn:", hoaDon.HoaDonId.ToString().Substring(0, 8).ToUpper(), normalFont, boldFont);
                AddModernInfoRow(leftInfoTable, "Ngày lập:", hoaDon.NgayTao.ToString("dd/MM/yyyy HH:mm"), normalFont, boldFont);
                AddModernInfoRow(leftInfoTable, "Nhân viên:", "Admin", normalFont, boldFont);
                AddModernInfoRow(leftInfoTable, "Ngày nhận hàng:", hoaDon.NgayNhanHang?.ToString("dd/MM/yyyy HH:mm") ?? "Chưa cập nhật", normalFont, boldFont);
                // Xác định hình thức thanh toán dựa trên LoaiHoaDon
                string hinhThucThanhToan = "Không xác định";
                if (hoaDon.LoaiHoaDon == "BanTaiQuay")
                {
                    hinhThucThanhToan = "Đã thanh toán";
                }
                else
                {
                    hinhThucThanhToan = hoaDon.HinhThucThanhToan?.TenHinhThuc ?? "Không xác định";
                }
                AddModernInfoRow(leftInfoTable, "Hình thức thanh toán:", hinhThucThanhToan, normalFont, boldFont);
                // Hiển thị loại hóa đơn với text thân thiện
                string loaiHoaDonText = hoaDon.LoaiHoaDon switch
                {
                    "BanTaiQuay" => "Bán Tại Quầy",
                    "Online" => "Bán Online",
                    "GiaoHang" => "Giao Hàng",
                    _ => hoaDon.LoaiHoaDon ?? "Không xác định"
                };
                AddModernInfoRow(leftInfoTable, "Loại hóa đơn:", loaiHoaDonText, normalFont, boldFont);
                AddModernInfoRow(leftInfoTable, "Trạng thái:", "Hoàn thành", normalFont, boldFont);
                AddModernInfoRow(leftInfoTable, "Ghi chú:", hoaDon.GhiChu ?? "Hóa đơn Online", normalFont, boldFont);
                if (!string.IsNullOrWhiteSpace(hoaDon.ThongTinVoucherLucMua))
                {
                    AddModernInfoRow(leftInfoTable, "Voucher áp dụng:", hoaDon.ThongTinVoucherLucMua, normalFont, boldFont);
                }
                // Xác định người tạo hóa đơn dựa trên LoaiHoaDon
                string nguoiTaoHoaDon = "Chưa xác định";
                if (hoaDon.LoaiHoaDon == "BanTaiQuay")
                {
                    nguoiTaoHoaDon = hoaDon.NhanVien?.HoVaTen ?? "Chưa xác định";
                }
                else if (hoaDon.LoaiHoaDon == "Online" || hoaDon.LoaiHoaDon == "GiaoHang")
                {
                    nguoiTaoHoaDon = "Hệ thống";
                }
                AddModernInfoRow(leftInfoTable, "Người tạo hóa đơn:", nguoiTaoHoaDon, normalFont, boldFont);

                var leftCell = new PdfPCell(leftInfoTable);
                leftCell.Border = Rectangle.BOX;
                leftCell.BorderColor = lightGray;
                leftCell.BorderWidth = 1f;
                leftCell.Padding = 0f;

                // Right side - Customer details
                var rightInfoTable = new PdfPTable(2);
                rightInfoTable.WidthPercentage = 100;
                rightInfoTable.SetWidths(new float[] { 1, 1.5f });

                var customerHeaderCell = new PdfPCell(new Phrase("THÔNG TIN KHÁCH HÀNG", subtitleFont));
                customerHeaderCell.Colspan = 2;
                customerHeaderCell.BackgroundColor = lightGray;
                customerHeaderCell.Border = Rectangle.NO_BORDER;
                customerHeaderCell.Padding = 8f;
                rightInfoTable.AddCell(customerHeaderCell);

                AddModernInfoRow(rightInfoTable, "Tên khách hàng:", hoaDon.TenCuaKhachHang ?? "Khách lẻ", normalFont, boldFont);
                AddModernInfoRow(rightInfoTable, "Số điện thoại:", hoaDon.SdtCuaKhachHang ?? "N/A", normalFont, boldFont);
                AddModernInfoRow(rightInfoTable, "Email:", hoaDon.EmailCuaKhachHang ?? "N/A", normalFont, boldFont);
                
                // Thêm thông tin địa chỉ giao hàng từ DiaChiGiaoHang
                var diaChiGiaoHang = "Bán tại quầy không giao hàng";
                
                // ✅ Ưu tiên hiển thị snapshot địa chỉ giao hàng lúc mua
                if (!string.IsNullOrWhiteSpace(hoaDon.DiaChiGiaoHangLucMua))
                {
                    diaChiGiaoHang = hoaDon.DiaChiGiaoHangLucMua;
                }
                // Kiểm tra nếu có DiaChiGiaoHang thì hiển thị địa chỉ
                else if (hoaDon.DiaChiGiaoHang != null)
                {
                    var diaChi = hoaDon.DiaChiGiaoHang;
                    var diaChiParts = new List<string>();
                    
                    if (!string.IsNullOrWhiteSpace(diaChi.TenDiaChi))
                        diaChiParts.Add(diaChi.TenDiaChi);
                    
                    if (!string.IsNullOrWhiteSpace(diaChi.PhuongXa))
                        diaChiParts.Add(diaChi.PhuongXa);
                    
                    if (!string.IsNullOrWhiteSpace(diaChi.ThanhPho))
                        diaChiParts.Add(diaChi.ThanhPho);
                    
                    if (diaChiParts.Count > 0)
                    {
                        diaChiGiaoHang = string.Join(", ", diaChiParts);
                    }
                    else
                    {
                        // Nếu có DiaChiGiaoHang nhưng không có thông tin địa chỉ
                        diaChiGiaoHang = "Địa chỉ không đầy đủ";
                    }
                }
                else if (hoaDon.LoaiHoaDon == "BanTaiQuay")
                {
                    // Bán tại quầy và không có địa chỉ giao hàng
                    diaChiGiaoHang = "Không giao hàng";
                }
                else
                {
                    // Online/GiaoHang nhưng không có địa chỉ
                    diaChiGiaoHang = "Chưa cập nhật địa chỉ";
                }
                
                AddModernInfoRow(rightInfoTable, "Địa chỉ:", diaChiGiaoHang, normalFont, boldFont);

                var rightCell = new PdfPCell(rightInfoTable);
                rightCell.Border = Rectangle.BOX;
                rightCell.BorderColor = lightGray;
                rightCell.BorderWidth = 1f;
                rightCell.Padding = 0f;

                infoMainTable.AddCell(leftCell);
                infoMainTable.AddCell(rightCell);
                document.Add(infoMainTable);

                // Product details with enhanced styling
                var detailTable = new PdfPTable(6);
                detailTable.WidthPercentage = 100;
                detailTable.SetWidths(new float[] { 0.8f, 2.5f, 1.5f, 1f, 1.2f, 1.5f });
                detailTable.SpacingAfter = 20f;

                // Table header with gradient-like effect
                AddEnhancedTableHeader(detailTable, "STT", headerFont, accentColor);
                AddEnhancedTableHeader(detailTable, "TÊN SẢN PHẨM", headerFont, accentColor);
                AddEnhancedTableHeader(detailTable, "LOẠI", headerFont, accentColor);
                AddEnhancedTableHeader(detailTable, "SL", headerFont, accentColor);
                AddEnhancedTableHeader(detailTable, "ĐƠN GIÁ", headerFont, accentColor);
                AddEnhancedTableHeader(detailTable, "THÀNH TIỀN", headerFont, accentColor);

                // Add products with alternating row colors
                int stt = 1;
                decimal tongTienHang = 0;

                foreach (var chiTiet in hoaDon.HoaDonChiTiets ?? new List<HoaDonChiTiet>())
                {
                    var rowColor = (stt % 2 == 0) ? lightGray : whiteColor;
                    
                    // ✅ Sử dụng snapshot data thay vì tính toán lại giá hiện tại
                    decimal donGiaHienThi = chiTiet.GiaLucMua ?? chiTiet.Gia;
                    decimal thanhTien = chiTiet.SoLuongSanPham * donGiaHienThi;
                    tongTienHang += thanhTien;

                    AddEnhancedProductRow(detailTable, stt.ToString(), normalFont, rowColor);
                    
                    // ✅ Sử dụng tên sản phẩm lúc mua (snapshot)
                    var tenSp = chiTiet.TenSanPhamLucMua ?? "N/A";
                    AddEnhancedProductRow(detailTable, tenSp, normalFont, rowColor);
                    
                    // ✅ Sử dụng thông tin biến thể lúc mua (snapshot)
                    var mauSac = chiTiet.MauSacLucMua ?? "N/A";
                    var kichCo = chiTiet.KichCoLucMua ?? "N/A";
                    var bienThe = $"{mauSac} - {kichCo}";
                    AddEnhancedProductRow(detailTable, bienThe, normalFont, rowColor, Element.ALIGN_CENTER);
                    
                    AddEnhancedProductRow(detailTable, chiTiet.SoLuongSanPham.ToString(), normalFont, rowColor, Element.ALIGN_CENTER);
                    AddEnhancedProductRow(detailTable, donGiaHienThi.ToString("N0") + "đ", normalFont, rowColor, Element.ALIGN_RIGHT);
                    AddEnhancedProductRow(detailTable, thanhTien.ToString("N0") + "đ", boldFont, rowColor, Element.ALIGN_RIGHT);

                    stt++;
                }
                document.Add(detailTable);

                // Enhanced totals section
                var totalMainTable = new PdfPTable(1);
                totalMainTable.WidthPercentage = 100;
                totalMainTable.HorizontalAlignment = Element.ALIGN_RIGHT;

                var totalSectionTable = new PdfPTable(2);
                totalSectionTable.WidthPercentage = 60;
                totalSectionTable.SetWidths(new float[] { 1.5f, 1f });

                // Totals header
                var totalsHeaderCell = new PdfPCell(new Phrase("TỔNG KẾT THANH TOÁN", subtitleFont));
                totalsHeaderCell.Colspan = 2;
                totalsHeaderCell.BackgroundColor = primaryColor;
                totalsHeaderCell.HorizontalAlignment = Element.ALIGN_CENTER;
                totalsHeaderCell.Border = Rectangle.NO_BORDER;
                totalsHeaderCell.Padding = 10f;
                var totalsHeaderFont = new Font(baseFont, 12, Font.BOLD, whiteColor);
                totalsHeaderCell.Phrase = new Phrase("TỔNG KẾT THANH TOÁN", totalsHeaderFont);
                totalSectionTable.AddCell(totalsHeaderCell);

                // Calculate discount and shipping
                decimal giam = hoaDon.TongTien - hoaDon.TongTienSauKhiGiam;
                decimal phiVanChuyen = 0;
                
                // Tính phí vận chuyển (miễn phí nếu >= 500k, ngược lại 30k)
                if (hoaDon.TongTienSauKhiGiam < 500000)
                {
                    phiVanChuyen = 30000;
                }

                AddEnhancedTotalRow(totalSectionTable, "Tổng tiền hàng:", hoaDon.TongTien.ToString("N0") + "đ", normalFont, boldFont);

                if (giam > 0)
                {
                    AddEnhancedTotalRow(totalSectionTable, "Giảm giá:", "- " + giam.ToString("N0") + "đ", normalFont, new Font(baseFont, 11, Font.BOLD, new BaseColor(231, 76, 60)));
                }
                
                AddEnhancedTotalRow(totalSectionTable, "Phí vận chuyển:", phiVanChuyen.ToString("N0") + "đ", normalFont, boldFont);

                // Final total with emphasis
                var finalLabelCell = new PdfPCell(new Phrase("TỔNG THANH TOÁN:", totalFont));
                finalLabelCell.BackgroundColor = new BaseColor(95, 158, 160); // Cool blue-green
                finalLabelCell.Border = Rectangle.NO_BORDER;
                finalLabelCell.Padding = 10f;
                finalLabelCell.HorizontalAlignment = Element.ALIGN_LEFT;

                var finalValueCell = new PdfPCell(new Phrase(hoaDon.TongTienSauKhiGiam.ToString("N0") + "đ", new Font(baseFont, 16, Font.BOLD, whiteColor)));
                finalValueCell.BackgroundColor = new BaseColor(95, 158, 160); // Cool blue-green
                finalValueCell.Border = Rectangle.NO_BORDER;
                finalValueCell.Padding = 10f;
                finalValueCell.HorizontalAlignment = Element.ALIGN_RIGHT;

                totalSectionTable.AddCell(finalLabelCell);
                totalSectionTable.AddCell(finalValueCell);

                var totalMainCell = new PdfPCell(totalSectionTable);
                totalMainCell.Border = Rectangle.BOX;
                totalMainCell.BorderColor = primaryColor;
                totalMainCell.BorderWidth = 2f;
                totalMainCell.Padding = 0f;
                totalMainCell.HorizontalAlignment = Element.ALIGN_RIGHT;
                totalMainTable.AddCell(totalMainCell);

                document.Add(totalMainTable);

                // Enhanced footer with QR placeholder and thank you message
                var footerTable = new PdfPTable(2);
                footerTable.WidthPercentage = 100;
                footerTable.SetWidths(new float[] { 1, 1 });
                footerTable.SpacingBefore = 30f;

                // Left side - Thank you message
                var thankYouCell = new PdfPCell();
                thankYouCell.Border = Rectangle.NO_BORDER;
                thankYouCell.VerticalAlignment = Element.ALIGN_BOTTOM;

                var thankYou = new Paragraph();
                thankYou.Add(new Chunk("Cảm ơn quý khách!\n", new Font(baseFont, 12, Font.BOLD, primaryColor)));
                thankYou.Add(new Chunk("Hẹn gặp lại quý khách trong lần mua sắm tiếp theo.\n", normalFont));
                thankYou.Add(new Chunk("Hotline hỗ trợ: 0968596808", smallFont));
                thankYouCell.AddElement(thankYou);

                // Right side - Signature area
                var signatureCell = new PdfPCell();
                signatureCell.Border = Rectangle.NO_BORDER;
                signatureCell.HorizontalAlignment = Element.ALIGN_CENTER;

                var signature = new Paragraph();
                signature.Add(new Chunk("Chữ ký khách hàng\n\n\n\n", normalFont));
                signature.Add(new Chunk("_________________", smallFont));
                signature.Alignment = Element.ALIGN_CENTER;
                signatureCell.AddElement(signature);

                footerTable.AddCell(thankYouCell);
                footerTable.AddCell(signatureCell);
                document.Add(footerTable);

                // Add watermark-style footer
                var watermarkTable = new PdfPTable(1);
                watermarkTable.WidthPercentage = 100;
                watermarkTable.SpacingBefore = 20f;

                var watermarkCell = new PdfPCell(new Phrase("www.furryfriends.vn | Powered by FurryFriends System",
                    new Font(baseFont, 8, Font.ITALIC, BaseColor.LIGHT_GRAY)));
                watermarkCell.Border = Rectangle.TOP_BORDER;
                watermarkCell.BorderColor = BaseColor.LIGHT_GRAY;
                watermarkCell.HorizontalAlignment = Element.ALIGN_CENTER;
                watermarkCell.Padding = 5f;
                watermarkTable.AddCell(watermarkCell);
                document.Add(watermarkTable);

                document.Close();
                return memoryStream.ToArray();
            }
        }

        private void AddModernInfoRow(PdfPTable table, string label, string value, Font labelFont, Font valueFont)
        {
            var labelCell = new PdfPCell(new Phrase(label, labelFont));
            labelCell.Border = Rectangle.NO_BORDER;
            labelCell.Padding = 5f;
            labelCell.BackgroundColor = BaseColor.WHITE;
            table.AddCell(labelCell);

            var valueCell = new PdfPCell(new Phrase(value, valueFont));
            valueCell.Border = Rectangle.NO_BORDER;
            valueCell.Padding = 5f;
            valueCell.BackgroundColor = BaseColor.WHITE;
            table.AddCell(valueCell);
        }

        private void AddEnhancedTableHeader(PdfPTable table, string text, Font font, BaseColor backgroundColor)
        {
            var cell = new PdfPCell(new Phrase(text, font));
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.BackgroundColor = backgroundColor;
            cell.Border = Rectangle.NO_BORDER;
            cell.Padding = 8f;
            table.AddCell(cell);
        }

        private void AddEnhancedProductRow(PdfPTable table, string text, Font font, BaseColor backgroundColor, int alignment = Element.ALIGN_LEFT)
        {
            var cell = new PdfPCell(new Phrase(text, font));
            cell.HorizontalAlignment = alignment;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.BackgroundColor = backgroundColor;
            cell.Border = Rectangle.NO_BORDER;
            cell.Padding = 6f;
            table.AddCell(cell);
        }

        private void AddEnhancedTotalRow(PdfPTable table, string label, string value, Font labelFont, Font valueFont)
        {
            var labelCell = new PdfPCell(new Phrase(label, labelFont));
            labelCell.Border = Rectangle.NO_BORDER;
            labelCell.Padding = 8f;
            labelCell.BackgroundColor = BaseColor.WHITE;
            table.AddCell(labelCell);

            var valueCell = new PdfPCell(new Phrase(value, valueFont));
            valueCell.Border = Rectangle.NO_BORDER;
            valueCell.Padding = 8f;
            valueCell.HorizontalAlignment = Element.ALIGN_RIGHT;
            valueCell.BackgroundColor = BaseColor.WHITE;
            table.AddCell(valueCell);
        }

        // Keep existing helper methods for backward compatibility
        private void AddInfoRow(PdfPTable table, string label, string value, Font font)
        {
            AddModernInfoRow(table, label, value, font, font);
        }

        private void AddTableHeader(PdfPTable table, string text, Font font)
        {
            AddEnhancedTableHeader(table, text, font, BaseColor.LIGHT_GRAY);
        }

        private void AddTotalRow(PdfPTable table, string label, string value, Font font)
        {
            AddEnhancedTotalRow(table, label, value, font, font);
        }

        private string GetTrangThaiText(int trangThai)
        {
            return trangThai switch
            {
                0 => "Chờ duyệt",
                1 => "Đã duyệt", 
                2 => "Đang giao",
                3 => "Đã giao",
                4 => "Đã hủy",
                _ => "Không xác định"
            };
        }

        // ✅ Hủy đơn hàng và cộng lại số lượng sản phẩm
        public async Task<ApiResult> HuyDonHangAsync(Guid hoaDonId)
        {
            try
            {
                // Lấy hóa đơn với chi tiết
                var hoaDon = await _context.HoaDons
                    .Include(h => h.HoaDonChiTiets)
                        .ThenInclude(ct => ct.SanPhamChiTiet)
                    .FirstOrDefaultAsync(h => h.HoaDonId == hoaDonId);

                if (hoaDon == null)
                {
                    return new ApiResult { Success = false, Message = "Không tìm thấy đơn hàng!" };
                }

                // ✅ Kiểm tra trạng thái - chỉ cho phép hủy khi "Chờ duyệt" hoặc "Đã duyệt"
                if (hoaDon.TrangThai != 0 && hoaDon.TrangThai != 1)
                {
                    return new ApiResult { Success = false, Message = "Chỉ có thể hủy đơn hàng khi đang ở trạng thái 'Chờ duyệt' hoặc 'Đã duyệt'!" };
                }

                // ✅ Cộng lại số lượng sản phẩm vào kho
                foreach (var chiTiet in hoaDon.HoaDonChiTiets)
                {
                    if (chiTiet.SanPhamChiTiet != null)
                    {
                        // ✅ Sử dụng property SoLuong thay vì SoLuongTon
                        chiTiet.SanPhamChiTiet.SoLuong += chiTiet.SoLuongSanPham;
                    }
                }

                // ✅ Cập nhật trạng thái thành "Đã hủy"
                hoaDon.TrangThai = 4;
                // ✅ Lưu thời gian thực khi hủy đơn
                hoaDon.ThoiGianThayDoiTrangThai = DateTime.Now;

                // ✅ Lưu lịch sử thay đổi trạng thái khi hủy đơn
                var lichSu = new LichSuTrangThaiHoaDon
                {
                    Id = Guid.NewGuid(),
                    HoaDonId = hoaDonId,
                    TrangThaiCu = hoaDon.TrangThai,
                    TrangThaiMoi = 4,
                    ThoiGianThayDoi = DateTime.Now,
                    GhiChu = "Hủy đơn hàng"
                };
                
                _context.LichSuTrangThaiHoaDons.Add(lichSu);
                // ✅ Không cần cập nhật ngày vì model không có property này

                // ✅ Lưu thay đổi
                await _context.SaveChangesAsync();

                return new ApiResult { Success = true, Message = "Hủy đơn hàng thành công! Số lượng sản phẩm đã được cộng lại vào kho." };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in HuyDonHangAsync: {ex.Message}");
                return new ApiResult { Success = false, Message = $"Lỗi khi hủy đơn hàng: {ex.Message}" };
            }
        }

        // ✅ Cập nhật trạng thái đơn hàng
        public async Task<ApiResult> CapNhatTrangThaiAsync(Guid hoaDonId, int trangThaiMoi)
        {
            try
            {
                // Lấy hóa đơn
                var hoaDon = await _context.HoaDons
                    .FirstOrDefaultAsync(h => h.HoaDonId == hoaDonId);

                if (hoaDon == null)
                {
                    return new ApiResult { Success = false, Message = "Không tìm thấy đơn hàng!" };
                }

                // Kiểm tra tính hợp lệ của việc chuyển đổi trạng thái
                if (!IsValidStatusTransition(hoaDon.TrangThai, trangThaiMoi))
                {
                    return new ApiResult { Success = false, Message = GetInvalidTransitionMessage(hoaDon.TrangThai, trangThaiMoi) };
                }

                // ✅ Lưu lịch sử thay đổi trạng thái
                var lichSu = new LichSuTrangThaiHoaDon
                {
                    Id = Guid.NewGuid(),
                    HoaDonId = hoaDonId,
                    TrangThaiCu = hoaDon.TrangThai,
                    TrangThaiMoi = trangThaiMoi,
                    ThoiGianThayDoi = DateTime.Now,
                    GhiChu = $"Thay đổi từ {GetTrangThaiText(hoaDon.TrangThai)} sang {GetTrangThaiText(trangThaiMoi)}"
                };
                
                _context.LichSuTrangThaiHoaDons.Add(lichSu);

                // ✅ Lưu thời gian thực khi thay đổi trạng thái
                hoaDon.ThoiGianThayDoiTrangThai = DateTime.Now;
                
                // Cập nhật trạng thái
                hoaDon.TrangThai = trangThaiMoi;

                // ✅ Nếu hủy đơn (trạng thái 4), hoàn trả số lượng sản phẩm
                if (trangThaiMoi == 4) // Đã hủy
                {
                    await HoanTraSoLuongSanPham(hoaDonId);
                }

                // Lưu thay đổi (bao gồm cả trạng thái và số lượng sản phẩm)
                await _context.SaveChangesAsync();

                var trangThaiText = GetTrangThaiText(trangThaiMoi);
                return new ApiResult { Success = true, Message = $"Cập nhật trạng thái thành công! Trạng thái mới: {trangThaiText}" };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in CapNhatTrangThaiAsync: {ex.Message}");
                return new ApiResult { Success = false, Message = $"Lỗi khi cập nhật trạng thái: {ex.Message}" };
            }
        }

        // Kiểm tra tính hợp lệ của việc chuyển đổi trạng thái
        private bool IsValidStatusTransition(int trangThaiHienTai, int trangThaiMoi)
        {
            // Quy tắc chuyển đổi trạng thái:
            // 0 (Chờ duyệt) → 1 (Đã duyệt) ✓
            // 1 (Đã duyệt) → 2 (Đang giao) ✓
            // 2 (Đang giao) → 3 (Đã giao) ✓
            // 0, 1 → 4 (Đã hủy) ✓ - Cho phép hủy đơn từ trạng thái chờ duyệt và đã duyệt
            // 4 (Đã hủy) → Không thể chuyển sang trạng thái khác

            if (trangThaiHienTai == 4) // Đã hủy
            {
                return false; // Không thể chuyển từ trạng thái đã hủy
            }

            // Cho phép hủy đơn từ trạng thái "Chờ duyệt" và "Đã duyệt"
            if ((trangThaiHienTai == 0 || trangThaiHienTai == 1) && trangThaiMoi == 4)
            {
                return true; // Cho phép hủy đơn
            }

            switch (trangThaiHienTai)
            {
                case 0: // Chờ duyệt
                    return trangThaiMoi == 1; // Chỉ có thể chuyển thành "Đã duyệt"
                
                case 1: // Đã duyệt
                    return trangThaiMoi == 2; // Chỉ có thể chuyển thành "Đang giao"
                
                case 2: // Đang giao
                    return trangThaiMoi == 3; // Chỉ có thể chuyển thành "Đã giao"
                
                case 3: // Đã giao
                    return false; // Không thể chuyển từ trạng thái đã giao
                
                default:
                    return false;
            }
        }

        // Lấy thông báo lỗi khi chuyển đổi trạng thái không hợp lệ
        private string GetInvalidTransitionMessage(int trangThaiHienTai, int trangThaiMoi)
        {
            var trangThaiHienTaiText = GetTrangThaiText(trangThaiHienTai);
            var trangThaiMoiText = GetTrangThaiText(trangThaiMoi);

            if (trangThaiHienTai == 4)
            {
                return "Không thể thay đổi trạng thái của đơn hàng đã hủy";
            }

            if (trangThaiHienTai == 3)
            {
                return "Không thể thay đổi trạng thái của đơn hàng đã giao thành công";
            }

            // Thông báo đặc biệt cho việc hủy đơn
            if (trangThaiMoi == 4)
            {
                if (trangThaiHienTai == 2)
                {
                    return "Không thể hủy đơn hàng đang giao. Vui lòng chờ đơn hàng được giao hoặc liên hệ khách hàng.";
                }
                if (trangThaiHienTai == 3)
                {
                    return "Không thể hủy đơn hàng đã giao thành công.";
                }
            }

            if (trangThaiHienTai == 1 && trangThaiMoi == 0)
            {
                return "Không thể chuyển đơn hàng từ 'Đã duyệt' về 'Chờ duyệt'";
            }

            if (trangThaiHienTai == 2 && (trangThaiMoi == 0 || trangThaiMoi == 1))
            {
                return "Không thể chuyển đơn hàng từ 'Đang giao' về 'Chờ duyệt' hoặc 'Đã duyệt'";
            }

            if (trangThaiHienTai == 3 && (trangThaiMoi == 0 || trangThaiMoi == 1 || trangThaiMoi == 2))
            {
                return "Không thể chuyển đơn hàng từ 'Đã giao' về trạng thái trước đó";
            }

            return $"Không thể chuyển đơn hàng từ '{trangThaiHienTaiText}' sang '{trangThaiMoiText}'";
        }

        // Lấy chi tiết hóa đơn
        public async Task<IEnumerable<HoaDonChiTiet>> GetChiTietHoaDonAsync(Guid hoaDonId)
        {
            try
            {
                if (hoaDonId == Guid.Empty)
                {
                    throw new ArgumentException("ID hóa đơn không hợp lệ");
                }

                var chiTietHoaDon = await _context.HoaDonChiTiets
                    .Where(ct => ct.HoaDonId == hoaDonId)
                    .AsNoTracking()
                    .ToListAsync();

                return chiTietHoaDon ?? new List<HoaDonChiTiet>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetChiTietHoaDonAsync: {ex.Message}");
                return new List<HoaDonChiTiet>();
            }
        }

        // ✅ Hoàn trả số lượng sản phẩm khi hủy đơn hàng
        private async Task HoanTraSoLuongSanPham(Guid hoaDonId)
        {
            try
            {
                // Lấy chi tiết hóa đơn với thông tin sản phẩm
                var chiTietHoaDons = await _context.HoaDonChiTiets
                    .Where(ct => ct.HoaDonId == hoaDonId)
                    .Include(ct => ct.SanPhamChiTiet)
                    .ToListAsync();

                foreach (var chiTiet in chiTietHoaDons)
                {
                    if (chiTiet.SanPhamChiTiet != null)
                    {
                        // Hoàn trả số lượng đã mua vào kho
                        chiTiet.SanPhamChiTiet.SoLuong += chiTiet.SoLuongSanPham;
                        
                        Console.WriteLine($"✅ Hoàn trả {chiTiet.SoLuongSanPham} sản phẩm ID: {chiTiet.SanPhamChiTiet.SanPhamChiTietId}");
                        Console.WriteLine($"   Số lượng trước: {chiTiet.SanPhamChiTiet.SoLuong - chiTiet.SoLuongSanPham} → Sau: {chiTiet.SanPhamChiTiet.SoLuong}");
                    }
                }
                
                Console.WriteLine($"✅ Đã chuẩn bị hoàn trả số lượng cho đơn hàng: {hoaDonId}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Lỗi khi hoàn trả số lượng sản phẩm: {ex.Message}");
                throw; // Re-throw để transaction rollback nếu cần
            }
        }
    }
}