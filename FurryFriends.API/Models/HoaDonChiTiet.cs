using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace FurryFriends.API.Models
{
	public class HoaDonChiTiet
	{
		[Key]
		public Guid HoaDonChiTietId { get; set; }

		[Required]
		public Guid SanPhamId { get; set; } 

		[Required]
		public Guid HoaDonId { get; set; }

		[Required]
		public int SoLuongSanPham { get; set; }

		[Required]
		public decimal Gia { get; set; }

		[ForeignKey("HoaDonId")]
		public virtual HoaDon HoaDon { get; set; }

		[ForeignKey("SanPhamId")]
		public virtual SanPham SanPham { get; set; }
	}
}