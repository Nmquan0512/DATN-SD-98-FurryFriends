using FurryFriends.API.Models;
using Microsoft.EntityFrameworkCore;

namespace FurryFriends.API.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			optionsBuilder.UseSqlServer("Data Source=xxx;Initial Catalog=xxxx;Trusted_Connection=True;TrustServerCertificate=True");
		}

		protected AppDbContext()
		{
		}

		public DbSet<ChucVu> ChucVus { get; set; }
        public DbSet<NhanVien> NhanViens { get; set; }
        public DbSet<SanPham> SanPhams { get; set; }
        public DbSet<GiamGia> GiamGias { get; set; }
        public DbSet<DotGiamGiaSanPham> DotGiamGiaSanPhams { get; set; }
        public DbSet<Anh> anhs { get; set; }
        public DbSet<BangKichCo> bangKichCos { get; set; }
        public DbSet<ChucVu> chucVus { get; set; }
        public DbSet<DiaChiKhachHang> diaChiKhachHangs { get; set; }
        public DbSet<GioHang> GioHangs { get; set; }
        public DbSet<HinhThucThanhToan> hinhThucThanhToans { get; set; }
        public DbSet<HoaDon> hoaDons { get; set; }
        public DbSet<HoaDonChiTiet> hoaDonsChiTiet { get; set; }
        public DbSet<KhachHang> khachHangs { get; set; }
        public DbSet<MauSac> mauSacs { get; set; }
        public DbSet<SanPhamChiTiet> sanPhamChiTiets { get; set; }
        public DbSet<TaiKhoan> taiKhoans { get; set; }
        public DbSet<ThuongHieu> thuongHieus { get; set; }
        public DbSet<Voucher> vouchers { get; set; }

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

            // Cấu hình quan hệ Anh - SanPhamChiTiet (1:N) - Bat Buoc
            modelBuilder.Entity<SanPhamChiTiet>()
                .HasOne(spct => spct.Anh)
                .WithMany(a => a.SanPhamChiTiets)
                .HasForeignKey(spct => spct.AnhId)
                .OnDelete(DeleteBehavior.SetNull);

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

            // Cấu hình quan hệ SanPhamChiTiet - GioHangChiTiet (1:N)
            modelBuilder.Entity<GioHangChiTiet>()
                .HasOne(ghct => ghct.SanPhamChiTiet)
                .WithMany(spct => spct.GioHangChiTiets)
                .HasForeignKey(ghct => ghct.SanPhamChiTietId)
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

            // Cấu hình quan hệ SanPhamChiTiet - HoaDonChiTiet (1:N)
            modelBuilder.Entity<HoaDonChiTiet>()
                .HasOne(hdct => hdct.SanPhamChiTiet)
                .WithMany(spct => spct.HoaDonChiTiets)
                .HasForeignKey(hdct => hdct.SanPhamChiTietId)
                .OnDelete(DeleteBehavior.Restrict);

            // Cau hinh gia
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

            // de truy van hon
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

            // Cho trang thai mac dinh la true
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
        }
    }
}
