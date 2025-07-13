using FurryFriends.API.Models.DTO;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FurryFriends.Web.Services.IService
{
    public interface ISanPhamService
    {
        Task<IEnumerable<SanPhamDTO>> GetAllAsync();
        Task<SanPhamDTO> GetByIdAsync(Guid id);
        Task<SanPhamDTO> CreateAsync(SanPhamDTO dto);
        Task<bool> UpdateAsync(Guid id, SanPhamDTO dto);
        Task<bool> DeleteAsync(Guid id);

        Task<(IEnumerable<SanPhamDTO> Data, int TotalItems)> GetFilteredAsync(string? loai, int page, int pageSize);
        Task<int> GetTotalProductsAsync();
        Task<IEnumerable<SanPhamDTO>> GetTopSellingProductsAsync(int top);
    }
} 