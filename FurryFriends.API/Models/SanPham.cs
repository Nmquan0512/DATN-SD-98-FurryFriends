using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace FurryFriends.API.Models
{
    public class SanPham
    {
        [Key]
        public Guid SanPhamId { get; set; }

        [Required]
        public string TenSanPham { get; set; }
        public SanPham()
        {
            SanPhamChiTiets = new HashSet<SanPhamChiTiet>();
            SanPhamThanhPhans = new HashSet<SanPhamThanhPhan>();
            SanPhamChatLieus = new HashSet<SanPhamChatLieu>();
        }

        //[Required]
        public Guid? ThuongHieuId { get; set; }

        [Required]
        public bool TrangThai { get; set; }


        [ForeignKey("ThuongHieuId")]
        public virtual ThuongHieu? ThuongHieu { get; set; }

        // Navigation properties
        [JsonIgnore]
        public virtual ICollection<SanPhamChiTiet> SanPhamChiTiets { get; set; }
        public virtual ICollection<SanPhamThanhPhan> SanPhamThanhPhans { get; set; }
        public virtual ICollection<SanPhamChatLieu> SanPhamChatLieus { get; set; }


    }

}