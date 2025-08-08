using FurryFriends.API.Data;
using System.ComponentModel.DataAnnotations;

namespace FurryFriends.API.Models.DTO
{
    public class MauSacDTO : IValidatableObject
    {
        public Guid MauSacId { get; set; }

        [Required(ErrorMessage = "Tên màu sắc là bắt buộc.")]
        [StringLength(50, ErrorMessage = "Tên màu tối đa 50 ký tự.")]
        public string TenMau { get; set; }

        [Required(ErrorMessage = "Mã màu không được để trống")]
        public string MaMau { get; set; }

        public string? MoTa { get; set; }

        public bool TrangThai { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var results = new List<ValidationResult>();

            // Kiểm tra tên không được null hoặc rỗng
            if (string.IsNullOrWhiteSpace(TenMau))
            {
                results.Add(new ValidationResult("Tên màu sắc là bắt buộc.", new[] { nameof(TenMau) }));
                return results;
            }

            // Kiểm tra mã màu không được null hoặc rỗng
            if (string.IsNullOrWhiteSpace(MaMau))
            {
                results.Add(new ValidationResult("Mã màu không được để trống", new[] { nameof(MaMau) }));
                return results;
            }

            // Kiểm tra định dạng mã màu (kiểm tra chính xác 6 ký tự hex)
            if (!MaMau.StartsWith("#") || MaMau.Length != 7 || !System.Text.RegularExpressions.Regex.IsMatch(MaMau.Substring(1), @"^[0-9A-Fa-f]{6}$"))
            {
                results.Add(new ValidationResult("Mã màu không đúng định dạng", new[] { nameof(MaMau) }));
                return results;
            }

            // Kiểm tra trùng tên
            try
            {
                var _context = (AppDbContext)validationContext.GetService(typeof(AppDbContext));
                if (_context != null)
                {
                    var isDuplicate = _context.MauSacs
                        .Any(x => x.TenMau.ToLower().Trim() == TenMau.ToLower().Trim()
                               && x.MauSacId != MauSacId);

                    if (isDuplicate)
                    {
                        results.Add(new ValidationResult("Tên màu sắc đã tồn tại.", new[] { nameof(TenMau) }));
                    }
                }
            }
            catch
            {
                // Nếu không thể truy cập database, bỏ qua validation này
            }

            return results;
        }
    }
}
