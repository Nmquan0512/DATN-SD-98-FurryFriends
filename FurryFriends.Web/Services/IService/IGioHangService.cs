using FurryFriends.API.Models.DTO;
using FurryFriends.Web.ViewModels;

namespace FurryFriends.Web.Services.IService
{
    public interface IGioHangService
    {
        Task<GioHangDTO> GetGioHangAsync(Guid khachHangId);
        Task AddToCartAsync(AddToCartDTO dto);
        Task UpdateSoLuongAsync(Guid chiTietId, int soLuong);
        Task RemoveAsync(Guid chiTietId);
        Task<decimal> TinhTongTienSauVoucher(Guid khachHangId, Guid voucherId);
        Task<ThanhToanResultViewModel> ThanhToanAsync(ThanhToanDTO dto);
    }
}
