using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FurryFriends.API.Models
{
    public class ChucVuTaiKhoan
    {
        [Key]
        public Guid ChucVuTaiKhoanId { get; set; }

        [Required]
        public Guid ChucVuId { get; set; }

        [Required]
        public Guid TaiKhoanId { get; set; }

        [ForeignKey("ChucVuId")]
        public virtual ChucVu ChucVu { get; set; }

        [ForeignKey("TaiKhoanId")]
        public virtual TaiKhoan TaiKhoan { get; set; }
    }

}
