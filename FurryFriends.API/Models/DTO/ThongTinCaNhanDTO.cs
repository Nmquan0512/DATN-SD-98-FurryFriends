﻿namespace FurryFriends.API.Models.DTO
{
	public class ThongTinCaNhanDTO
	{
		public Guid TaiKhoanId { get; set; }
		public string UserName { get; set; }
		public string? HoTen { get; set; }
		public string? Email { get; set; }
		public string? SoDienThoai { get; set; }
		public DateTime? NgaySinh { get; set; }
		public string? GioiTinh { get; set; }
		public string? DiaChi { get; set; }
		public string? Role { get; set; }
	}
}
