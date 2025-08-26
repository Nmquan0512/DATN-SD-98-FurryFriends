using System.ComponentModel.DataAnnotations;

namespace FurryFriends.API.Models.DTO
{

    public class ForgotPasswordRequest
    {
        [Required(ErrorMessage = "Vui lòng nhập email của bạn.")]
        [EmailAddress(ErrorMessage = "Địa chỉ email không hợp lệ.")]
        public string Email { get; set; }
    }
}