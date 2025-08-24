using FurryFriends.API.Models.DTO;
using System.ComponentModel.DataAnnotations;

namespace FurryFriends.Web.ViewModels
{
    public class SanPhamChiTietCreateViewModel : IValidatableObject
    {
        public Guid? SanPhamChiTietId { get; set; }
        public Guid SanPhamId { get; set; }
        
        [Required(ErrorMessage = "Vui lòng chọn màu sắc")]
        public Guid MauSacId { get; set; }
        
        [Required(ErrorMessage = "Vui lòng chọn kích cỡ")]
        public Guid KichCoId { get; set; }
        
        [Required(ErrorMessage = "Vui lòng nhập số lượng")]
        [Range(0, int.MaxValue, ErrorMessage = "Số lượng phải lớn hơn hoặc bằng 0")]
        public int SoLuongTon { get; set; }
        
        [Required(ErrorMessage = "Vui lòng nhập giá bán")]
        [Range(0, double.MaxValue, ErrorMessage = "Giá bán phải lớn hơn hoặc bằng 0")]
        public decimal GiaBan { get; set; }

        // ✅ Thêm trường giá nhập (nullable)
        [Range(0, double.MaxValue, ErrorMessage = "Giá nhập phải lớn hơn hoặc bằng 0")]
        public decimal? GiaNhap { get; set; }
        
        public Guid? AnhId { get; set; }
        public string? DuongDan { get; set; }
        public string MoTa { get; set; } = string.Empty;
        public int TrangThai { get; set; } = 1;

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var results = new List<ValidationResult>();

            // ✅ Thêm validation: Giá nhập không được lớn hơn giá bán
            if (GiaNhap.HasValue && GiaNhap.Value > GiaBan)
            {
                results.Add(new ValidationResult("Giá nhập không được lớn hơn giá bán", new[] { nameof(GiaNhap) }));
            }

            return results;
        }
    }
}
