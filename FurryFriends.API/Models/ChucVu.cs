using FurryFriends.API.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FurryFriends.API.Models
{
    public class ChucVu : IValidatableObject
    {
        [Key]
        public Guid ChucVuId { get; set; }

        [Required(ErrorMessage = "Tên chức vụ là bắt buộc.")]
        [StringLength(50, ErrorMessage = "Tên chức vụ tối đa 50 ký tự.")]
        public string TenChucVu { get; set; }

        [Required]
        [StringLength(250)]
        public string MoTaChucVu { get; set; }

        [Required]
        public bool TrangThai { get; set; }

        [Required]
        public DateTime NgayTao { get; set; }

        [Required]
        public DateTime NgayCapNhat { get; set; }

        public virtual ICollection<NhanVien>? NhanViens { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var _context = (AppDbContext)validationContext.GetService(typeof(AppDbContext));

            if (_context != null)
            {
                var isDuplicate = _context.ChucVus
                    .Any(c => c.TenChucVu.ToLower().Trim() == TenChucVu.ToLower().Trim()
                           && c.ChucVuId != ChucVuId); // bỏ qua chính nó khi update

                if (isDuplicate)
                {
                    yield return new ValidationResult(
                        "Tên chức vụ đã tồn tại.",
                        new[] { nameof(TenChucVu) });
                }
            }
        }
    }

}