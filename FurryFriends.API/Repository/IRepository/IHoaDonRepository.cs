using FurryFriends.API.Models;

namespace FurryFriends.API.Repository.IRepository
{
    public interface IHoaDonRepository
    {
        Task<IEnumerable<HoaDon>> GetHoaDonListAsync();
        Task<HoaDon> GetHoaDonByIdAsync(Guid hoaDonId);
        Task<IEnumerable<HoaDon>> SearchHoaDonAsync(Func<HoaDon, bool> predicate);
        Task<byte[]> ExportHoaDonToPdfAsync(Guid hoaDonId);
        Task<ApiResult> HuyDonHangAsync(Guid hoaDonId);
        Task<ApiResult> CapNhatTrangThaiAsync(Guid hoaDonId, int trangThaiMoi);
        Task<IEnumerable<HoaDonChiTiet>> GetChiTietHoaDonAsync(Guid hoaDonId);
        
        // ✅ Method mới cho quản lý đơn hàng - chỉ lấy hóa đơn trạng thái 0-5
        Task<IEnumerable<HoaDon>> GetDonHangListAsync();
    }
}
