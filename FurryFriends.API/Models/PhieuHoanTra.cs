using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace FurryFriends.API.Models
{
	public class PhieuHoanTra
	{
		[Key]
		public Guid PhieuHoanTraId { get; set; }

		[Required]
		public Guid HoaDonChiTietId { get; set; } // liên kết với sản phẩm trong hóa đơn

		[Required]
		[Range(1, int.MaxValue, ErrorMessage = "Số lượng hoàn phải lớn hơn 0")]
		public int SoLuongHoan { get; set; }

		[Required]
		public DateTime NgayHoanTra { get; set; } = DateTime.Now;

		public string? LyDoHoanTra { get; set; }

		/// <summary>
		/// 0 = Yêu cầu
		/// 1 = Đã duyệt
		/// 2 = Từ chối
		/// 3 = Đã hoàn tiền / đổi hàng
		/// </summary>
		public int TrangThai { get; set; } = 0;

		[ForeignKey("HoaDonChiTietId")]
		public virtual HoaDonChiTiet HoaDonChiTiet { get; set; }
	}
}
