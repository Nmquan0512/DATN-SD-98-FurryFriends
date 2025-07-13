using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace FurryFriends.API.Models
{
    public class HoaDon : IValidatableObject
    {
        [Key]
        public Guid HoaDonId { get; set; }

        [Required]
        public Guid TaiKhoanId { get; set; }

        public Guid? VoucherId { get; set; }

        [Required]
        public Guid KhachHangId { get; set; }

        [Required]
        public Guid HinhThucThanhToanId { get; set; }

        //[Required(ErrorMessage = "Tên khách hàng không được để trống")]
        //[StringLength(100, ErrorMessage = "Tên khách hàng tối đa 100 ký tự")]
        public string TenCuaKhachHang { get; set; }
        //[Required(ErrorMessage = "Số điện thoại không được để trống")]
        //[Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        public string SdtCuaKhachHang { get; set; }
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public string EmailCuaKhachHang { get; set; }

        [Required]
        public DateTime NgayTao { get; set; }
        public DateTime? NgayNhanHang { get; set; }
        [Required(ErrorMessage = "Tổng tiền là bắt buộc")]
        [Range(0, double.MaxValue, ErrorMessage = "Tổng tiền không được âm")]
        public decimal TongTien { get; set; }

        [Required(ErrorMessage = "Tổng tiền sau khi giảm là bắt buộc")]
        [Range(0, double.MaxValue, ErrorMessage = "Tổng tiền sau khi giảm không được âm")]
        public decimal TongTienSauKhiGiam { get; set; }

        [Required]
        public int TrangThai { get; set; }

        public string GhiChu { get; set; }

        [ForeignKey("VoucherId")]
        public virtual Voucher Voucher { get; set; }

        [ForeignKey("TaiKhoanId")]
        public virtual TaiKhoan TaiKhoan { get; set; }

        [ForeignKey("KhachHangId")]
        public virtual KhachHang KhachHang { get; set; }

        [ForeignKey("HinhThucThanhToanId")]
        public virtual HinhThucThanhToan HinhThucThanhToan { get; set; }

<<<<<<< HEAD
		[Required]
		public Guid NhanVienId { get; set; } // Nhân viên tạo hóa đơn

		[ForeignKey("NhanVienId")]
		public virtual NhanVien NhanVien { get; set; }

		public string LoaiHoaDon { get; set; } // Loại hóa đơn (ví dụ: "BanTaiQuay", ...)

		public virtual ICollection<HoaDonChiTiet> HoaDonChiTiets { get; set; }
	}
=======
        public virtual ICollection<HoaDonChiTiet> HoaDonChiTiets { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (TongTienSauKhiGiam > TongTien)
            {
                yield return new ValidationResult(
                    "Tổng tiền sau khi giảm không được lớn hơn tổng tiền.",
                    new[] { nameof(TongTienSauKhiGiam) });
            }

            if (NgayNhanHang.HasValue && NgayNhanHang.Value < NgayTao)
            {
                yield return new ValidationResult(
                    "Ngày nhận hàng không được trước ngày tạo.",
                    new[] { nameof(NgayNhanHang) });
            }
        }
    }
>>>>>>> origin/TruongValidate
}