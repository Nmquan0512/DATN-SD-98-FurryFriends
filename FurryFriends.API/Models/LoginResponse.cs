namespace FurryFriends.API.Models
{
    public class LoginResponse
    {
        public Guid TaiKhoanId { get; set; }
        public string Role { get; set; }
        public string? HoTen { get; set; }
        public Guid KhachHangId { get; set; } //sửa ỏ đây thêm khachHangId
    }
} 