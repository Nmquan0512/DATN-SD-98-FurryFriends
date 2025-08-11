using FurryFriends.API.Models.DTO;
using FurryFriends.Web.ViewModels;

namespace FurryFriends.Web.Services.IService
{
    public class VoucherPreviewResult
    {
        public decimal TongTienHang { get; set; }
        public decimal PhiVanChuyen { get; set; }
        public decimal TongDonHang { get; set; }
        public decimal GiamGia { get; set; }
        public decimal TienSauGiam { get; set; }
        public decimal PhanTramGiam { get; set; }
        public string TenVoucher { get; set; } = "";
        public string MaVoucher { get; set; } = "";
    }

    public interface IGioHangService
    {
        Task<GioHangDTO> GetGioHangAsync(Guid khachHangId);
        Task AddToCartAsync(AddToCartDTO dto);
        Task UpdateSoLuongAsync(Guid chiTietId, int soLuong);
        Task RemoveAsync(Guid chiTietId);
        Task<decimal> TinhTongTienSauVoucher(Guid khachHangId, Guid voucherId);
        Task<ThanhToanResultViewModel> ThanhToanAsync(ThanhToanDTO dto);
        Task<VoucherPreviewResult?> PreviewVoucherAsync(Guid khachHangId, Guid voucherId);
    }
}
