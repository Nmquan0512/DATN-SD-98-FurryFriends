using FurryFriends.API.Models.DTO;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FurryFriends.API.Services.IServices
{
    public interface ISanPhamChiTietService
    {
        Task<IEnumerable<SanPhamChiTietDTO>> GetAllAsync();
        Task<SanPhamChiTietDTO?> GetByIdAsync(Guid id);
        Task<bool> CreateAsync(SanPhamChiTietDTO dto);
        Task<bool> UpdateAsync(Guid id, SanPhamChiTietDTO dto);
        Task<bool> DeleteAsync(Guid id);
    }
}
