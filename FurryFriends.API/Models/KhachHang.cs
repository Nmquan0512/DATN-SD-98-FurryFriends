using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FurryFriends.API.Models
{
	public class KhachHang
	{
		[Key]
		public Guid KhachHangId { get; set; }

		[Required]
		[MaxLength(100)]
		public string TenKhachHang { get; set; }

		[MaxLength(20)]
		public string SDT { get; set; }

		public int? DiemKhachHang { get; set; }

		[MaxLength(100)]
		public string EmailCuaKhachHang { get; set; }

		[Required]
		public DateTime NgayTaoTaiKhoan { get; set; }

		public DateTime? NgayCapNhatCuoiCung { get; set; }

		[Required]
		public int TrangThai { get; set; }

		[Required]
		public Guid TaiKhoanId { get; set; }

		public KhachHang()
		{
			KhachHangId = Guid.NewGuid();
		}

		[ForeignKey("TaiKhoanId")]
		public virtual TaiKhoan TaiKhoan { get; set; }

		public virtual ICollection<DiaChiKhachHang> DiaChiKhachHangs { get; set; }
		public virtual ICollection<HoaDon> HoaDons { get; set; }
		public virtual ICollection<GioHang> GioHangs { get; set; }
	}
}