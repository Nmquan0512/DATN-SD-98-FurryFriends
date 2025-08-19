using FurryFriends.API.Data;
using FurryFriends.API.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace FurryFriends.API.Services
{
    public class InvoiceCleanupService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<InvoiceCleanupService> _logger;
        private readonly TimeSpan _checkInterval = TimeSpan.FromMinutes(5); // Kiểm tra mỗi 5 phút

        public InvoiceCleanupService(IServiceProvider serviceProvider, ILogger<InvoiceCleanupService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Invoice Cleanup Service đã khởi động");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await CleanupOldInvoices();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Lỗi trong quá trình cleanup hóa đơn");
                }

                await Task.Delay(_checkInterval, stoppingToken);
            }
        }

        private async Task CleanupOldInvoices()
        {
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            try
            {
                var cutoffTime = DateTime.UtcNow.AddMinutes(-30); // 30 phút trước

                var oldInvoices = await context.HoaDons
                    .Include(h => h.HoaDonChiTiets)
                    .Where(h => h.TrangThai == (int)TrangThaiHoaDon.Offline_ChuaThanhToan && 
                               h.NgayTao < cutoffTime)
                    .ToListAsync();

                if (oldInvoices.Any())
                {
                    _logger.LogInformation($"Tự động hủy {oldInvoices.Count} hóa đơn chưa thanh toán sau 30 phút");

                    foreach (var invoice in oldInvoices)
                    {
                        // Hoàn trả số lượng sản phẩm
                        foreach (var item in invoice.HoaDonChiTiets)
                        {
                            var sanPhamChiTiet = await context.SanPhamChiTiets.FindAsync(item.SanPhamChiTietId);
                            if (sanPhamChiTiet != null)
                            {
                                sanPhamChiTiet.SoLuong += item.SoLuongSanPham;
                            }
                        }

                        // Hoàn trả voucher nếu có
                        if (invoice.VoucherId.HasValue)
                        {
                            var voucher = await context.Vouchers.FindAsync(invoice.VoucherId.Value);
                            if (voucher != null)
                            {
                                voucher.SoLuong++;
                            }
                        }

                        // Cập nhật trạng thái thành đã hủy
                        invoice.TrangThai = (int)TrangThaiHoaDon.Offline_DaHuy;
                    }

                    await context.SaveChangesAsync();
                    _logger.LogInformation($"Đã tự động hủy {oldInvoices.Count} hóa đơn thành công");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tự động hủy hóa đơn cũ");
            }
        }
    }
} 