using FurryFriends.API.Data;
using FurryFriends.API.Models;
using Microsoft.EntityFrameworkCore;
using FurryFriends.API.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FurryFriends.API.Repository
{
    public class ChatLieuRepository : IChatLieuRepository
    {
        private readonly AppDbContext _context;

        public ChatLieuRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ChatLieu>> GetAllAsync()
        {
            return await _context.ChatLieus.ToListAsync();
        }

        public async Task<ChatLieu> GetByIdAsync(Guid id)
        {
            return await _context.ChatLieus.FindAsync(id);
        }

        public async Task AddAsync(ChatLieu entity)
        {
            await _context.ChatLieus.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(ChatLieu entity)
        {
            _context.ChatLieus.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var entity = await _context.ChatLieus.FindAsync(id);
            if (entity != null)
            {
                _context.ChatLieus.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExistsAsync(Guid id)
        {
            return await _context.ChatLieus.AnyAsync(e => e.ChatLieuId == id);
        }
    }
}