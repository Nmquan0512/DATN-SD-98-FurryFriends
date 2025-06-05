using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FurryFriends.API.Models
{
	public class DiaChiKhachHang
	{
		[Key]
		public Guid DiaChiId { get; set; }

		[MaxLength(200)]
		public string TenDiaChi { get; set; }

		[MaxLength(50)]
		public string ThanhPho { get; set; }

		[MaxLength(50)]
		public string QuanHuyen { get; set; }

		[MaxLength(50)]
		public string PhuongXa { get; set; }

		public string MoTa { get; set; }
		public int TrangThai { get; set; }

		[Required]
		public DateTime NgayTao { get; set; }

		[Required]
		public DateTime NgayCapNhat { get; set; }

		// FK tới KhachHang
		[Required]
		public Guid KhachHangId { get; set; }

		[ForeignKey("KhachHangId")]
		public virtual KhachHang KhachHang { get; set; }
	}
}