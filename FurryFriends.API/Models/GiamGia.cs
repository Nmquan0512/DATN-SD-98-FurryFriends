using FurryFriends.API.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.InteropServices;

namespace FurryFriends.API.Models
{
    public class GiamGia : IValidatableObject
    {
        [Key]
        public Guid GiamGiaId { get; set; }


        [Required(ErrorMessage = "Tên giảm giá không được để trống")]
        [StringLength(100, ErrorMessage = "Tên giảm giá không được vượt quá 100 ký tự")]
        public string TenGiamGia { get; set; }


        [Required(ErrorMessage = "Phần trăm khuyến mãi không được để trống")]
        [Range(0, 100, ErrorMessage = "Phần trăm khuyến mãi phải từ 0 đến 100")]
        public decimal PhanTramKhuyenMai { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn ngày bắt đầu")]
        public DateTime? NgayBatDau { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn ngày kết thúc")]
        public DateTime? NgayKetThuc { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn trạng thái")]
        public bool TrangThai { get; set; }

        [Required]
        public DateTime NgayTao { get; set; }

        [Required]
        public DateTime NgayCapNhat { get; set; }

        public virtual ICollection<DotGiamGiaSanPham>? DotGiamGiaSanPhams { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var _context = (AppDbContext)validationContext.GetService(typeof(AppDbContext));

            // Kiểm tra trùng tên giảm giá (trừ trường hợp đang cập nhật chính nó)
            if (_context != null)
            {
                var isDuplicate = _context.GiamGias
                    .Any(x => x.TenGiamGia.ToLower().Trim() == TenGiamGia.ToLower().Trim()
                           && x.GiamGiaId != GiamGiaId);

                if (isDuplicate)
                {
                    yield return new ValidationResult(
                        "Tên giảm giá đã tồn tại.",
                        new[] { nameof(TenGiamGia) });
                }
            }

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