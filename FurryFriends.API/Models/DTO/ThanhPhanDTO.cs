using FurryFriends.API.Data;
using System;
using System.ComponentModel.DataAnnotations;

namespace FurryFriends.API.Models.DTO
{
    public class ThanhPhanDTO : IValidatableObject
    {
        public Guid ThanhPhanId { get; set; }

        [Required(ErrorMessage = "Tên thành phần là bắt buộc.")]
        [StringLength(100, ErrorMessage = "Tên tối đa 100 ký tự.")]
        public string TenThanhPhan { get; set; }

        public string MoTa { get; set; }

        [Required(ErrorMessage = "Trạng thái là bắt buộc.")]
        public bool TrangThai { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var _context = (AppDbContext)validationContext.GetService(typeof(AppDbContext));

            if (_context != null)
            {
                var isDuplicate = _context.ThanhPhans
                    .Any(x => x.TenThanhPhan.ToLower().Trim() == TenThanhPhan.ToLower().Trim()
                           && x.ThanhPhanId != ThanhPhanId);

                if (isDuplicate)
                {
                    yield return new ValidationResult("Tên thành phần đã tồn tại.", new[] { nameof(TenThanhPhan) });
                }
            }
        }
    }
}