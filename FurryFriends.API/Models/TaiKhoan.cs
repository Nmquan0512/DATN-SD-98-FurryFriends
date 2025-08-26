using FurryFriends.API.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FurryFriends.API.Models
{
    public class TaiKhoan : IValidatableObject
    {
        [Key]
        public Guid TaiKhoanId { get; set; }

        [Required(ErrorMessage = "Tên đăng nhập không được để trống")]
        [StringLength(50, MinimumLength = 6, ErrorMessage = "Tên đăng nhập phải có từ 6 đến 50 ký tự")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Mật khẩu không được để trống")]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "Mật khẩu phải có từ 8 đến 100 ký tự")]

        public string Password { get; set; }
        [Required]
        public DateTime NgayTaoTaiKhoan { get; set
; }

        public DateTime? NgayCapNhatCuoiCung { get; set; }

        [Required]
        public bool TrangThai { get; set; }

        public Guid? KhachHangId { get; set; }  // Cho phép null
        [ForeignKey("KhachHangId")]
        public virtual KhachHang? KhachHang { get; set; }

        public Guid? NhanVienId { get; set; }  // Cho phép null
        [ForeignKey("NhanVienId")]
        public virtual NhanVien? NhanVien { get; set; }

        public TaiKhoan()
        {
            TaiKhoanId = Guid.NewGuid();
            TrangThai = true;
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