using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FurryFriends.API.Models.DTO
{
    public class GiamGiaDTO
    {
        public Guid GiamGiaId { get; set; }
        [Required(ErrorMessage = "Tên giảm giá không được để trống")]
        [StringLength(100, ErrorMessage = "Tên giảm giá không được vượt quá 100 ký tự")]
        public string TenGiamGia { get; set; }
        [Required(ErrorMessage = "Phần trăm khuyến mãi không được để trống")]
        [Range(0, 100, ErrorMessage = "Phần trăm khuyến mãi phải từ 0 đến 100")]
        public decimal PhanTramKhuyenMai { get; set; }
        [Required(ErrorMessage = "Vui lòng chọn ngày bắt đầu")]
        public DateTime? NgayBatDau { get; set; }
        [Required(ErrorMessage = "Vui lòng chọn ngày kết thúc")]
        public DateTime? NgayKetThuc { get; set; }
        [Required(ErrorMessage = "Vui lòng chọn trạng thái")]
        public bool TrangThai { get; set; }
        public DateTime NgayTao { get; set; }
        public DateTime NgayCapNhat { get; set; }
        public List<Guid>? SanPhamChiTietIds { get; set; }
    }
} 