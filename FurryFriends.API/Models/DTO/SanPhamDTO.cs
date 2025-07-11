using FurryFriends.API.Data;
using System;
using System.ComponentModel.DataAnnotations;

namespace FurryFriends.API.Models.DTO
{
    public class SanPhamDTO : IValidatableObject
    {
        public Guid SanPhamId { get; set; }

        [Required(ErrorMessage = "Tên sản phẩm là bắt buộc.")]
        public string TenSanPham { get; set; }

        [Required(ErrorMessage = "Tài khoản là bắt buộc.")]
        public Guid TaiKhoanId { get; set; }

        [Required(ErrorMessage = "Thương hiệu là bắt buộc.")]
        public Guid ThuongHieuId { get; set; }

        [Required(ErrorMessage = "Trạng thái là bắt buộc.")]
        public bool TrangThai { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var _context = (AppDbContext)validationContext.GetService(typeof(AppDbContext));

            if (_context != null)
            {
                var isDuplicate = _context.SanPhams
                    .Any(x => x.TenSanPham.ToLower().Trim() == TenSanPham.ToLower().Trim()
                           && x.SanPhamId != SanPhamId);

                if (isDuplicate)
                {
                    yield return new ValidationResult("Tên sản phẩm đã tồn tại.", new[] { nameof(TenSanPham) });
                }
            }
        }
    }
}