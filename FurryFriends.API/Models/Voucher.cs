using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace FurryFriends.API.Models
{
	public class Voucher
	{
		[Key]
		public Guid VoucherId { get; set; }

		[Required]
		public string TenVoucher { get; set; }

		[Required]
		public DateTime NgayBatDau { get; set; }

		[Required]
		public DateTime NgayKetThuc { get; set; }
		[Range(0, 100)]
		public decimal PhanTramGiam { get; set; }

		[Required]
		public int TrangThai { get; set; }

		[Required]
		public int SoLuong { get; set; }

		[Required]
		public DateTime NgayTao { get; set; }

		public DateTime? NgayCapNhat { get; set; }

		[JsonIgnore]
		public virtual ICollection<HoaDon> HoaDons { get; set; } = new List<HoaDon>();

		public Voucher()
		{
			VoucherId = Guid.NewGuid();
			HoaDons = new List<HoaDon>();
		}
	}
}