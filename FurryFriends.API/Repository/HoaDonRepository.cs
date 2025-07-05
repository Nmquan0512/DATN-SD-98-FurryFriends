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
                        .ThenInclude(ct => ct.SanPham)
                    .Include(h => h.KhachHang)
                    .Include(h => h.TaiKhoan)
                    .Include(h => h.Voucher)
                    .Include(h => h.HinhThucThanhToan)
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


        public async Task<HoaDon> GetHoaDonByIdAsync(Guid hoaDonId)
        {
            if (hoaDonId == Guid.Empty)
            {
                throw new ArgumentException("ID hóa đơn không hợp lệ");
            }

            var hoaDon = await _context.Set<HoaDon>()
                                 .Include(h => h.HoaDonChiTiets)
                                 .ThenInclude(ct => ct.SanPham)
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
            //if (hoaDon == null)
            //{
            //    throw new KeyNotFoundException($"Không tìm thấy hóa đơn với ID: {hoaDonId}");
            //}

            //if (hoaDon.HoaDonChiTiets == null || !hoaDon.HoaDonChiTiets.Any())
            //{
            //    throw new InvalidOperationException($"Hóa đơn {hoaDonId} không có chi tiết sản phẩm");
            //}

            //// Kiểm tra thông tin khách hàng
            //if (string.IsNullOrWhiteSpace(hoaDon.TenCuaKhachHang))
            //{
            //    throw new InvalidOperationException("Hóa đơn thiếu thông tin tên khách hàng");
            //}

            //if (string.IsNullOrWhiteSpace(hoaDon.SdtCuaKhachHang))
            //{
            //    throw new InvalidOperationException("Hóa đơn thiếu thông tin số điện thoại khách hàng");
            //}

            //// Kiểm tra thông tin thanh toán
            //if (hoaDon.TongTien <= 0)
            //{
            //    throw new InvalidOperationException("Tổng tiền hóa đơn không hợp lệ");
            //}

            //if (hoaDon.TongTienSauKhiGiam <= 0)
            //{
            //    throw new InvalidOperationException("Tổng tiền sau khi giảm giá không hợp lệ");
            //}

            //if (hoaDon.TongTienSauKhiGiam > hoaDon.TongTien)
            //{
            //    throw new InvalidOperationException("Tổng tiền sau khi giảm giá không được lớn hơn tổng tiền");
            //}

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

                var rightCell = new PdfPCell(rightInfoTable);
                rightCell.Border = Rectangle.BOX;
                rightCell.BorderColor = lightGray;
                rightCell.BorderWidth = 1f;
                rightCell.Padding = 0f;

                infoMainTable.AddCell(leftCell);
                infoMainTable.AddCell(rightCell);
                document.Add(infoMainTable);

                // Product details with enhanced styling
                var detailTable = new PdfPTable(5);
                detailTable.WidthPercentage = 100;
                detailTable.SetWidths(new float[] { 0.8f, 3f, 1f, 1.2f, 1.5f });
                detailTable.SpacingAfter = 20f;

                // Table header with gradient-like effect
                AddEnhancedTableHeader(detailTable, "STT", headerFont, accentColor);
                AddEnhancedTableHeader(detailTable, "TÊN SẢN PHẨM", headerFont, accentColor);
                AddEnhancedTableHeader(detailTable, "SL", headerFont, accentColor);
                AddEnhancedTableHeader(detailTable, "ĐƠN GIÁ", headerFont, accentColor);
                AddEnhancedTableHeader(detailTable, "THÀNH TIỀN", headerFont, accentColor);

                // Add products with alternating row colors
                int stt = 1;
                decimal tongTienHang = 0;

                foreach (var chiTiet in hoaDon.HoaDonChiTiets)
                {
                    var rowColor = (stt % 2 == 0) ? lightGray : whiteColor;
                    decimal thanhTien = chiTiet.SoLuongSanPham * chiTiet.Gia;
                    tongTienHang += thanhTien;

                    AddEnhancedProductRow(detailTable, stt.ToString(), normalFont, rowColor);
                    AddEnhancedProductRow(detailTable, chiTiet.SanPham?.TenSanPham ?? "N/A", normalFont, rowColor);
                    AddEnhancedProductRow(detailTable, chiTiet.SoLuongSanPham.ToString(), normalFont, rowColor, Element.ALIGN_CENTER);
                    AddEnhancedProductRow(detailTable, chiTiet.Gia.ToString("N0") + "đ", normalFont, rowColor, Element.ALIGN_RIGHT);
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

                // Calculate discount if any
                decimal giam = hoaDon.TongTien - hoaDon.TongTienSauKhiGiam;

                AddEnhancedTotalRow(totalSectionTable, "Tổng tiền hàng:", hoaDon.TongTien.ToString("N0") + "đ", normalFont, boldFont);

                if (giam > 0)
                {
                    AddEnhancedTotalRow(totalSectionTable, "Giảm giá:", "- " + giam.ToString("N0") + "đ", normalFont, new Font(baseFont, 11, Font.BOLD, new BaseColor(231, 76, 60)));
                }

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
    }
}