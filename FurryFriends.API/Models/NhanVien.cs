using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace FurryFriends.API.Models
{

    public class NhanVien
    {
        [Key]
        public Guid NhanVienId { get; set; }

        public Guid? TaiKhoanId { get; set; }  // Cho phép null nếu không bắt buộc
        [ForeignKey("TaiKhoanId")]
        public virtual TaiKhoan? TaiKhoan { get; set; }

        [Required, StringLength(50)]
        public string HoVaTen { get; set; }

        [Required]
        public DateTime NgaySinh { get; set; }

        [Required, StringLength(100)]
        public string DiaChi { get; set; }

        [Required, StringLength(20)]
        public string SDT { get; set; }

        [Required, EmailAddress, StringLength(100)]
        public string Email { get; set; }

        [Required, StringLength(100)]
        public string GioiTinh { get; set; }

        [Required]
        public Guid ChucVuId { get; set; }

        [ForeignKey("ChucVuId")]
        public virtual ChucVu? ChucVu { get; set; }

        [Required]
        public bool TrangThai { get; set; }

        [Required]
        public DateTime NgayTao { get; set; }

        [Required]
        public DateTime NgayCapNhat { get; set; }
    }
}