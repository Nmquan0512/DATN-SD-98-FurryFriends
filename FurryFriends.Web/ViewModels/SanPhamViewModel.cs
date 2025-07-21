﻿using FurryFriends.API.Models.DTO;
namespace FurryFriends.Web.ViewModels
{
    public class SanPhamViewModel
    {
        public Guid SanPhamId { get; set; }
        public string TenSanPham { get; set; } = "";
        public string MoTa { get; set; } = "";
        public bool TrangThai { get; set; }

        public string? AnhDaiDienUrl { get; set; } // ảnh đầu tiên của chi tiết

        public List<SanPhamChiTietViewModel> ChiTietList { get; set; } = new();
    }
}
