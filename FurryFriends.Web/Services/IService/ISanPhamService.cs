using System.Collections.Generic;
using System.Threading.Tasks;

namespace FurryFriends.Web.Services.IService
{
    public interface ISanPhamService
    {
        Task<int> GetTotalProductsAsync();
        Task<List<object>> GetTopSellingProductsAsync(int count);
        Task<decimal> GetTotalRevenueAsync();
        Task<List<object>> GetProductsByCategoryAsync();
    }
} 