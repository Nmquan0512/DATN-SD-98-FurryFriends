using FurryFriends.API.Data;
﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FurryFriends.API.Models.DTO
{
    public class SanPhamDTO : IValidatableObject
    {
        public Guid SanPhamId { get; set; } = Guid.NewGuid();

        [Required(ErrorMessage = "Tên sản phẩm là bắt buộc.")]
        [StringLength(100, ErrorMessage = "Tên sản phẩm tối đa 100 ký tự.")]
        public string TenSanPham { get; set; }

        [Required(ErrorMessage = "Loại sản phẩm là bắt buộc.")]
        [RegularExpression("DoAn|DoDung", ErrorMessage = "Loại sản phẩm phải là 'DoAn' hoặc 'DoDung'.")]
        public string LoaiSanPham { get; set; } // "DoAn" hoặc "DoDung"

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

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            // Nếu cần validate nâng cao, thêm ở đây
            yield break;
                }
    }
}
