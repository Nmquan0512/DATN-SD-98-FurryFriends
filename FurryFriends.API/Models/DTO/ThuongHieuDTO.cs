using System;
using System.ComponentModel.DataAnnotations;

namespace FurryFriends.API.Models.DTO
{
    public class ThuongHieuDTO
    {
        public Guid ThuongHieuId { get; set; }

        [Required(ErrorMessage = "Tên thương hiệu là bắt buộc.")]
        [StringLength(50, ErrorMessage = "Tên tối đa 50 ký tự.")]
        [RegularExpression(@"^[a-zA-Z0-9\s]+$", ErrorMessage = "Tên không được chứa ký tự đặc biệt.")]
        public string TenThuongHieu { get; set; }

        [EmailAddress(ErrorMessage = "Email không hợp lệ.")]
        public string Email { get; set; }

        [Phone(ErrorMessage = "Số điện thoại không hợp lệ.")]
        public string SDT { get; set; }

        public string DiaChi { get; set; }
        public string MoTa { get; set; }

        [Required(ErrorMessage = "Trạng thái là bắt buộc.")]
        public bool TrangThai { get; set; }
    }
}