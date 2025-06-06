﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FurryFriends.API.Models
{
    public class GioHang
    {
		[Key]
		public Guid GioHangId { get; set; }
		[Required]
		public Guid KhachHangId { get; set; }

		[ForeignKey(nameof(KhachHangId))]
		public virtual KhachHang KhachHang { get; set; }
		[Required]
		public DateTime NgayTao { get; set; }
		public DateTime? NgayCapNhat { get; set; }
		[Required]
		public bool TrangThai { get; set; }

		// Một giỏ hàng có nhiều chi tiết

		public virtual ICollection<GioHangChiTiet> GioHangChiTiets { get; set; }
	}
}
