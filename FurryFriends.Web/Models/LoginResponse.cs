namespace FurryFriends.Web.Models
{
	public class LoginResponse
	{
		public Guid TaiKhoanId { get; set; }
		public string Role { get; set; }
		public string? HoTen { get; set; }
	}
}
