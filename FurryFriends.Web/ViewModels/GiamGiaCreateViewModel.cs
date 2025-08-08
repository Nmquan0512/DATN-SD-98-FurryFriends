using FurryFriends.API.Models.DTO;
using System;
using System.Collections.Generic;

namespace FurryFriends.Web.ViewModels
{
    public class GiamGiaCreateViewModel
    {
        public GiamGiaDTO GiamGia { get; set; } = new GiamGiaDTO();
        public List<Guid> SanPhamChiTietIds { get; set; } = new List<Guid>();

        // Thêm các thuộc tính hỗ trợ cho view
        public int SoLuongSanPhamDaChon => SanPhamChiTietIds?.Count ?? 0;
        public bool CoSanPhamDaChon => SoLuongSanPhamDaChon > 0;
    }

    public class GiamGiaAssignProductsViewModel
    {
        public GiamGiaDTO GiamGia { get; set; }
        public List<SanPhamChiTietGiamGiaItemViewModel> SanPhamChiTiets { get; set; } = new List<SanPhamChiTietGiamGiaItemViewModel>();
        public List<Guid> SanPhamChiTietIds { get; set; } = new List<Guid>();

        // Thuộc tính hỗ trợ
        public string TenChuongTrinh => GiamGia?.TenGiamGia ?? "Không xác định";
    }

    public class SanPhamChiTietGiamGiaItemViewModel
    {
        public Guid SanPhamChiTietId { get; set; }
        public string TenSanPham { get; set; }
        public string TenMau { get; set; }
        public string TenKichCo { get; set; }
        public decimal Gia { get; set; }
        public string DuongDan { get; set; }
        public bool DuocChon { get; set; }

        // Thuộc tính hiển thị
        public string GiaDisplay => Gia.ToString("N0") + "₫";
        public string AnhSanPham => !string.IsNullOrEmpty(DuongDan) ? DuongDan : "/images/no-image.png";
    }
}