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
    public class ChatLieuService : IChatLieuService
    {
        private readonly IChatLieuRepository _repository;

        public ChatLieuService(IChatLieuRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<ChatLieuDTO>> GetAllAsync()
        {
            var entities = await _repository.GetAllAsync();
            return entities.Select(x => new ChatLieuDTO
            {
                ChatLieuId = x.ChatLieuId,
                TenChatLieu = x.TenChatLieu,
                MoTa = x.MoTa,
                TrangThai = x.TrangThai
            });
        }

        public async Task<ChatLieuDTO> GetByIdAsync(Guid id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null) return null;

            return new ChatLieuDTO
            {
                ChatLieuId = entity.ChatLieuId,
                TenChatLieu = entity.TenChatLieu,
                MoTa = entity.MoTa,
                TrangThai = entity.TrangThai
            };
        }

        public async Task<ChatLieuDTO> CreateAsync(ChatLieuDTO dto)
        {
            var entity = new ChatLieu
            {
                ChatLieuId = Guid.NewGuid(),
                TenChatLieu = dto.TenChatLieu,
                MoTa = dto.MoTa,
                TrangThai = dto.TrangThai
            };

            await _repository.AddAsync(entity);

            dto.ChatLieuId = entity.ChatLieuId;
            return dto;
        }

        public async Task<bool> UpdateAsync(Guid id, ChatLieuDTO dto)
        {
            var existing = await _repository.GetByIdAsync(id);
            if (existing == null) return false;

            existing.TenChatLieu = dto.TenChatLieu;
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
