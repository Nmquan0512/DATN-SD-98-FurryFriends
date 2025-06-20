using FurryFriends.API.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FurryFriends.Web.Services.IService
{
    public interface IKhachHangService
    {
        Task<IEnumerable<KhachHang>> GetAllKhachHangAsync();
        Task<KhachHang> GetKhachHangByIdAsync(Guid id);
        Task AddKhachHangAsync(KhachHang model);
        Task UpdateKhachHangAsync(KhachHang model);
        Task DeleteKhachHangAsync(Guid id);
    }
}
