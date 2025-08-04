using FurryFriends.API.Models.DTO;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FurryFriends.Web.Services.IService
{
    public interface IGiamGiaService
    {
        Task<IEnumerable<GiamGiaDTO>> GetAllAsync();
        Task<GiamGiaDTO> GetByIdAsync(Guid id);
        Task<GiamGiaDTO> CreateAsync(GiamGiaDTO dto);
        Task<GiamGiaDTO> UpdateAsync(Guid id, GiamGiaDTO dto);
        Task<bool> DeleteAsync(Guid id);
        Task<bool> AssignProductsAsync(Guid giamGiaId, List<Guid> productIds);
    }
}