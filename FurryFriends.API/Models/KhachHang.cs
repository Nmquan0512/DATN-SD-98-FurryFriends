using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FurryFriends.API.Models
{
    public class KhachHang
    {
        [Key]
        public Guid KhachHangId { get; set; }

        [Required(ErrorMessage = "Tên khách hàng không được để trống.")]
        [MaxLength(100, ErrorMessage = "Tên khách hàng không được vượt quá 100 ký tự.")]
        public string TenKhachHang { get; set; }

        [MaxLength(20, ErrorMessage = "Số điện thoại không được vượt quá 20 ký tự.")]
        [RegularExpression(@"^0\d{8,10}$", ErrorMessage = "Số điện thoại phải bắt đầu bằng số 0 và có từ 9 đến 11 chữ số.")]
        public string SDT { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Điểm khách hàng không hợp lệ.")]
        public int? DiemKhachHang { get; set; }

        [MaxLength(100)]
        [EmailAddress(ErrorMessage = "Email không hợp lệ.")]
        public string EmailCuaKhachHang { get; set; }

        [Required]
        public DateTime NgayTaoTaiKhoan { get; set; }

        public DateTime? NgayCapNhatCuoiCung { get; set; }

        [Required]
        public int TrangThai { get; set; }


        public Guid? TaiKhoanId { get; set; }

        public KhachHang()
        {
            KhachHangId = Guid.NewGuid();
        }

        public virtual ICollection<TaiKhoan>? TaiKhoans { get; set; }
        public virtual ICollection<DiaChiKhachHang>? DiaChiKhachHangs { get; set; }
        public virtual ICollection<HoaDon>? HoaDons { get; set; }
        public virtual ICollection<GioHang>? GioHangs { get; set; }
    }

}