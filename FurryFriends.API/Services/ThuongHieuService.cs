
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
    public class ThuongHieuService : IThuongHieuService
    {
        private readonly IThuongHieuRepository _repository;

        public ThuongHieuService(IThuongHieuRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<ThuongHieuDTO>> GetAllAsync()
        {
            var entities = await _repository.GetAllAsync();
            return entities.Select(x => new ThuongHieuDTO
            {
                ThuongHieuId = x.ThuongHieuId,
                TenThuongHieu = x.TenThuongHieu,
                Email = x.Email,
                SDT = x.SDT,
                DiaChi = x.DiaChi,
                MoTa = x.MoTa,
                TrangThai = x.TrangThai
            });
        }

        public async Task<ThuongHieuDTO> GetByIdAsync(Guid id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null) return null;

            return new ThuongHieuDTO
            {
                ThuongHieuId = entity.ThuongHieuId,
                TenThuongHieu = entity.TenThuongHieu,
                Email = entity.Email,
                SDT = entity.SDT,
                DiaChi = entity.DiaChi,
                MoTa = entity.MoTa,
                TrangThai = entity.TrangThai
            };
        }

        public async Task<ThuongHieuDTO> CreateAsync(ThuongHieuDTO dto)
        {
            var entity = new ThuongHieu
            {
                ThuongHieuId = Guid.NewGuid(),
                TenThuongHieu = dto.TenThuongHieu,
                Email = dto.Email,
                SDT = dto.SDT,
                DiaChi = dto.DiaChi,
                MoTa = dto.MoTa,
                TrangThai = dto.TrangThai
            };

            await _repository.AddAsync(entity);
            dto.ThuongHieuId = entity.ThuongHieuId;
            return dto;
        }

        public async Task<bool> UpdateAsync(Guid id, ThuongHieuDTO dto)
        {
            var existing = await _repository.GetByIdAsync(id);
            if (existing == null) return false;

            existing.TenThuongHieu = dto.TenThuongHieu;
            existing.Email = dto.Email;
            existing.SDT = dto.SDT;
            existing.DiaChi = dto.DiaChi;
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
