using System.ComponentModel.DataAnnotations;

namespace FurryFriends.API.Models
{
	public class Anh
	{
		[Key]
		
		public Guid AnhId { get; set; }
        public Guid SanPhamChiTietId { get; set; }
        public string DuongDan { get; set; }
		public string TenAnh { get; set; }
		public bool TrangThai { get; set; }

		public Anh()
		{
			AnhId = Guid.NewGuid();
			TrangThai = true;
		}

		public virtual ICollection<SanPhamChiTiet> SanPhamChiTiets { get; set; }
	}

}