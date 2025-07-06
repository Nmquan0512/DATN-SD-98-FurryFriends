using FurryFriends.API.Models.DTO;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FurryFriends.API.Services.IServices
{
    public interface IKichCoService
    {
        Task<IEnumerable<KichCoDTO>> GetAllAsync();
        Task<KichCoDTO> GetByIdAsync(Guid id);
        Task<KichCoDTO> CreateAsync(KichCoDTO dto);
        Task<bool> UpdateAsync(Guid id, KichCoDTO dto);
        Task<bool> DeleteAsync(Guid id);
    }
}
