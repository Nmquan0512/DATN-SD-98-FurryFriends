using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace FurryFriends.API.Models
{
	public class Voucher
	{
		[Key]
		public Guid VoucherId { get; set; }

		[Required]
		public string TenVoucher { get; set; }

		[Required]
		public DateTime NgayBatDau { get; set; }

		[Required]
		public DateTime NgayKetThuc { get; set; }
		[Range(0, 1)]
		public decimal PhanTramGiam { get; set; }

		[Required]
		public int TrangThai { get; set; }

		[Required]
		public int SoLuong { get; set; }

		[Required]
		public Guid TaiKhoanId { get; set; }

		[Required]
		public DateTime NgayTao { get; set; }

		public DateTime? NgayCapNhat { get; set; }

		[ForeignKey("TaiKhoanId")]
		public virtual TaiKhoan TaiKhoan { get; set; }

		public virtual ICollection<HoaDon> HoaDons { get; set; }
	}
}