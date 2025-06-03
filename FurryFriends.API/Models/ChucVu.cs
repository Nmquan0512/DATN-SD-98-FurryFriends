using System.ComponentModel.DataAnnotations;

namespace FurryFriends.API.Models
{
    public class ChucVu
    {
        [Key]
        public Guid ChucVuId { get; set; }

        [Required]
        public string TenChucVu { get; set; }

        public string? MoTaChucVu { get; set; }

        public bool TrangThai { get; set; }

        public virtual ICollection<ChucVuTaiKhoan> ChucVuTaiKhoans { get; set; }
    }

}
