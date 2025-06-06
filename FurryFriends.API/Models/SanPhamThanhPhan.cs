using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FurryFriends.API.Models
{
    public class SanPhamThanhPhan
    {
        [Key]
        public Guid SanPhamThanhPhanId { get; set; }

        [Required]
        public Guid ThanhPhanId { get; set; }

        [Required]
        public Guid SanPhamId { get; set; }

        [ForeignKey("ThanhPhanId")]
        public virtual ThanhPhan ThanhPhan { get; set; }

        [ForeignKey("SanPhamId")]
        public virtual SanPham SanPham { get; set; }
    }
}