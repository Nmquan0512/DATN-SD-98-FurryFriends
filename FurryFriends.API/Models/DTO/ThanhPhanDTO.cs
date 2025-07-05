using System;
using System.ComponentModel.DataAnnotations;

namespace FurryFriends.API.Models.DTO
{
    public class ThanhPhanDTO
    {
        public Guid ThanhPhanId { get; set; }

        [Required(ErrorMessage = "Tên thành phần là bắt buộc.")]
        [StringLength(100, ErrorMessage = "Tên tối đa 100 ký tự.")]
        public string TenThanhPhan { get; set; }

        public string MoTa { get; set; }

        [Required(ErrorMessage = "Trạng thái là bắt buộc.")]
        public bool TrangThai { get; set; }
    }
}