using FurryFriends.API.Models;
using FurryFriends.API.Models.DTO;

namespace FurryFriends.API.Repository.IRepository
{
    public interface IGioHangRepository
    {
        Task<GioHangDTO> GetGioHangByKhachHangIdAsync(Guid khachHangId);
        Task<GioHangChiTiet> AddSanPhamVaoGioAsync(Guid khachHangId, Guid sanPhamId, int soLuong);
        Task<GioHangChiTiet> UpdateSoLuongAsync(Guid gioHangChiTietId, int soLuong);
        Task<bool> RemoveSanPhamKhoiGioAsync(Guid gioHangChiTietId);
        Task<SanPhamChiTiet> GetSanPhamChiTietByIdAsync(Guid id);
        Task<GioHangChiTietDTO> ConvertToDTOAsync(GioHangChiTiet gioHangChiTiet);
        Task<GioHang> GetGioHangEntityByKhachHangIdAsync(Guid khachHangId);
        Task<object> ThanhToanAsync(ThanhToanDTO dto);
    }
}
