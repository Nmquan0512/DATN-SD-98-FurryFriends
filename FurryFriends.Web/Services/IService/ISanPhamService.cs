using FurryFriends.API.Models;
using FurryFriends.API.Models.DTO;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FurryFriends.Web.Services.IService
{
    public interface ISanPhamService
    {
        Task<IEnumerable<SanPhamDTO>> GetAllAsync();
        Task<SanPhamDTO?> GetByIdAsync(Guid id);

        // Sửa kiểu trả về của các phương thức này
        Task<ApiResult<SanPhamDTO>> CreateAsync(SanPhamDTO dto);
        Task<ApiResult<bool>> UpdateAsync(Guid id, SanPhamDTO dto);
        Task<ApiResult<bool>> DeleteAsync(Guid id);

        // Giữ nguyên các phương thức này
        Task<(IEnumerable<SanPhamDTO> Data, int Total)> GetFilteredAsync(string? loai, int page, int pageSize);
        Task<int> GetTotalProductsAsync();
        Task<IEnumerable<SanPhamDTO>> GetTopSellingProductsAsync(int top);
    }
}