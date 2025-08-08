using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace FurryFriends.API.Models.DTO
{
    public class SanPhamChiTietDTO : IValidatableObject
    {
        
        public Guid SanPhamChiTietId { get; set; }
        public Guid SanPhamId { get; set; }
        
        public Guid MauSacId { get; set; }
        public string? TenSanPham { get; set; } 
        public string? TenMau { get; set; }

        public Guid KichCoId { get; set; }

        public string? TenKichCo { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "Giá bán phải là số dương")]
        public decimal Gia { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Số lượng phải là số dương hoặc bằng 0")]
        public int SoLuong { get; set; }

        public string? MoTa { get; set; }

        public List<IFormFile>? AnhSanPham { get; set; } // cho tạo mới

        public Guid? AnhId { get; set; }
         
        public string? DuongDan { get; set; }

        public DateTime? NgayTao { get; set; }

        public DateTime? NgaySua { get; set; }


        public int? TrangThai { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var results = new List<ValidationResult>();

            if (MauSacId == Guid.Empty)
            {
                results.Add(new ValidationResult("Màu sắc không được để trống", new[] { nameof(MauSacId) }));
            }

            if (KichCoId == Guid.Empty)
            {
                results.Add(new ValidationResult("Kích cỡ không được để trống", new[] { nameof(KichCoId) }));
            }

            return results;
        }
    }
}
