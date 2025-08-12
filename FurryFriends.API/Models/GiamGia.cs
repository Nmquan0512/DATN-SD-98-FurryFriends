using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FurryFriends.API.Models
{
    public class GiamGia
    {
        [Key]
        public Guid GiamGiaId { get; set; } = Guid.NewGuid();

        [Required(ErrorMessage = "Tên giảm giá không được để trống")]
        [StringLength(100, ErrorMessage = "Tên giảm giá không được vượt quá 100 ký tự")]
        public string TenGiamGia { get; set; }

        [Required(ErrorMessage = "Phần trăm khuyến mãi không được để trống")]
        [Range(0, 100, ErrorMessage = "Phần trăm khuyến mãi phải từ 0 đến 100")]
        public decimal PhanTramKhuyenMai { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn ngày bắt đầu")]
        public DateTime NgayBatDau { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn ngày kết thúc")]
        public DateTime NgayKetThuc { get; set; }

        [Required]
        public bool TrangThai { get; set; } = true;

        [Required]
        public DateTime NgayTao { get; set; } = DateTime.UtcNow;

        [Required]
        public DateTime NgayCapNhat { get; set; } = DateTime.UtcNow;

        // Navigation property
        public virtual ICollection<DotGiamGiaSanPham> DotGiamGiaSanPhams { get; set; } = new List<DotGiamGiaSanPham>();

        // Validation logic
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (NgayKetThuc <= NgayBatDau)
            {
                yield return new ValidationResult(
                    "Ngày kết thúc phải sau ngày bắt đầu.",
                    new[] { nameof(NgayKetThuc) });
            }

            if (NgayBatDau < DateTime.Today)
            {
                yield return new ValidationResult(
                    "Ngày bắt đầu không được trong quá khứ.",
                    new[] { nameof(NgayBatDau) });
            }
        }
    }
}