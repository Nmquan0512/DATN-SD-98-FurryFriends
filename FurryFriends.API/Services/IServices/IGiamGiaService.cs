using FurryFriends.API.Models.DTO;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FurryFriends.API.Services.IServices
{
    public interface IGiamGiaService
    {
        Task<GiamGiaDTO> GetByIdAsync(Guid id);
        Task<IEnumerable<GiamGiaDTO>> GetAllAsync();
        Task<GiamGiaDTO> CreateAsync(GiamGiaDTO dto);
        Task<GiamGiaDTO> UpdateAsync(GiamGiaDTO dto);
        Task<bool> DeleteAsync(Guid id);
    }
}