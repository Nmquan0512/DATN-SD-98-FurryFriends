using System;
using System.ComponentModel.DataAnnotations;

namespace FurryFriends.API.Models
{
    public class Notification
    {
        [Key]
        public Guid NotificationId { get; set; }
        [Required]
        public string Message { get; set; }
        public string Type { get; set; } // "Create", "Update", "Delete", ...
        public DateTime CreatedAt { get; set; }
        public bool IsRead { get; set; }
        public string UserName { get; set; } // hoặc UserId nếu muốn gắn với user cụ thể
    }
} 