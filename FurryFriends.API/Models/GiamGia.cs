using System.ComponentModel.DataAnnotations;

namespace FurryFriends.API.Models
{
    public class GiamGia
    {
        [Key]
        public Guid GiamGiaId { get; set; }

        [Required]
        public string TenGiamGia { get; set; }

        public string? MoTa { get; set; }

        [Range(0, 100)]
        public double PhanTramKhuyenMai { get; set; }
        [Required]
        public DateTime NgayBatDau { get; set; }
        [Required]
        public DateTime NgayKetThuc { get; set; }

        public bool TrangThai { get; set; }

        public virtual ICollection<DotGiamGiaSanPham> DotGiamGiaSanPhams { get; set; }
    }


}
