namespace FurryFriends.Web.ViewModels
{
	public class PhieuHoanTraViewModel
	{
		public Guid PhieuHoanTraId { get; set; }
		public Guid HoaDonChiTietId { get; set; }
		public int SoLuongHoan { get; set; }
		public DateTime NgayHoanTra { get; set; }
		public string? LyDoHoanTra { get; set; }
		public int TrangThai { get; set; }

		// Có thể thêm trường hiển thị phụ
		public string? TenSanPham { get; set; }
		public int SoLuongTrongHoaDon { get; set; }
	}
}
