using System;
using System.ComponentModel.DataAnnotations;

namespace FurryFriends.API.Models.DTO
{
    public class AnhDTO
    {
        public Guid AnhId { get; set; }

        [Required(ErrorMessage = "Đường dẫn ảnh là bắt buộc.")]
        [StringLength(250, ErrorMessage = "Đường dẫn tối đa 250 ký tự.")]
        public string DuongDan { get; set; }

        [Required(ErrorMessage = "Tên ảnh là bắt buộc.")]
        [StringLength(100, ErrorMessage = "Tên ảnh tối đa 100 ký tự.")]
        public string TenAnh { get; set; }

        public bool TrangThai { get; set; } = true;
    }
}