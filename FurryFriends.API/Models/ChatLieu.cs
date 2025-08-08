using System.ComponentModel.DataAnnotations;

namespace FurryFriends.API.Models
{
    public class ChatLieu
    {
        [Key]
        public Guid ChatLieuId { get; set; }

        [Required]
        [StringLength(100)]
        public string TenChatLieu { get; set; } = string.Empty;

        public string? MoTa { get; set; }

        [Required]
        public bool TrangThai { get; set; }

        public virtual ICollection<SanPhamChatLieu> SanPhamChatLieus { get; set; } = new List<SanPhamChatLieu>();
    }
}