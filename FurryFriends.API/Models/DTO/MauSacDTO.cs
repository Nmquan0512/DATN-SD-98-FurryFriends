using FurryFriends.API.Data;
using System.ComponentModel.DataAnnotations;

namespace FurryFriends.API.Models.DTO
{
    public class MauSacDTO : IValidatableObject
    {
        public Guid MauSacId { get; set; }

        [Required]
        [StringLength(50)]
        public string TenMau { get; set; }

        [Required(ErrorMessage = "Mã màu là bắt buộc.")]
        public string MaMau { get; set; }

        public string MoTa { get; set; }

        public bool TrangThai { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var _context = (AppDbContext)validationContext.GetService(typeof(AppDbContext));

            if (_context != null)
            {
                var isDuplicate = _context.MauSacs
                    .Any(x => x.TenMau.ToLower().Trim() == TenMau.ToLower().Trim()
                           && x.MauSacId != MauSacId);

                if (isDuplicate)
                {
                    yield return new ValidationResult("Tên màu sắc đã tồn tại.", new[] { nameof(TenMau) });
                }
            }
        }
    }
}
