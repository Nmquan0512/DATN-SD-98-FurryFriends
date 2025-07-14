using System.Collections.Generic;

namespace FurryFriends.Web.Models
{
	public class ThongTinCaNhanViewModel
	{
		public Guid TaiKhoanId { get; set; }

		public string UserName { get; set; }

		public string? HoTen { get; set; }

		public string? Email { get; set; }

		public string? SoDienThoai { get; set; }

		public DateTime? NgaySinh { get; set; } // Chỉ dành cho Nhân viên

		public string? GioiTinh { get; set; }    // Chỉ dành cho Nhân viên

		// public string? DiaChi { get; set; }
		public List<DiaChiKhachHangViewModel> DiaChis { get; set; } = new();

		public DiaChiKhachHangViewModel? DiaChiChinh { get; set; }

		public string? Role { get; set; }

		// Phục vụ đổi mật khẩu
		public string? MatKhauCu { get; set; }

		public string? MatKhauMoi { get; set; }

		public string? XacNhanMatKhauMoi { get; set; }
	}
}
