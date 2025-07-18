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
        Task UpdateAsync(Guid id, SanPhamDTO dto);
        Task DeleteAsync(Guid id);
        Task<(IEnumerable<SanPhamDTO> Data, int TotalCount)> GetFilteredAsync(string? loaiSanPham, int page, int pageSize);
        Task<int> GetTotalProductsAsync();
        Task<IEnumerable<SanPhamDTO>> GetTopSellingProductsAsync(int top);
    }
}