using System;
using System.ComponentModel.DataAnnotations;

namespace FurryFriends.API.Models.DTO
{
    public class ChatLieuDTO
    {
        public Guid ChatLieuId { get; set; }

        [Required(ErrorMessage = "Tên chất liệu là bắt buộc.")]
        [StringLength(100, ErrorMessage = "Tên tối đa 100 ký tự.")]
        public string TenChatLieu { get; set; }

        public string MoTa { get; set; }

        [Required(ErrorMessage = "Trạng thái là bắt buộc.")]
        public bool TrangThai { get; set; }
    }
}