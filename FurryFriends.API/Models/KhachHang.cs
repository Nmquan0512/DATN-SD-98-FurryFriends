using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FurryFriends.API.Models
{
    public class KhachHang
    {
		[Key]
		public Guid KhachHangId { get; set; }

		[Required, MaxLength(100)]
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

		public virtual ICollection<TaiKhoan> TaiKhoans { get; set; } = new List<TaiKhoan>();
		public virtual ICollection<DiaChiKhachHang> DiaChiKhachHangs { get; set; } = new List<DiaChiKhachHang>();
		public virtual ICollection<HoaDon> HoaDons { get; set; } = new List<HoaDon>();
		public virtual ICollection<GioHang> GioHangs { get; set; } = new List<GioHang>();

		public KhachHang()
		{
			KhachHangId = Guid.NewGuid();
		}
	}
}