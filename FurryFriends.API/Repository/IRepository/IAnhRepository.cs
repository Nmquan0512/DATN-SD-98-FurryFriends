using FurryFriends.API.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FurryFriends.API.Repositories.IRepositories
{
    public interface IAnhRepository
    {
        Task<IEnumerable<Anh>> GetAllAsync();
        Task<Anh> GetByIdAsync(Guid id);
        Task<Anh> UploadAsync(IFormFile file);
        Task<bool> DeleteAsync(Guid id);
    }
}