using System;

namespace FurryFriends.API.Models.DTO.BanHang
{
    public class KhachHangDto
    {
        public Guid KhachHangId { get; set; }
        public string TenKhachHang { get; set; }
        public string SDT { get; set; }
        public string Email { get; set; }
        public int DiemTichLuy { get; set; }
        public bool LaKhachLe { get; set; } = false;
    }
}