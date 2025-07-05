using FurryFriends.Web.Services.IService;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FurryFriends.Web.Services
{
    public class SanPhamService : ISanPhamService
    {
        public async Task<int> GetTotalProductsAsync()
        {
            // Simulate database call
            await Task.Delay(100);
            return 150; // Mock data
        }

        public async Task<List<object>> GetTopSellingProductsAsync(int count)
        {
            // Simulate database call
            await Task.Delay(100);
            
            var products = new List<object>
            {
                new { Name = "Thức ăn mèo Royal Canin", Sales = 150, Revenue = 15000000 },
                new { Name = "Thức ăn chó Pedigree", Sales = 120, Revenue = 12000000 },
                new { Name = "Đồ chơi cho mèo", Sales = 80, Revenue = 8000000 },
                new { Name = "Vòng cổ thú cưng", Sales = 60, Revenue = 6000000 },
                new { Name = "Chuồng thú cưng", Sales = 40, Revenue = 4000000 }
            };

            return products.GetRange(0, count);
        }

        public async Task<decimal> GetTotalRevenueAsync()
        {
            // Simulate database call
            await Task.Delay(100);
            return 45000000; // Mock data
        }

        public async Task<List<object>> GetProductsByCategoryAsync()
        {
            // Simulate database call
            await Task.Delay(100);
            
            return new List<object>
            {
                new { Category = "Thức ăn", Count = 50, Revenue = 25000000 },
                new { Category = "Đồ chơi", Count = 30, Revenue = 10000000 },
                new { Category = "Phụ kiện", Count = 40, Revenue = 8000000 },
                new { Category = "Chuồng", Count = 20, Revenue = 2000000 }
            };
        }
    }
} 