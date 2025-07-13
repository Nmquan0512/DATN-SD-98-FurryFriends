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

        [Required(ErrorMessage = "Họ và tên không được để trống.")]
        [StringLength(50, ErrorMessage = "Họ và tên tối đa 50 ký tự.")]
        public string HoVaTen { get; set; }

        [Required]
        public DateTime NgaySinh { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "Địa chỉ tối đa 100 ký tự.")]
        public string DiaChi { get; set; }

        [Required(ErrorMessage = "Số điện thoại là bắt buộc.")]
        [RegularExpression(@"^0\d{9,10}$", ErrorMessage = "Số điện thoại phải bắt đầu bằng số 0 và có từ 10 đến 11 chữ số.")]
        public string SDT { get; set; }

        [Required]
        [EmailAddress(ErrorMessage = "Email không hợp lệ.")]
        [StringLength(100)]
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