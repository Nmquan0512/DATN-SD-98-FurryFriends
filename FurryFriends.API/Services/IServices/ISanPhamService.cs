using FurryFriends.API.Models.DTO;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FurryFriends.API.Services.IServices
{
    public interface ISanPhamService
    {
        Task<IEnumerable<SanPhamDTO>> GetAllAsync();
        Task<SanPhamDTO> GetByIdAsync(Guid id);
        Task<SanPhamDTO> CreateAsync(SanPhamDTO dto);
        Task<bool> UpdateAsync(Guid id, SanPhamDTO dto);
        Task<bool> DeleteAsync(Guid id);

        // Phân trang + lọc
        Task<(IEnumerable<SanPhamDTO> Data, int TotalCount)> GetFilteredAsync(string? loaiSanPham, int page, int pageSize);
        Task<int> GetTotalProductsAsync();
        Task<IEnumerable<SanPhamDTO>> GetTopSellingProductsAsync(int top);
    }
}
