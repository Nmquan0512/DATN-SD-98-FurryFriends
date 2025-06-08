using System.ComponentModel.DataAnnotations;

namespace FurryFriends.API.Models
{
	public class TaiKhoan
	{
		[Key]
		public Guid TaiKhoanId { get; set; }

		[Required]
		public string UserName { get; set; }

		[Required]
		public string Password { get; set; }

		[Required]
		public DateTime NgayTaoTaiKhoan { get; set; }

		public DateTime? NgayCapNhatCuoiCung { get; set; }

		[Required]
		public bool TrangThai { get; set; }
		

		public TaiKhoan()
		{
			TaiKhoanId = Guid.NewGuid();
			TrangThai = true;
		}

		public virtual NhanVien ? NhanVien { get; set; }
		public virtual ICollection<SanPham> SanPhams { get; set; }
		public virtual ICollection<Voucher> Vouchers { get; set; }
		public virtual ICollection<KhachHang> KhachHangs { get; set; }
		public virtual ICollection<HoaDon> HoaDons { get; set; }
		public virtual ICollection<GiamGia> GiamGias { get; set; }
	}
}