using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FurryFriends.API.Models
{
    public class DotGiamGiaSanPham
    {
        [Key]
        public Guid DotGiamGiaSanPhamId { get; set; }

        [Required]
        public Guid GiamGiaId { get; set; }

        [Required]
        public Guid SanPhamId { get; set; }

        [Range(0, 100)]
        public double PhanTramGiamGia { get; set; }

        [ForeignKey("GiamGiaId")]
        public virtual GiamGia GiamGia { get; set; }

        [ForeignKey("SanPhamId")]
        public virtual SanPham SanPham { get; set; }
    }

}
