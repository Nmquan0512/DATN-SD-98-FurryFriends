<<<<<<< HEAD
﻿using System;
using System.Collections.Generic;
=======
﻿using FurryFriends.API.Data;
using System;
>>>>>>> origin/TruongValidate
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

<<<<<<< HEAD
        public string? TenThuongHieu { get; set; }

        // Hiển thị tên chất liệu nếu có
        public List<string>? TenChatLieus { get; set; }

        // Hiển thị tên thành phần nếu có
        public List<string>? TenThanhPhans { get; set; }

        public DateTime NgayTao { get; set; } = DateTime.UtcNow;

        public DateTime? NgaySua { get; set; }

        public bool TrangThai { get; set; } = true;
=======
        [Required(ErrorMessage = "Trạng thái là bắt buộc.")]
        public bool TrangThai { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var _context = (AppDbContext)validationContext.GetService(typeof(AppDbContext));

            if (_context != null)
            {
                var isDuplicate = _context.SanPhams
                    .Any(x => x.TenSanPham.ToLower().Trim() == TenSanPham.ToLower().Trim()
                           && x.SanPhamId != SanPhamId);

                if (isDuplicate)
                {
                    yield return new ValidationResult("Tên sản phẩm đã tồn tại.", new[] { nameof(TenSanPham) });
                }
            }
        }
>>>>>>> origin/TruongValidate
    }
}
