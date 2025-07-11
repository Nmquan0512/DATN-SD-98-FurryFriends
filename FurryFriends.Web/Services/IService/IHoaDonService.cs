using FurryFriends.API.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FurryFriends.Web.Services.IService
{
    public interface IHoaDonService
    {
        Task<IEnumerable<HoaDon>> GetHoaDonListAsync();
        Task<HoaDon> GetHoaDonByIdAsync(Guid hoaDonId);
        Task<IEnumerable<HoaDon>> SearchHoaDonAsync(string keyword);
        Task<byte[]> ExportHoaDonToPdfAsync(Guid hoaDonId);
        
        // Dashboard methods
        Task<int> GetTotalOrdersAsync();
        Task<decimal> GetMonthlyRevenueAsync();
        Task<List<object>> GetRevenueByMonthAsync();
        Task<List<object>> GetOrdersByStatusAsync();
        Task<List<object>> GetRecentOrdersAsync(int count);
    }
}