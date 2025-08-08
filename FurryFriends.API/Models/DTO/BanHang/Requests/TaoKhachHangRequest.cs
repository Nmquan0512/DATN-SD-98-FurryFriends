namespace FurryFriends.API.Models.DTO.BanHang.Requests
{
    public class TaoKhachHangRequest
    {
        public string TenKhachHang { get; set; }
        public string SDT { get; set; }
        public string Email { get; set; }

        // Thêm địa chỉ nếu cần
        public string DiaChi { get; set; }
    }
}