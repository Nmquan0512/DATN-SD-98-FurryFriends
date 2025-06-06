using Microsoft.EntityFrameworkCore;
using FurryFriends.API.Models;

namespace FurryFriends.API.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        // DbSet cho các entities
        public DbSet<TaiKhoan> TaiKhoans { get; set; }
        public DbSet<NhanVien> NhanViens { get; set; }
        public DbSet<ChucVu> ChucVus { get; set; }
        public DbSet<KhachHang> KhachHangs { get; set; }
        public DbSet<DiaChiKhachHang> DiaChiKhachHangs { get; set; }
        public DbSet<ThuongHieu> ThuongHieus { get; set; }
        public DbSet<SanPham> SanPhams { get; set; }
        public DbSet<SanPhamChiTiet> SanPhamChiTiets { get; set; }
        public DbSet<BangKichCo> BangKichCos { get; set; }
        public DbSet<MauSac> MauSacs { get; set; }
        public DbSet<GiamGia> GiamGias { get; set; }
        public DbSet<DotGiamGiaSanPham> DotGiamGiaSanPhams { get; set; }
        public DbSet<Voucher> Vouchers { get; set; }
        public DbSet<GioHang> GioHangs { get; set; }
        public DbSet<GioHangChiTiet> GioHangChiTiets { get; set; }
        public DbSet<HoaDon> HoaDons { get; set; }
        public DbSet<HoaDonChiTiet> HoaDonChiTiets { get; set; }
        public DbSet<HinhThucThanhToan> HinhThucThanhToans { get; set; }
        public DbSet<Anh> Anhs { get; set; }
        public DbSet<ThanhPhan> ThanhPhans { get; set; }
        public DbSet<ChatLieu> ChatLieus { get; set; }
        public DbSet<SanPhamThanhPhan> SanPhamThanhPhans { get; set; }
        public DbSet<SanPhamChatLieu> SanPhamChatLieus { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Cấu hình quan hệ TaiKhoan - NhanVien (1:1)
            modelBuilder.Entity<NhanVien>()
                .HasOne(nv => nv.TaiKhoan)
                .WithOne(tk => tk.NhanVien)
                .HasForeignKey<NhanVien>(nv => nv.TaiKhoanId)
                .OnDelete(DeleteBehavior.Cascade);

            // Cấu hình quan hệ ChucVu - NhanVien (1:N)
            modelBuilder.Entity<NhanVien>()
                .HasOne(nv => nv.ChucVu)
                .WithMany(cv => cv.NhanViens)
                .HasForeignKey(nv => nv.ChucVuId)
                .OnDelete(DeleteBehavior.Restrict);

            // Cấu hình quan hệ TaiKhoan - KhachHang (1:N)
            modelBuilder.Entity<KhachHang>()
                .HasOne(kh => kh.TaiKhoan)
                .WithMany(tk => tk.KhachHangs)
                .HasForeignKey(kh => kh.TaiKhoanId)
                .OnDelete(DeleteBehavior.Cascade);

            // Cấu hình quan hệ KhachHang - DiaChiKhachHang (1:N)
            modelBuilder.Entity<DiaChiKhachHang>()
                .HasOne(dc => dc.KhachHang)
                .WithMany(kh => kh.DiaChiKhachHangs)
                .HasForeignKey(dc => dc.KhachHangId)
                .OnDelete(DeleteBehavior.Cascade);

            // Cấu hình quan hệ TaiKhoan - SanPham (1:N)
            modelBuilder.Entity<SanPham>()
                .HasOne(sp => sp.TaiKhoan)
                .WithMany(tk => tk.SanPhams)
                .HasForeignKey(sp => sp.TaiKhoanId)
                .OnDelete(DeleteBehavior.Restrict);

            // Cấu hình quan hệ ThuongHieu - SanPham (1:N)
            modelBuilder.Entity<SanPham>()
                .HasOne(sp => sp.ThuongHieu)
                .WithMany(th => th.SanPhams)
                .HasForeignKey(sp => sp.ThuongHieuId)
                .OnDelete(DeleteBehavior.Restrict);

            // Cấu hình quan hệ SanPham - SanPhamChiTiet (1:N)
            modelBuilder.Entity<SanPhamChiTiet>()
                .HasOne(spct => spct.SanPham)
                .WithMany(sp => sp.SanPhamChiTiets)
                .HasForeignKey(spct => spct.SanPhamId)
                .OnDelete(DeleteBehavior.Cascade);

            // Cấu hình quan hệ BangKichCo - SanPhamChiTiet (1:N)
            modelBuilder.Entity<SanPhamChiTiet>()
                .HasOne(spct => spct.BangKichCo)
                .WithMany(kc => kc.SanPhamChiTiets)
                .HasForeignKey(spct => spct.KichCoId)
                .OnDelete(DeleteBehavior.Restrict);

            // Cấu hình quan hệ MauSac - SanPhamChiTiet (1:N)
            modelBuilder.Entity<SanPhamChiTiet>()
                .HasOne(spct => spct.MauSac)
                .WithMany(ms => ms.SanPhamChiTiets)
                .HasForeignKey(spct => spct.MauSacId)
                .OnDelete(DeleteBehavior.Restrict);

            // Cấu hình quan hệ Anh - SanPhamChiTiet (1:N) - Optional
            modelBuilder.Entity<SanPhamChiTiet>()
                .HasOne(spct => spct.Anh)
                .WithMany(a => a.SanPhamChiTiets)
                .HasForeignKey(spct => spct.AnhId)
                .OnDelete(DeleteBehavior.SetNull);

            // Cấu hình quan hệ ThanhPhan - SanPhamThanhPhan (1:N)
            modelBuilder.Entity<SanPhamThanhPhan>()
                .HasOne(sptp => sptp.ThanhPhan)
                .WithMany(tp => tp.SanPhamThanhPhans)
                .HasForeignKey(sptp => sptp.ThanhPhanId)
                .OnDelete(DeleteBehavior.Restrict);

            // Cấu hình quan hệ SanPham - SanPhamThanhPhan (1:N)
            modelBuilder.Entity<SanPhamThanhPhan>()
                .HasOne(sptp => sptp.SanPham)
                .WithMany(sp => sp.SanPhamThanhPhans)
                .HasForeignKey(sptp => sptp.SanPhamId)
                .OnDelete(DeleteBehavior.Cascade);

            // Cấu hình quan hệ ChatLieu - SanPhamChatLieu (1:N)
            modelBuilder.Entity<SanPhamChatLieu>()
                .HasOne(spcl => spcl.ChatLieu)
                .WithMany(cl => cl.SanPhamChatLieus)
                .HasForeignKey(spcl => spcl.ChatLieuId)
                .OnDelete(DeleteBehavior.Restrict);

            // Cấu hình quan hệ SanPham - SanPhamChatLieu (1:N)
            modelBuilder.Entity<SanPhamChatLieu>()
                .HasOne(spcl => spcl.SanPham)
                .WithMany(sp => sp.SanPhamChatLieus)
                .HasForeignKey(spcl => spcl.SanPhamId)
                .OnDelete(DeleteBehavior.Cascade);

            // Cấu hình composite key cho SanPhamThanhPhan (tránh trùng lặp)
            modelBuilder.Entity<SanPhamThanhPhan>()
                .HasIndex(sptp => new { sptp.SanPhamId, sptp.ThanhPhanId })
                .IsUnique();

            // Cấu hình composite key cho SanPhamChatLieu (tránh trùng lặp)
            modelBuilder.Entity<SanPhamChatLieu>()
                .HasIndex(spcl => new { spcl.SanPhamId, spcl.ChatLieuId })
                .IsUnique();

            // Cấu hình quan hệ GiamGia - DotGiamGiaSanPham (1:N)
            modelBuilder.Entity<DotGiamGiaSanPham>()
                .HasOne(dg => dg.GiamGia)
                .WithMany(gg => gg.DotGiamGiaSanPhams)
                .HasForeignKey(dg => dg.GiamGiaId)
                .OnDelete(DeleteBehavior.Cascade);

            // Cấu hình quan hệ SanPham - DotGiamGiaSanPham (1:N)
            modelBuilder.Entity<DotGiamGiaSanPham>()
                .HasOne(dg => dg.SanPham)
                .WithMany(sp => sp.DotGiamGiaSanPhams)
                .HasForeignKey(dg => dg.SanPhamId)
                .OnDelete(DeleteBehavior.Cascade);

            // Cấu hình quan hệ TaiKhoan - Voucher (1:N)
            modelBuilder.Entity<Voucher>()
                .HasOne(v => v.TaiKhoan)
                .WithMany(tk => tk.Vouchers)
                .HasForeignKey(v => v.TaiKhoanId)
                .OnDelete(DeleteBehavior.Restrict);

            // Cấu hình quan hệ KhachHang - GioHang (1:N)
            modelBuilder.Entity<GioHang>()
                .HasOne(gh => gh.KhachHang)
                .WithMany(kh => kh.GioHangs)
                .HasForeignKey(gh => gh.KhachHangId)
                .OnDelete(DeleteBehavior.Cascade);

            // Cấu hình quan hệ GioHang - GioHangChiTiet (1:N)
            modelBuilder.Entity<GioHangChiTiet>()
                .HasOne(ghct => ghct.GioHang)
                .WithMany(gh => gh.GioHangChiTiets)
                .HasForeignKey(ghct => ghct.GioHangId)
                .OnDelete(DeleteBehavior.Cascade);

            // Cấu hình quan hệ SanPham - GioHangChiTiet (1:N)
            modelBuilder.Entity<GioHangChiTiet>()
                .HasOne(ghct => ghct.SanPham)
                .WithMany(sp => sp.GioHangChiTiets)
                .HasForeignKey(ghct => ghct.SanPhamId)
                .OnDelete(DeleteBehavior.Restrict);


            // Cấu hình quan hệ TaiKhoan - HoaDon (1:N)
            modelBuilder.Entity<HoaDon>()
                .HasOne(hd => hd.TaiKhoan)
                .WithMany(tk => tk.HoaDons)
                .HasForeignKey(hd => hd.TaiKhoanId)
                .OnDelete(DeleteBehavior.Restrict);

            // Cấu hình quan hệ KhachHang - HoaDon (1:N)
            modelBuilder.Entity<HoaDon>()
                .HasOne(hd => hd.KhachHang)
                .WithMany(kh => kh.HoaDons)
                .HasForeignKey(hd => hd.KhachHangId)
                .OnDelete(DeleteBehavior.Restrict);

            // Cấu hình quan hệ Voucher - HoaDon (1:N) - Optional
            modelBuilder.Entity<HoaDon>()
                .HasOne(hd => hd.Voucher)
                .WithMany(v => v.HoaDons)
                .HasForeignKey(hd => hd.VoucherId)
                .OnDelete(DeleteBehavior.SetNull);

            // Cấu hình quan hệ HinhThucThanhToan - HoaDon (1:N)
            modelBuilder.Entity<HoaDon>()
                .HasOne(hd => hd.HinhThucThanhToan)
                .WithMany(httt => httt.HoaDons)
                .HasForeignKey(hd => hd.HinhThucThanhToanId)
                .OnDelete(DeleteBehavior.Restrict);

            // Cấu hình quan hệ HoaDon - HoaDonChiTiet (1:N)
            modelBuilder.Entity<HoaDonChiTiet>()
                .HasOne(hdct => hdct.HoaDon)
                .WithMany(hd => hd.HoaDonChiTiets)
                .HasForeignKey(hdct => hdct.HoaDonId)
                .OnDelete(DeleteBehavior.Cascade);

            // Cấu hình quan hệ SanPham - HoaDonChiTiet (1:N)
            modelBuilder.Entity<HoaDonChiTiet>()
                .HasOne(hdct => hdct.SanPham)
                .WithMany(sp => sp.HoaDonChiTiets)
                .HasForeignKey(hdct => hdct.SanPhamId)
                .OnDelete(DeleteBehavior.Restrict);

            // Cấu hình precision cho decimal
            modelBuilder.Entity<SanPhamChiTiet>()
                .Property(sp => sp.Gia)
                .HasPrecision(18, 2);

            modelBuilder.Entity<GioHangChiTiet>()
                .Property(gh => gh.DonGia)
                .HasPrecision(18, 2);

            modelBuilder.Entity<GioHangChiTiet>()
                .Property(gh => gh.ThanhTien)
                .HasPrecision(18, 2);

            modelBuilder.Entity<HoaDon>()
                .Property(hd => hd.TongTienSauKhiGiam)
                .HasPrecision(18, 2);

            modelBuilder.Entity<HoaDonChiTiet>()
                .Property(hd => hd.Gia)
                .HasPrecision(18, 2);

            // Cấu hình index cho các trường thường xuyên query
            modelBuilder.Entity<KhachHang>()
                .HasIndex(kh => kh.SDT)
                .IsUnique(false);

            modelBuilder.Entity<KhachHang>()
                .HasIndex(kh => kh.EmailCuaKhachHang)
                .IsUnique(false);

            modelBuilder.Entity<NhanVien>()
                .HasIndex(nv => nv.Email)
                .IsUnique();

            modelBuilder.Entity<TaiKhoan>()
                .HasIndex(tk => tk.UserName)
                .IsUnique();

            // Cấu hình default values
            modelBuilder.Entity<TaiKhoan>()
                .Property(tk => tk.TrangThai)
                .HasDefaultValue(true);

            modelBuilder.Entity<BangKichCo>()
                .Property(kc => kc.TrangThai)
                .HasDefaultValue(true);

            modelBuilder.Entity<MauSac>()
                .Property(ms => ms.TrangThai)
                .HasDefaultValue(true);

            modelBuilder.Entity<ThuongHieu>()
                .Property(th => th.TrangThai)
                .HasDefaultValue(true);

            modelBuilder.Entity<Anh>()
                .Property(a => a.TrangThai)
                .HasDefaultValue(true);

            modelBuilder.Entity<ThanhPhan>()
                .Property(tp => tp.TrangThai)
                .HasDefaultValue(true);

            modelBuilder.Entity<ChatLieu>()
                .Property(cl => cl.TrangThai)
                .HasDefaultValue(true);
        }
    }
}