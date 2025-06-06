using System.ComponentModel.DataAnnotations;

namespace FurryFriends.API.Models
{
    public class ThanhPhan
    {
        [Key]
        public Guid ThanhPhanId { get; set; }

        [Required]
        [StringLength(100)]
        public string TenThanhPhan { get; set; }

        public string MoTa { get; set; }

        [Required]
        public bool TrangThai { get; set; }

        public virtual ICollection<SanPhamThanhPhan> SanPhamThanhPhans { get; set; }
    }
}
