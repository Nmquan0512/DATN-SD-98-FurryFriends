using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FurryFriends.API.Models
{
    public class DotGiamGiaSanPham
    {
        [Key]
        public Guid DotGiamGiaSanPhamId { get; set; }

        public Guid GiamGiaId { get; set; }

        public Guid? SanPhamChiTietId { get; set; }

        [ForeignKey("GiamGiaId")]
        public virtual GiamGia GiamGia { get; set; }

        [ForeignKey("SanPhamChiTietId")]
        public virtual SanPhamChiTiet? SanPhamChiTiet { get; set; }
    }

}