using FurryFriends.API.Models.DTO;
using System;
using System.Collections.Generic;

namespace FurryFriends.Web.ViewModels
{
    // ViewModels/GiamGiaCreateViewModel.cs
    public class SanPhamChiTietGiamGiaItemViewModel
    {
        public Guid SanPhamChiTietId { get; set; }
        public string TenSanPham { get; set; }
        public string TenMau { get; set; }
        public string TenKichCo { get; set; }
        public decimal Gia { get; set; }
        public string? DuongDan { get; set; } // ảnh
        public bool DuocChon { get; set; } // để đánh dấu checkbox
    }

    public class GiamGiaCreateViewModel
    {
        public GiamGiaDTO GiamGia { get; set; } = new GiamGiaDTO();
        public List<SanPhamChiTietGiamGiaItemViewModel> SanPhamChiTietList { get; set; } = new();
    }

}
