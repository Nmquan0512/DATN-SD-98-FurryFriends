using System.ComponentModel.DataAnnotations;

namespace FurryFriends.Web.Models
{
	public class DoiMatKhauViewModel
	{
		[Required(ErrorMessage = "Vui lòng nhập mật khẩu cũ.")]
		public string MatKhauCu { get; set; }

		[Required(ErrorMessage = "Vui lòng nhập mật khẩu mới.")]
		[MinLength(6, ErrorMessage = "Mật khẩu phải có ít nhất 6 ký tự.")]
		public string MatKhauMoi { get; set; }

		[Compare("MatKhauMoi", ErrorMessage = "Mật khẩu xác nhận không khớp.")]
		public string XacNhanMatKhauMoi { get; set; }
	}
}
