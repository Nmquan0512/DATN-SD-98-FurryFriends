using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.InteropServices;

namespace FurryFriends.API.Models
{
	public class GiamGia
	{
		[Key]
		public Guid GiamGiaId { get; set; }

		[Required]
		[StringLength(100)]
		public string TenGiamGia { get; set; }

		public string SanPhamApDung { get; set; }

		[Required]
		[Range(0, 100)]
		public decimal PhanTramKhuyenMai { get; set; }

		[Required]
		public DateTime NgayBatDau { get; set; }

		[Required]
		public DateTime NgayKetThuc { get; set; }

		[Required]
		public bool TrangThai { get; set; }

		[Required]
		public DateTime NgayTao { get; set; }

		[Required]
		public DateTime NgayCapNhat { get; set; }

		public virtual ICollection<DotGiamGiaSanPham> DotGiamGiaSanPhams { get; set; }
	}

}
