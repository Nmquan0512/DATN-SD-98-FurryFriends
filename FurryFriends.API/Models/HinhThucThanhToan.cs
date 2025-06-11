using System.ComponentModel.DataAnnotations;

namespace FurryFriends.API.Models
{
	public class HinhThucThanhToan
	{
		[Key]
		public Guid HinhThucThanhToanId { get; set; }
		public string TenHinhThuc { get; set; }
		public string MoTa { get; set; }

		public virtual ICollection<HoaDon> HoaDons { get; set; }
	}
}