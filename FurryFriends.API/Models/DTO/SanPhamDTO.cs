using System;
using System.ComponentModel.DataAnnotations;

namespace FurryFriends.API.Models.DTO
{
    public class SanPhamDTO
    {
        public Guid SanPhamId { get; set; }

        [Required(ErrorMessage = "Tên sản phẩm là bắt buộc.")]
        public string TenSanPham { get; set; }

        [Required(ErrorMessage = "Tài khoản là bắt buộc.")]
        public Guid TaiKhoanId { get; set; }

        [Required(ErrorMessage = "Thương hiệu là bắt buộc.")]
        public Guid ThuongHieuId { get; set; }

        [Required(ErrorMessage = "Trạng thái là bắt buộc.")]
        public bool TrangThai { get; set; }
    }
}