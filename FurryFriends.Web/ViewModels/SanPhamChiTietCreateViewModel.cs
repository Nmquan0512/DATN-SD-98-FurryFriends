using FurryFriends.API.Models.DTO;
using FurryFriends.API.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace FurryFriends.Web.ViewModels
{
    public class SanPhamChiTietCreateViewModel
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
        
        public Guid? AnhId { get; set; }
        public string? DuongDan { get; set; }
        public string MoTa { get; set; } = string.Empty;
        public int TrangThai { get; set; } = 1;
    }
}
