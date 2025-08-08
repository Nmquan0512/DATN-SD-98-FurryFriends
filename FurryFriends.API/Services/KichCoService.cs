using FurryFriends.API.Models;
using FurryFriends.API.Models.DTO;
using FurryFriends.API.Repository.IRepository;
using FurryFriends.API.Services.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FurryFriends.API.Services
{
    public class KichCoService : IKichCoService
    {
        private readonly IKichCoRepository _repository;

        public KichCoService(IKichCoRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<KichCoDTO>> GetAllAsync()
        {
            var entities = await _repository.GetAllAsync();
            return entities.Select(x => new KichCoDTO
            {
                KichCoId = x.KichCoId,
                TenKichCo = x.TenKichCo,
                MoTa = x.MoTa,
                TrangThai = x.TrangThai
            });
        }

        public async Task<KichCoDTO> GetByIdAsync(Guid id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null) return null;

            return new KichCoDTO
            {
                KichCoId = entity.KichCoId,
                TenKichCo = entity.TenKichCo,
                MoTa = entity.MoTa,
                TrangThai = entity.TrangThai
            };
        }

        public async Task<KichCoDTO> CreateAsync(KichCoDTO dto)
        {
            // Kiểm tra trùng tên
            var allEntities = await _repository.GetAllAsync();
            var isDuplicate = allEntities.Any(x => 
                x.TenKichCo.ToLower().Trim() == dto.TenKichCo.ToLower().Trim());

            if (isDuplicate)
            {
                throw new InvalidOperationException("Tên kích cỡ đã tồn tại");
            }

            var entity = new KichCo
            {
                KichCoId = Guid.NewGuid(),
                TenKichCo = dto.TenKichCo,
                MoTa = dto.MoTa,
                TrangThai = dto.TrangThai
            };

            await _repository.AddAsync(entity);
            dto.KichCoId = entity.KichCoId;
            return dto;
        }

        public async Task<bool> UpdateAsync(Guid id, KichCoDTO dto)
        {
            var existing = await _repository.GetByIdAsync(id);
            if (existing == null) return false;

            // Kiểm tra trùng tên (trừ chính nó)
            var allEntities = await _repository.GetAllAsync();
            var isDuplicate = allEntities.Any(x => 
                x.KichCoId != id && 
                x.TenKichCo.ToLower().Trim() == dto.TenKichCo.ToLower().Trim());

            if (isDuplicate)
            {
                throw new InvalidOperationException("Tên kích cỡ đã tồn tại");
            }

            existing.TenKichCo = dto.TenKichCo;
            existing.MoTa = dto.MoTa;
            existing.TrangThai = dto.TrangThai;

            await _repository.UpdateAsync(existing);
            return true;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var exists = await _repository.ExistsAsync(id);
            if (!exists) return false;

            await _repository.DeleteAsync(id);
            return true;
        }
    }
}
