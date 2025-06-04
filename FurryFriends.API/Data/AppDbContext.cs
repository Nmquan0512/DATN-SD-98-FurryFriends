using FurryFriends.API.Models;
using Microsoft.EntityFrameworkCore;

namespace FurryFriends.API.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<ChucVu> ChucVus { get; set; }
        public DbSet<NhanVien> NhanViens { get; set; }
        public DbSet<SanPham> SanPhams { get; set; }
        public DbSet<GiamGia> GiamGias { get; set; }
        public DbSet<DotGiamGiaSanPham> DotGiamGiaSanPhams { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure relationships
            modelBuilder.Entity<NhanVien>()
                .HasOne(nv => nv.ChucVu)
                .WithMany(cv => cv.NhanViens)
                .HasForeignKey(nv => nv.ChucVuId);

            modelBuilder.Entity<DotGiamGiaSanPham>()
                .HasOne(d => d.GiamGia)
                .WithMany(g => g.DotGiamGiaSanPhams)
                .HasForeignKey(d => d.GiamGiaId);

            modelBuilder.Entity<DotGiamGiaSanPham>()
                .HasOne(d => d.SanPham)
                .WithMany(s => s.DotGiamGiaSanPhams)
                .HasForeignKey(d => d.SanPhamId);
        }
    }
}
