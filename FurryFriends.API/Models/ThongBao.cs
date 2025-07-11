using System;
using System.ComponentModel.DataAnnotations;

namespace FurryFriends.API.Models
{
    public class ThongBao
    {
        [Key]
        public Guid ThongBaoId { get; set; }
        [Required]
        public string NoiDung { get; set; }
        public string Loai { get; set; } // "Create", "Update", "Delete", ...
        public DateTime NgayTao { get; set; }
        public bool DaDoc { get; set; }
        public string UserName { get; set; } // hoặc UserId nếu muốn gắn với user cụ thể
    }
} 