using FurryFriends.API.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace FurryFriends.API.Models
{
    public class Voucher : IValidatableObject
    {
        [Key]
        public Guid VoucherId { get; set; }

        [Required(ErrorMessage = "Tên voucher là bắt buộc.")]
        [StringLength(100, ErrorMessage = "Tên voucher tối đa 100 ký tự.")]
        public string TenVoucher { get; set; }

        [Required]
        public DateTime NgayBatDau { get; set; }

        [Required]
        public DateTime NgayKetThuc { get; set; }
        [Range(0, 100, ErrorMessage = "Phần trăm giảm phải từ 0 đến 100.")]
        public decimal PhanTramGiam { get; set; }

        [Required]
        public int TrangThai { get; set; }

        [Required(ErrorMessage = "Số lượng là bắt buộc.")]
        [Range(1, int.MaxValue, ErrorMessage = "Số lượng phải lớn hơn 0.")]
        public int SoLuong { get; set; }

        [Required]
        public DateTime NgayTao { get; set; }

        public DateTime? NgayCapNhat { get; set; }

        [JsonIgnore]
        public virtual ICollection<HoaDon> HoaDons { get; set; } = new List<HoaDon>();

        public Voucher()
        {
            VoucherId = Guid.NewGuid();
            HoaDons = new List<HoaDon>();
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var _context = (AppDbContext)validationContext.GetService(typeof(AppDbContext));

            if (NgayKetThuc <= NgayBatDau)
            {
                yield return new ValidationResult(
                    "Ngày kết thúc phải lớn hơn ngày bắt đầu.",
                    new[] { nameof(NgayKetThuc) });
            }

            if (_context != null)
            {
                var isDuplicate = _context.Vouchers
                    .Any(v => v.TenVoucher.ToLower().Trim() == TenVoucher.ToLower().Trim()
                           && v.VoucherId != VoucherId);

                if (isDuplicate)
                {
                    yield return new ValidationResult(
                        "Tên voucher đã tồn tại.",
                        new[] { nameof(TenVoucher) });
                }
            }
        }
    }
}