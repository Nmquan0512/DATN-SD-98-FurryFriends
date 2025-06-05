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
            //Ghi moi quan he trong day
        }
    }
}
