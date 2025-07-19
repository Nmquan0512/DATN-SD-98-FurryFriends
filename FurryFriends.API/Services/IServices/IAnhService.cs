using FurryFriends.API.Models.DTO;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FurryFriends.API.Services.IServices
{
    public interface IAnhService
    {
        Task<IEnumerable<AnhDTO>> GetAllAsync();
        Task<AnhDTO?> GetByIdAsync(Guid id);
        Task<AnhDTO?> UploadAsync(IFormFile file, Guid? sanPhamChiTietId = null);
        Task<bool> UpdateAsync(Guid id, AnhDTO dto);
        Task<bool> DeleteAsync(Guid id);
    }
}
