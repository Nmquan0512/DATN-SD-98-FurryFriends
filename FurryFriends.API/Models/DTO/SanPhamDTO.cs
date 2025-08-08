using FurryFriends.API.Data;
﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FurryFriends.API.Models.DTO
{
    public class SanPhamDTO : IValidatableObject
    {
        public Guid SanPhamId { get; set; } = Guid.NewGuid();

        [Required(ErrorMessage = "Tên sản phẩm không được để trống")]
        [StringLength(255, ErrorMessage = "Tên sản phẩm vượt quá giới hạn ký tự cho phép")]
        public string TenSanPham { get; set; } = string.Empty;

        [Required(ErrorMessage = "Loại sản phẩm là bắt buộc.")]
        [RegularExpression("DoAn|DoDung", ErrorMessage = "Loại sản phẩm phải là 'DoAn' hoặc 'DoDung'.")]
        public string LoaiSanPham { get; set; } = string.Empty; // "DoAn" hoặc "DoDung"

        // Nếu là Đồ Ăn
        public List<Guid>? ThanhPhanIds { get; set; }

        // Nếu là Đồ Dùng
        public List<Guid>? ChatLieuIds { get; set; }

        [Required(ErrorMessage = "Thương hiệu là bắt buộc.")]
        public Guid ThuongHieuId { get; set; }

        public string? TenThuongHieu { get; set; }
        public List<string>? TenChatLieus { get; set; }
        public List<string>? TenThanhPhans { get; set; }
        public DateTime NgayTao { get; set; } = DateTime.UtcNow;
        public DateTime? NgaySua { get; set; }
        public bool TrangThai { get; set; } = true;

        public List<SanPhamChiTietDTO>? SanPhamChiTiets { get; set; } //thêm cái này

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (string.IsNullOrWhiteSpace(TenSanPham))
            {
                yield return new ValidationResult("Tên sản phẩm không được để trống", new[] { nameof(TenSanPham) });
            }

            if (TenSanPham?.Length > 255)
            {
                yield return new ValidationResult("Tên sản phẩm vượt quá giới hạn ký tự cho phép", new[] { nameof(TenSanPham) });
            }

            if (ThuongHieuId == Guid.Empty)
            {
                yield return new ValidationResult("Thương hiệu không được để trống", new[] { nameof(ThuongHieuId) });
            }
        }
    }
}
