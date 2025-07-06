using FurryFriends.API.Models;
using FurryFriends.API.Models.DTO;
using FurryFriends.API.Repository.IRepository;
using FurryFriends.API.Services.IServices;

namespace FurryFriends.API.Services
{
    public class MauSacService : IMauSacService
    {
        private readonly IMauSacRepository _repository;

        public MauSacService(IMauSacRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<MauSacDTO>> GetAllAsync()
        {
            var list = await _repository.GetAllAsync();
            return list.Select(x => new MauSacDTO
            {
                MauSacId = x.MauSacId,
                TenMau = x.TenMau,
                MaMau = x.MaMau,
                MoTa = x.MoTa,
                TrangThai = x.TrangThai
            });
        }

        public async Task<MauSacDTO> GetByIdAsync(Guid id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null) return null;

            return new MauSacDTO
            {
                MauSacId = entity.MauSacId,
                TenMau = entity.TenMau,
                MaMau = entity.MaMau,
                MoTa = entity.MoTa,
                TrangThai = entity.TrangThai
            };
        }

        public async Task<MauSacDTO> CreateAsync(MauSacDTO dto)
        {
            var entity = new MauSac
            {
                MauSacId = Guid.NewGuid(),
                TenMau = dto.TenMau,
                MaMau = dto.MaMau,
                MoTa = dto.MoTa,
                TrangThai = dto.TrangThai
            };

            await _repository.AddAsync(entity);
            dto.MauSacId = entity.MauSacId;
            return dto;
        }

        public async Task<bool> UpdateAsync(Guid id, MauSacDTO dto)
        {
            var existing = await _repository.GetByIdAsync(id);
            if (existing == null) return false;

            existing.TenMau = dto.TenMau;
            existing.MaMau = dto.MaMau;
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
