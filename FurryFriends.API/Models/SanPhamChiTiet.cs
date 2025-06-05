using System.ComponentModel.DataAnnotations;

namespace FurryFriends.API.Models
{
    public class SanPhamChiTiet
	{
		[Key]
		public Guid SanPhamChiTietId { get; set; }
		[Required]
		public Guid SanPhamId { get; set; }
		[Required]
		public Guid KichCoId { get; set; }
		[Required]
		public Guid MauSacId { get; set; }
		[Required]
		public Guid? AnhSanPhamId { get; set; }
		[Required]
		public int SoLuong { get; set; }
		[Required]
		public decimal Gia { get; set; }
		
		public string MoTa { get; set; }
		[Required]
		public bool TrangThai { get; set; }
		[Required]
		public DateTime NgayTao { get; set; }
		
		public DateTime? NgaySua { get; set; }
		public virtual SanPham SanPham { get; set; }
	}
}
