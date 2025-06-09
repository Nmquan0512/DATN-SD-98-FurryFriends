using FurryFriends.API.Data;
using FurryFriends.API.Models;
using FurryFriends.API.Repository.IRepository;
using Microsoft.EntityFrameworkCore;

namespace FurryFriends.API.Repository
{
    public class HoaDonRepository : IHoaDonRepository
    {
        private readonly DbContext _context;

        public HoaDonRepository(DbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<HoaDon>> GetHoaDonListAsync()
        {
            return await _context.Set<HoaDon>()
                                 .Include(h => h.HoaDonChiTiets)
                                 .ToListAsync();
        }

        public async Task<HoaDon> GetHoaDonByIdAsync(Guid hoaDonId)
        {
            return await _context.Set<HoaDon>()
                                 .Include(h => h.HoaDonChiTiets)
                                 .ThenInclude(ct => ct.SanPham)
                                 .FirstOrDefaultAsync(h => h.HoaDonId == hoaDonId);
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
            var hoaDon = await GetHoaDonByIdAsync(hoaDonId);

            if (hoaDon == null)
                throw new Exception("Không tìm thấy hóa đơn");

            using (var stream = new MemoryStream())
            {
                var document = new PdfSharp.Pdf.PdfDocument();
                var page = document.AddPage();
                var gfx = PdfSharp.Drawing.XGraphics.FromPdfPage(page);

                gfx.DrawString($"Hóa Đơn ID: {hoaDon.HoaDonId}", new PdfSharp.Drawing.XFont("Arial", 14), PdfSharp.Drawing.XBrushes.Black, new PdfSharp.Drawing.XPoint(40, 50));
                gfx.DrawString($"Ngày tạo: {hoaDon.NgayTao}", new PdfSharp.Drawing.XFont("Arial", 12), PdfSharp.Drawing.XBrushes.Black, new PdfSharp.Drawing.XPoint(40, 80));
                gfx.DrawString($"Tổng tiền: {hoaDon.TongTienSauKhiGiam:C}", new PdfSharp.Drawing.XFont("Arial", 12), PdfSharp.Drawing.XBrushes.Black, new PdfSharp.Drawing.XPoint(40, 110));

                int yOffset = 140;
                foreach (var chiTiet in hoaDon.HoaDonChiTiets)
                {
                    gfx.DrawString($"{chiTiet.SanPham.TenSanPham} - SL: {chiTiet.SoLuongSanPham}, Giá: {chiTiet.Gia:C}",
                        new PdfSharp.Drawing.XFont("Arial", 10), PdfSharp.Drawing.XBrushes.Black,
                        new PdfSharp.Drawing.XPoint(40, yOffset));
                    yOffset += 20;
                }

                document.Save(stream);
                return stream.ToArray();
            }
        }
    }

}
