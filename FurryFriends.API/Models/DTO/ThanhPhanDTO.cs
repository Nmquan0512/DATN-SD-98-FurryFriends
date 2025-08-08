using FurryFriends.API.Data;
using System;
using System.ComponentModel.DataAnnotations;

namespace FurryFriends.API.Models.DTO
{
    public class ThanhPhanDTO : IValidatableObject
    {
        public Guid ThanhPhanId { get; set; }

        [Required(ErrorMessage = "Tên thành phần không được để trống")]
        [StringLength(255, ErrorMessage = "Tên thành phần không được vượt quá 255 ký tự")]
        public string TenThanhPhan { get; set; }

        [StringLength(1000, ErrorMessage = "Mô tả không được vượt quá 1000 ký tự")]
        public string MoTa { get; set; }

        [Required(ErrorMessage = "Trạng thái là bắt buộc")]
        public bool TrangThai { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var results = new List<ValidationResult>();

            // Kiểm tra tên không được null hoặc rỗng
            if (string.IsNullOrWhiteSpace(TenThanhPhan))
            {
                results.Add(new ValidationResult("Tên thành phần không được để trống", new[] { nameof(TenThanhPhan) }));
                return results;
            }

            // Kiểm tra ký tự đặc biệt - chỉ từ chối một số ký tự đặc biệt nhất định
            if (TenThanhPhan.Contains("%") || TenThanhPhan.Contains("@") || TenThanhPhan.Contains("#") || 
                TenThanhPhan.Contains("$") || TenThanhPhan.Contains("!") || TenThanhPhan.Contains("_"))
            {
                results.Add(new ValidationResult("Tên thành phần không được chứa ký tự đặc biệt", new[] { nameof(TenThanhPhan) }));
                return results;
            }

            // Kiểm tra trùng tên (nếu có context)
            var _context = (AppDbContext)validationContext.GetService(typeof(AppDbContext));
            if (_context != null)
            {
                var isDuplicate = _context.ThanhPhans
                    .Any(x => x.TenThanhPhan.ToLower().Trim() == TenThanhPhan.ToLower().Trim()
                           && x.ThanhPhanId != ThanhPhanId);

                if (isDuplicate)
                {
                    results.Add(new ValidationResult("Tên thành phần đã tồn tại", new[] { nameof(TenThanhPhan) }));
                }
            }

            return results;
        }
    }
}