using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace FurryFriends.API.Models
{
	public class HoaDon
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

		public string TenCuaKhachHang { get; set; }
		public string SdtCuaKhachHang { get; set; }
		public string EmailCuaKhachHang { get; set; }

		[Required]
		public DateTime NgayTao { get; set; }
		public DateTime? NgayNhanHang { get; set; }
		required
		public decimal TongTien { get; set; }	

		[Required]
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

		[Required]
		public Guid NhanVienId { get; set; } // Nhân viên tạo hóa đơn

		[ForeignKey("NhanVienId")]
		public virtual NhanVien NhanVien { get; set; }

		public string LoaiHoaDon { get; set; } // Loại hóa đơn (ví dụ: "BanTaiQuay", ...)

		public virtual ICollection<HoaDonChiTiet> HoaDonChiTiets { get; set; }
	}
}