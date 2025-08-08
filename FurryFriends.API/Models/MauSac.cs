using System.ComponentModel.DataAnnotations;

namespace FurryFriends.API.Models
{
    public class MauSac
    {
        [Key]
        public Guid MauSacId { get; set; }

        [Required]
        [StringLength(50)]
        [RegularExpression(@"^[a-zA-Z0-9\s]+$")]
        public string TenMau { get; set; }
        public string MaMau { get; set; }
        public string? MoTa { get; set; }
        public bool TrangThai { get; set; }

        public virtual ICollection<SanPhamChiTiet> SanPhamChiTiets { get; set; }
    }
}