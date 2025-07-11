using FurryFriends.API.Models;
using FurryFriends.API.Models.DTO;
using FurryFriends.API.Repositories.IRepositories;
using FurryFriends.API.Services.IServices;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FurryFriends.API.Services
{
    public class AnhService : IAnhService
    {
        private readonly IAnhRepository _repo;

        public AnhService(IAnhRepository repo)
        {
            _repo = repo;
        }

        public async Task<IEnumerable<AnhDTO>> GetAllAsync()
        {
            var list = await _repo.GetAllAsync();
            return list.Select(a => new AnhDTO
            {
                AnhId = a.AnhId,
                TenAnh = a.TenAnh,
                DuongDan = a.DuongDan,
                TrangThai = a.TrangThai
            });
        }

        public async Task<AnhDTO> GetByIdAsync(Guid id)
        {
            var a = await _repo.GetByIdAsync(id);
            if (a == null) return null;

            return new AnhDTO
            {
                AnhId = a.AnhId,
                TenAnh = a.TenAnh,
                DuongDan = a.DuongDan,
                TrangThai = a.TrangThai
            };
        }

        public async Task<AnhDTO> UploadAsync(IFormFile file)
        {
            var a = await _repo.UploadAsync(file);
            if (a == null) return null;

            return new AnhDTO
            {
                AnhId = a.AnhId,
                TenAnh = a.TenAnh,
                DuongDan = a.DuongDan,
                TrangThai = a.TrangThai
            };
        }

        public async Task<bool> DeleteAsync(Guid id) => await _repo.DeleteAsync(id);
    }
}