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
<<<<<<< HEAD
                optionsBuilder.UseSqlServer("Server=XCBA2\\SQLEXPRESS;Database=DuanNhom11ModelsBanHang;Trusted_Connection=True;TrustServerCertificate=True");
=======
                //optionsBuilder.UseSqlServer("Data Source=ANH2005\\SQLEXPRESS;Initial Catalog=duantn;Integrated Security=True;Encrypt=True;TrustServerCertificate=True");
                optionsBuilder.UseSqlServer("Data Source=PHUNGHUYTRUONG\\SQLEXPRESS01;Initial Catalog=BanHang;Integrated Security=True;Encrypt=True;TrustServerCertificate=True");
>>>>>>> origin/HuyTruongDatHang
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
        public DbSet<KichCo> KichCos { get; set; }
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
        public DbSet<ThongBao> ThongBaos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Gọi các hàm cấu hình chi tiết
            ConfigureTaiKhoan(modelBuilder);
            ConfigureSanPham(modelBuilder);
            ConfigureGioHang(modelBuilder);
            ConfigureHoaDon(modelBuilder);
            ConfigureDotGiamGiaSanPham(modelBuilder);
            ConfigureSanPhamThanhPhanChatLieu(modelBuilder);

            // Seed admin account
            modelBuilder.Entity<TaiKhoan>().HasData(new TaiKhoan
            {
                TaiKhoanId = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                UserName = "admin",
                Password = "123456",
                NgayTaoTaiKhoan = DateTime.UtcNow,
                TrangThai = true
            });

            // Seed admin role
            modelBuilder.Entity<ChucVu>().HasData(new ChucVu
            {
                ChucVuId = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                TenChucVu = "admin",
                MoTaChucVu = "Quản trị viên hệ thống",
                TrangThai = true,
                NgayTao = DateTime.UtcNow,
                NgayCapNhat = DateTime.UtcNow
            });

            // Seed admin employee
            modelBuilder.Entity<NhanVien>().HasData(new NhanVien
            {
                NhanVienId = Guid.Parse("33333333-3333-3333-3333-333333333333"),
                TaiKhoanId = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                HoVaTen = "Admin hệ thống",
                NgaySinh = new DateTime(1990, 1, 1),
                DiaChi = "Hà Nội",
                SDT = "0123456789",
                Email = "admin@furryfriends.local",
                GioiTinh = "Nam",
                ChucVuId = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                TrangThai = true,
                NgayTao = DateTime.UtcNow,
                NgayCapNhat = DateTime.UtcNow
            });
        }

        private void ConfigureTaiKhoan(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<NhanVien>()
                .HasOne(nv => nv.TaiKhoan)
                .WithOne(tk => tk.NhanVien)
                .HasForeignKey<NhanVien>(nv => nv.TaiKhoanId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<TaiKhoan>()
                .HasOne(tk => tk.KhachHang)
                .WithMany(kh => kh.TaiKhoans)
                .HasForeignKey(tk => tk.KhachHangId);

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
                .HasOne(sp => sp.ThuongHieu)
                .WithMany(th => th.SanPhams)
                .HasForeignKey(sp => sp.ThuongHieuId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<SanPham>()
                .HasMany(sp => sp.SanPhamChiTiets)
                .WithOne(spct => spct.SanPham)
                .HasForeignKey(spct => spct.SanPhamId)
                .OnDelete(DeleteBehavior.Restrict); //sửa ở đây lúc trước là Cascade sau sửa thành Restrict

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

            modelBuilder.Entity<GioHangChiTiet>() //Sửa ở đây
                .HasOne(ghct => ghct.SanPham)
                .WithMany(sp => sp.GioHangChiTiets) // nếu bạn không có navigation, dùng .WithMany()
                .HasForeignKey(ghct => ghct.SanPhamId)
                .OnDelete(DeleteBehavior.Restrict); // hoặc NoAction

            modelBuilder.Entity<GioHangChiTiet>() //Sửa ở đây
                .HasOne(ghct => ghct.SanPhamChiTiet)
                .WithMany(spct => spct.GioHangChiTiets) // nếu có navigation
                .HasForeignKey(ghct => ghct.SanPhamChiTietId)
                .OnDelete(DeleteBehavior.Restrict); // hoặc NoAction

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

        private void ConfigureDotGiamGiaSanPham(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DotGiamGiaSanPham>()
                .HasOne(d => d.GiamGia)
                .WithMany(g => g.DotGiamGiaSanPhams)
                .HasForeignKey(d => d.GiamGiaId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<DotGiamGiaSanPham>()
                .HasOne(d => d.SanPhamChiTiet)
                .WithMany(sp => sp.DotGiamGiaSanPhams)
                .HasForeignKey(d => d.SanPhamChiTietId)
                .OnDelete(DeleteBehavior.Restrict);
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
