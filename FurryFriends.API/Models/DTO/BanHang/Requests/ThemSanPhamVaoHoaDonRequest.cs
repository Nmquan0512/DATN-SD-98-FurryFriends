using System;

namespace FurryFriends.API.Models.DTO.BanHang.Requests
{
    public class ThemSanPhamVaoHoaDonRequest
    {
        public Guid HoaDonId { get; set; }
        public Guid SanPhamChiTietId { get; set; } // ID biến thể sản phẩm (màu sắc/kích cỡ)
        public int SoLuong { get; set; }

        // Thêm giá bán để kiểm tra tính nhất quán
        public decimal? GiaBan { get; set; } // Optional để validate
    }
}