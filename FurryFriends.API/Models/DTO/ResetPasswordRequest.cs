using System.ComponentModel.DataAnnotations;

namespace FurryFriends.API.Models.DTO
{
    public class ResetPasswordRequest
    {
        [Required(ErrorMessage = "Vui lòng nhập email của bạn.")]
        [EmailAddress(ErrorMessage = "Địa chỉ email không hợp lệ.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập mã xác nhận.")]
        public string Code { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập mật khẩu mới.")]
        [MinLength(6, ErrorMessage = "Mật khẩu mới phải có ít nhất 6 ký tự.")]
        public string NewPassword { get; set; }
    }
}
