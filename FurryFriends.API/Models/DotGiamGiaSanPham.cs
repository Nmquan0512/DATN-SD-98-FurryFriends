using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FurryFriends.API.Models
{
	public class DotGiamGiaSanPham
	{
		[Key]
		public Guid DotGiamGiaSanPhamId { get; set; }

		[Required]
		public Guid GiamGiaId { get; set; }

		[Required]
		public Guid SanPhamId { get; set; }

		[Required]
		public decimal PhanTramGiamGia { get; set; }

		[Required]
		public bool TrangThai { get; set; }

		[Required]
		public DateTime NgayTao { get; set; }

		[Required]
		public DateTime NgayCapNhat { get; set; }

		[ForeignKey("GiamGiaId")]
		public virtual GiamGia GiamGias { get; set; }

		[ForeignKey("SanPhamId")]
		public virtual SanPham SanPhams { get; set; }
	}

}
