using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace FurryFriends.API.Models.DTO
{
    public class SanPhamChiTietDTO
    {
        public Guid Id { get; set; } 
        public Guid? SanPhamChiTietId { get; set; } 
        public Guid MauSacId { get; set; }

        public string? TenMauSac { get; set; }

        public Guid KichCoId { get; set; }

        public string? TenKichCo { get; set; }

        public decimal Gia { get; set; }

        public int SoLuong { get; set; }

        public string? MoTa { get; set; }

        public List<IFormFile>? AnhSanPham { get; set; } // cho tạo mới

        public Guid? AnhId { get; set; }

        public string UrlAnh { get; set; }

        public DateTime? NgayTao { get; set; }

        public DateTime? NgaySua { get; set; }

        public int? TrangThai { get; set; }
    }
}
