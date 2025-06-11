using FurryFriends.API.Models;
using Microsoft.EntityFrameworkCore;

namespace FurryFriends.API.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext() { }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Data Source=ANH2005\\SQLEXPRESS;Initial Catalog=duantn;Integrated Security=True;Encrypt=True;TrustServerCertificate=True");
            }
        }

        // DbSets
        public DbSet<TaiKhoan> TaiKhoans { get; set; }
        public DbSet<NhanVien> NhanViens { get; set; }
        public DbSet<ChucVu> ChucVus { get; set; }
        public DbSet<KhachHang> KhachHangs { get; set; }
        public DbSet<DiaChiKhachHang> DiaChiKhachHangs { get; set; }
        public DbSet<ThuongHieu> ThuongHieus { get; set; }
        public DbSet<SanPham> SanPhams { get; set; }
        public DbSet<SanPhamChiTiet> SanPhamChiTiets { get; set; }
        public DbSet<KichCo> BangKichCos { get; set; }
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

            ConfigureTaiKhoan(modelBuilder);
            ConfigureSanPham(modelBuilder);
            ConfigureGioHang(modelBuilder);
            ConfigureHoaDon(modelBuilder);
            ConfigureDotGiamGia(modelBuilder);
            ConfigureSanPhamThanhPhanChatLieu(modelBuilder);
        }

        private void ConfigureTaiKhoan(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<NhanVien>()
                .HasOne(nv => nv.TaiKhoan)
                .WithOne(tk => tk.NhanVien)
                .HasForeignKey<NhanVien>(nv => nv.TaiKhoanId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<TaiKhoan>()
                .HasOne(kh => kh.KhachHang)
                .WithMany(tk => tk.TaiKhoans)
                .HasForeignKey(kh => kh.KhachHangId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Voucher>()
                .HasOne(v => v.TaiKhoan)
                .WithMany(tk => tk.Vouchers)
                .HasForeignKey(v => v.TaiKhoanId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<TaiKhoan>()
                .HasIndex(tk => tk.UserName)
                .IsUnique();

            modelBuilder.Entity<TaiKhoan>()
                .Property(tk => tk.TrangThai)
                .HasDefaultValue(true);
        }

        private void ConfigureSanPham(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<SanPham>()
                .HasOne(sp => sp.TaiKhoan)
                .WithMany(tk => tk.SanPhams)
                .HasForeignKey(sp => sp.TaiKhoanId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<SanPham>()
                .HasOne(sp => sp.ThuongHieu)
                .WithMany(th => th.SanPhams)
                .HasForeignKey(sp => sp.ThuongHieuId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<SanPhamChiTiet>()
                .Property(spct => spct.Gia)
                .HasPrecision(18, 2);
        }

        private void ConfigureGioHang(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<GioHang>()
                .HasOne(gh => gh.KhachHangs)
                .WithMany(kh => kh.GioHangs)
                .HasForeignKey(gh => gh.KhachHangId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<GioHangChiTiet>()
                .Property(ghct => ghct.DonGia)
                .HasPrecision(18, 2);

            modelBuilder.Entity<GioHangChiTiet>()
                .Property(ghct => ghct.ThanhTien)
                .HasPrecision(18, 2);
        }

        private void ConfigureHoaDon(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<HoaDon>()
                .Property(hd => hd.TongTienSauKhiGiam)
                .HasPrecision(18, 2);

            modelBuilder.Entity<HoaDonChiTiet>()
                .Property(hdct => hdct.Gia)
                .HasPrecision(18, 2);


            modelBuilder.Entity<HoaDon>()
                .HasOne(hd => hd.TaiKhoan)
                .WithMany(tk => tk.HoaDons)
                .HasForeignKey(hd => hd.TaiKhoanId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<HoaDon>()
                .HasOne(hd => hd.KhachHang)
                .WithMany(kh => kh.HoaDons)
                .HasForeignKey(hd => hd.KhachHangId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<HoaDon>()
                .HasOne(hd => hd.Voucher)
                .WithMany(v => v.HoaDons)
                .HasForeignKey(hd => hd.VoucherId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<HoaDon>()
                .HasOne(hd => hd.HinhThucThanhToan)
                .WithMany(ht => ht.HoaDons)
                .HasForeignKey(hd => hd.HinhThucThanhToanId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<HoaDonChiTiet>()
                .HasOne(hdct => hdct.HoaDon)
                .WithMany(hd => hd.HoaDonChiTiets)
                .HasForeignKey(hdct => hdct.HoaDonId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<HoaDonChiTiet>()
                .HasOne(hdct => hdct.SanPham)
                .WithMany(spct => spct.HoaDonChiTiets)
                .HasForeignKey(hdct => hdct.SanPhamId)
                .OnDelete(DeleteBehavior.Restrict);
        }

        private void ConfigureDotGiamGia(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DotGiamGiaSanPham>()
                .HasOne(dg => dg.GiamGias)
                .WithMany(gg => gg.DotGiamGiaSanPhams)
                .HasForeignKey(dg => dg.GiamGiaId)
                .OnDelete(DeleteBehavior.Cascade);
        }

        private void ConfigureSanPhamThanhPhanChatLieu(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<SanPhamThanhPhan>()
                .HasIndex(sptp => new { sptp.SanPhamId, sptp.ThanhPhanId })
                .IsUnique();

            modelBuilder.Entity<SanPhamChatLieu>()
                .HasIndex(spcl => new { spcl.SanPhamId, spcl.ChatLieuId })
                .IsUnique();
        }
    }
}
