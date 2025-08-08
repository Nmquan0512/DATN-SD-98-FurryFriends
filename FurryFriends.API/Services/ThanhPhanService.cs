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
    public class ThanhPhanService : IThanhPhanService
    {
        private readonly IThanhPhanRepository _repository;

        public ThanhPhanService(IThanhPhanRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<ThanhPhanDTO>> GetAllAsync()
        {
            var entities = await _repository.GetAllAsync();
            return entities.Select(x => new ThanhPhanDTO
            {
                ThanhPhanId = x.ThanhPhanId,
                TenThanhPhan = x.TenThanhPhan,
                MoTa = x.MoTa,
                TrangThai = x.TrangThai
            });
        }

        public async Task<ThanhPhanDTO> GetByIdAsync(Guid id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null) return null;

            return new ThanhPhanDTO
            {
                ThanhPhanId = entity.ThanhPhanId,
                TenThanhPhan = entity.TenThanhPhan,
                MoTa = entity.MoTa,
                TrangThai = entity.TrangThai
            };
        }

        public async Task<ThanhPhanDTO> CreateAsync(ThanhPhanDTO dto)
        {
            // Kiểm tra trùng tên
            var existing = await _repository.GetAllAsync();
            if (existing.Any(x => x.TenThanhPhan.ToLower().Trim() == dto.TenThanhPhan.ToLower().Trim()))
            {
                throw new InvalidOperationException("Tên thành phần đã tồn tại.");
            }

            var entity = new ThanhPhan
            {
                ThanhPhanId = Guid.NewGuid(),
                TenThanhPhan = dto.TenThanhPhan,
                MoTa = dto.MoTa,
                TrangThai = dto.TrangThai
            };

            await _repository.AddAsync(entity);
            dto.ThanhPhanId = entity.ThanhPhanId;
            return dto;
        }

        public async Task<bool> UpdateAsync(Guid id, ThanhPhanDTO dto)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null) return false;

            // Kiểm tra trùng tên (trừ chính nó)
            var allExisting = await _repository.GetAllAsync();
            if (allExisting.Any(x => x.ThanhPhanId != id && x.TenThanhPhan.ToLower().Trim() == dto.TenThanhPhan.ToLower().Trim()))
            {
                throw new InvalidOperationException("Tên thành phần đã tồn tại.");
            }

            entity.TenThanhPhan = dto.TenThanhPhan;
            entity.MoTa = dto.MoTa;
            entity.TrangThai = dto.TrangThai;

            await _repository.UpdateAsync(entity);
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
