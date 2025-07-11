using FurryFriends.API.Models;
using FurryFriends.API.Models.DTO;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FurryFriends.Web.Services.IService
{
    public interface IThanhPhanService
    {
        Task<IEnumerable<ThanhPhanDTO>> GetAllAsync();
        Task<ThanhPhanDTO> GetByIdAsync(Guid id);
        Task<ApiResult<ThanhPhanDTO>> CreateAsync(ThanhPhanDTO dto);
        Task<ApiResult<bool>> UpdateAsync(Guid id, ThanhPhanDTO dto);
        Task<bool> DeleteAsync(Guid id);
    }
}
