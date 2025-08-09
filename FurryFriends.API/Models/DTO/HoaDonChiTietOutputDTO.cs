namespace FurryFriends.API.Models.DTO
{
    public class HoaDonChiTietOutputDTO
    {
        public string TenSanPhamLucMua { get; set; }
        public string MoTaSanPhamLucMua { get; set; }
        public string ThuongHieuLucMua { get; set; }
        public string KichCoLucMua { get; set; }
        public string MauSacLucMua { get; set; }
        public string AnhSanPhamLucMua { get; set; }
        public string ChatLieuLucMua { get; set; }
        public string ThanhPhanLucMua { get; set; }

        public int? SoLuong { get; set; }
        public decimal? GiaLucMua { get; set; }
    }
}
