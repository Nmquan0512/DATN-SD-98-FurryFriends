using FurryFriends.API.Data;
using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic; // Added for List
using System.Linq; // Added for Any

namespace FurryFriends.API.Models.DTO
{
    public class KichCoDTO : IValidatableObject
    {
        public Guid KichCoId { get; set; }

        [Required(ErrorMessage = "Tên kích cỡ không được để trống")]
        [StringLength(50, ErrorMessage = "Tên kích cỡ không được vượt quá 50 ký tự")]
        public string TenKichCo { get; set; }

        [StringLength(500, ErrorMessage = "Mô tả không được vượt quá 500 ký tự")]
        public string MoTa { get; set; }
        
        [Required(ErrorMessage = "Trạng thái là bắt buộc")]
        public bool TrangThai { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var results = new List<ValidationResult>();

            // Kiểm tra tên không được null hoặc rỗng
            if (string.IsNullOrWhiteSpace(TenKichCo))
            {
                results.Add(new ValidationResult("Tên kích cỡ không được để trống", new[] { nameof(TenKichCo) }));
                return results;
            }

            // Kiểm tra ký tự đặc biệt - chỉ từ chối một số ký tự đặc biệt nhất định
            if (TenKichCo.Contains("%") || TenKichCo.Contains("@") || TenKichCo.Contains("#") || 
                TenKichCo.Contains("$") || TenKichCo.Contains("!") || TenKichCo.Contains("_") ||
                TenKichCo.Contains("^") || TenKichCo.Contains("&") || TenKichCo.Contains("*") ||
                TenKichCo.Contains("(") || TenKichCo.Contains(")"))
            {
                results.Add(new ValidationResult("Tên kích cỡ không được chứa ký tự đặc biệt", new[] { nameof(TenKichCo) }));
                return results;
            }

            // Kiểm tra trùng tên (nếu có context)
            var _context = (AppDbContext)validationContext.GetService(typeof(AppDbContext));
            if (_context != null)
            {
                var isDuplicate = _context.KichCos
                    .Any(x => x.TenKichCo.ToLower().Trim() == TenKichCo.ToLower().Trim()
                           && x.KichCoId != KichCoId);

                if (isDuplicate)
                {
                    results.Add(new ValidationResult("Tên kích cỡ đã tồn tại", new[] { nameof(TenKichCo) }));
                }
            }

            return results;
        }
    }
}
