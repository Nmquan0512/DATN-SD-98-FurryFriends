using FurryFriends.API.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FurryFriends.API.Models
{
    public class TaiKhoan : IValidatableObject
    {
        [Key]
        public Guid TaiKhoanId { get; set; }

        [Required]
        public string UserName { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public DateTime NgayTaoTaiKhoan { get; set; }

        public DateTime? NgayCapNhatCuoiCung { get; set; }

        [Required]
        public bool TrangThai { get; set; }

        public Guid? KhachHangId { get; set; }  // Cho phép null
        [ForeignKey("KhachHangId")]
        public virtual KhachHang? KhachHang { get; set; }

        public virtual NhanVien? NhanVien { get; set; }

        public virtual ICollection<SanPham>? SanPhams { get; set; }
        public virtual ICollection<HoaDon>? HoaDons { get; set; }

        public TaiKhoan()
        {
            TaiKhoanId = Guid.NewGuid();
            TrangThai = true;
            SanPhams = new List<SanPham>();
            HoaDons = new List<HoaDon>();
        }
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var _context = (AppDbContext)validationContext.GetService(typeof(AppDbContext));

            if (_context != null)
            {
                var isDuplicate = _context.TaiKhoans
                    .Any(x => x.UserName.ToLower().Trim() == UserName.ToLower().Trim()
                           && x.TaiKhoanId != TaiKhoanId);

                if (isDuplicate)
                {
                    yield return new ValidationResult(
                        "Tên đăng nhập đã tồn tại.",
                        new[] { nameof(UserName) });
                }
            }
        }
    }
}