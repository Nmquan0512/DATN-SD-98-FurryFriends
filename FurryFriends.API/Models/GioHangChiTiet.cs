using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FurryFriends.API.Models
{
	public class GioHangChiTiet
	{
		[Key]
		public Guid GioHangChiTietId { get; set; }

		[Required]
		public Guid GioHangId { get; set; }

		[Required]
		public Guid SanPhamChiTietId { get; set; } // Đổi từ SanPhamId thành SanPhamChiTietId

		[Required]
		public int SoLuong { get; set; }

		[Required]
		public decimal DonGia { get; set; }

		[Required]
		public decimal ThanhTien { get; set; }

		[Required]
		public bool TrangThai { get; set; }

		[Required]
		public DateTime NgayTao { get; set; }

		public DateTime? NgayCapNhat { get; set; }

		[ForeignKey("GioHangId")]
		public virtual GioHang GioHang { get; set; }

		[ForeignKey("SanPhamChiTietId")]
		public virtual SanPhamChiTiet SanPhamChiTiet { get; set; }
	}
}