using FurryFriends.API.Models.DTO;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FurryFriends.API.Services.IServices
{
    public interface IThuongHieuService
    {
        Task<IEnumerable<ThuongHieuDTO>> GetAllAsync();
        Task<ThuongHieuDTO> GetByIdAsync(Guid id);
        Task<ThuongHieuDTO> CreateAsync(ThuongHieuDTO dto);
        Task<bool> UpdateAsync(Guid id, ThuongHieuDTO dto);
        Task<bool> DeleteAsync(Guid id);
    }
}