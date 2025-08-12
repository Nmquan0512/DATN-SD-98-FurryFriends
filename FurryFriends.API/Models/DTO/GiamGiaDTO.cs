using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FurryFriends.API.Models.DTO
{
    public class GiamGiaDTO
    {
        public Guid GiamGiaId { get; set; } = Guid.NewGuid();
       
        [Required(ErrorMessage = "Tên giảm giá không được để trống")]
        [StringLength(100, ErrorMessage = "Tên giảm giá không được vượt quá 100 ký tự")]
        public string TenGiamGia { get; set; }

        [Required(ErrorMessage = "Phần trăm khuyến mãi không được để trống")]
        [Range(0, 100, ErrorMessage = "Phần trăm khuyến mãi phải từ 0 đến 100")]
        public decimal PhanTramKhuyenMai { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn ngày bắt đầu")]
        public DateTime NgayBatDau { get; set; } = DateTime.UtcNow.Date;

        [Required(ErrorMessage = "Vui lòng chọn ngày kết thúc")]
        public DateTime NgayKetThuc { get; set; } = DateTime.UtcNow.Date.AddDays(7);

        public bool TrangThai { get; set; } = true;

        // Danh sách sản phẩm áp dụng (không bắt buộc khi tạo mới)
        public List<Guid> SanPhamChiTietIds { get; set; } = new List<Guid>();

        // Các thuộc tính chỉ để hiển thị (không cần validate)
        public string TrangThaiDisplay => TrangThai ? "Đang hoạt động" : "Đã tắt";
        public bool ConHieuLuc => TrangThai && NgayBatDau <= DateTime.UtcNow.Date && NgayKetThuc >= DateTime.UtcNow.Date;
        public int SoLuongSanPhamApDung => SanPhamChiTietIds?.Count ?? 0;

        // Validation logic đơn giản hóa
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (NgayKetThuc <= NgayBatDau)
            {
                yield return new ValidationResult(
                    "Ngày kết thúc phải sau ngày bắt đầu.",
                    new[] { nameof(NgayKetThuc) });
            }

            if (PhanTramKhuyenMai <= 0 || PhanTramKhuyenMai > 100)
            {
                yield return new ValidationResult(
                    "Phần trăm khuyến mãi phải từ 1 đến 100.",
                    new[] { nameof(PhanTramKhuyenMai) });
            }
        }
    }
}