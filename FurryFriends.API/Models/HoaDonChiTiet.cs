using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace FurryFriends.API.Models
{
	public class HoaDonChiTiet
	{
		[Key]
		public Guid HoaDonChiTietId { get; set; }

		[Required]
		public Guid SanPhamChiTietId { get; set; } // Đổi từ SanPhamId thành SanPhamChiTietId

		[Required]
		public Guid HoaDonId { get; set; }

		[Required]
		public int SoLuongSanPham { get; set; }

		[Required]
		public decimal Gia { get; set; }

		[ForeignKey("HoaDonId")]
		public virtual HoaDon HoaDon { get; set; }

		[ForeignKey("SanPhamChiTietId")]
		public virtual SanPhamChiTiet SanPhamChiTiet { get; set; }
	}
}