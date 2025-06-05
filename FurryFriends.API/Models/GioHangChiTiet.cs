using System.ComponentModel.DataAnnotations;

namespace FurryFriends.API.Models
{
    public class GioHangChiTiet
    {
		[Key]
		public Guid GioHangChiTietId { get; set; }
		[Required]
		public Guid GioHangId { get; set; }
		[Required]
		public Guid SanPhamId { get; set; }
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
		public virtual GioHang GioHang { get; set; }
		public virtual SanPham SanPham { get; set; }
	}
}
