using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FurryFriends.API.Models
{
	public class ChucVu
	{
		[Key]
		public Guid ChucVuId { get; set; }

		[Required]
		[StringLength(50)]
		public string TenChucVu { get; set; }

		[Required]
		[StringLength(250)]
		public string MoTaChucVu { get; set; }

		[Required]
		public bool TrangThai { get; set; }

		[Required]
		public DateTime NgayTao { get; set; }

		[Required]
		public DateTime NgayCapNhat { get; set; }

		public virtual ICollection<NhanVien> NhanViens { get; set; }
	}

}
