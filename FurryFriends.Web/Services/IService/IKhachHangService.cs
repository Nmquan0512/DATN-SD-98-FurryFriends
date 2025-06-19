using FurryFriends.API.Models;

  namespace FurryFriends.Web.IServices
    {
        public interface IKhachHangService
        {
            Task<IEnumerable<KhachHang>> GetAllKhachHangAsync();
            Task<KhachHang> GetKhachHangByIdAsync(Guid id);
            Task AddKhachHangAsync(KhachHang khachHang);
            Task UpdateKhachHangAsync(KhachHang khachHang);
            Task DeleteKhachHangAsync(Guid id);
        }
    }

