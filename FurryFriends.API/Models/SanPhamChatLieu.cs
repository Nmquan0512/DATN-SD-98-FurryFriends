using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FurryFriends.API.Models
{
    public class SanPhamChatLieu
    {
        [Key]
        public Guid SanPhamChatLieuId { get; set; }

        [Required]
        public Guid ChatLieuId { get; set; }

        [Required]
        public Guid SanPhamId { get; set; }

        [ForeignKey("ChatLieuId")]
        public virtual ChatLieu ChatLieu { get; set; }

        [ForeignKey("SanPhamId")]
        public virtual SanPham SanPham { get; set; }
    }
}