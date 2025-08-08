using FurryFriends.API.Data;
using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic; // Added for List

namespace FurryFriends.API.Models.DTO
{
    public class ThuongHieuDTO : IValidatableObject
    {
        public Guid ThuongHieuId { get; set; }

        [Required(ErrorMessage = "Tên thương hiệu là bắt buộc.")]
        [StringLength(50, ErrorMessage = "Tên tối đa 50 ký tự.")]
        [RegularExpression(@"^[a-zA-Z0-9\s]+$", ErrorMessage = "Tên không được chứa ký tự đặc biệt.")]
        public string? TenThuongHieu { get; set; }

        [Required(ErrorMessage = "Email là bắt buộc")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Số điện thoại là bắt buộc")]
        [RegularExpression(@"^0\d{9,10}$", ErrorMessage = "Số điện thoại phải bắt đầu bằng số 0 và gồm 10 đến 11 chữ số.")]
        public string SDT { get; set; }

        public string? DiaChi { get; set; }
        public string? MoTa { get; set; }

        [Required(ErrorMessage = "Trạng thái là bắt buộc.")]
        public bool TrangThai { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var results = new List<ValidationResult>();
            
            try
            {
                var _context = (AppDbContext)validationContext.GetService(typeof(AppDbContext));

                if (_context != null && TenThuongHieu != null)
                {
                    var isDuplicate = _context.ThuongHieus
                        .Any(x => x.TenThuongHieu.ToLower().Trim() == TenThuongHieu.ToLower().Trim()
                               && x.ThuongHieuId != ThuongHieuId);

                    if (isDuplicate)
                    {
                        results.Add(new ValidationResult("Tên thương hiệu đã tồn tại.", new[] { nameof(TenThuongHieu) }));
                    }
                }
            }
            catch
            {
                // If database context is not available, skip duplicate validation
                // This will be handled by the service layer
            }
            
            return results;
        }
    }
}