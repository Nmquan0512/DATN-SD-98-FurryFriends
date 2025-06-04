using System.ComponentModel.DataAnnotations;

namespace FurryFriends.API.Models
{
    public class SanPham
    {
        [Key]
        public Guid SanPhamId { get; set; }

        [Required]
        public string TenSanPham { get; set; }

        public string? MoTa { get; set; }

        public bool TrangThai { get; set; }

        public virtual ICollection<DotGiamGiaSanPham> DotGiamGiaSanPhams { get; set; }
    }

}
