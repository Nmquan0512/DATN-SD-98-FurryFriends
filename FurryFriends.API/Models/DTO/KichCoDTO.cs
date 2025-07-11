using FurryFriends.API.Data;
using System;
using System.ComponentModel.DataAnnotations;

namespace FurryFriends.API.Models.DTO
{
    public class KichCoDTO : IValidatableObject
    {
        public Guid KichCoId { get; set; }

        [Required]
        [StringLength(50)]
        [RegularExpression(@"^[a-zA-ZÀÁÂÃÈÉÊÌÍÒÓÔÕÙÚĂĐĨŨƠƯàáâãèéêìíòóôõùúăđĩũơưẠ-ỹ\s0-9]+$", ErrorMessage = "Tên chỉ được chứa chữ cái tiếng Việt, số và khoảng trắng")]
        public string TenKichCo { get; set; }

        public string MoTa { get; set; }
        public bool TrangThai { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var _context = (AppDbContext)validationContext.GetService(typeof(AppDbContext));

            if (_context != null)
            {
                var isDuplicate = _context.KichCos
                    .Any(x => x.TenKichCo.ToLower().Trim() == TenKichCo.ToLower().Trim()
                           && x.KichCoId != KichCoId);

                if (isDuplicate)
                {
                    yield return new ValidationResult("Tên kích cỡ đã tồn tại.", new[] { nameof(TenKichCo) });
                }
            }
        }
    }
}
