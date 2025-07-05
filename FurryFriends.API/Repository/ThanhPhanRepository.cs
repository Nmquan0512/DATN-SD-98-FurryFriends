using FurryFriends.API.Data;
using FurryFriends.API.Models;
using FurryFriends.API.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FurryFriends.API.Repository
{
    public class ThanhPhanRepository : IThanhPhanRepository
    {
        private readonly AppDbContext _context;

        public ThanhPhanRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ThanhPhan>> GetAllAsync()
        {
            return await _context.ThanhPhans.ToListAsync();
        }

        public async Task<ThanhPhan> GetByIdAsync(Guid id)
        {
            return await _context.ThanhPhans.FindAsync(id);
        }

        public async Task AddAsync(ThanhPhan entity)
        {
            await _context.ThanhPhans.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(ThanhPhan entity)
        {
            _context.ThanhPhans.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var entity = await _context.ThanhPhans.FindAsync(id);
            if (entity != null)
            {
                _context.ThanhPhans.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExistsAsync(Guid id)
        {
            return await _context.ThanhPhans.AnyAsync(x => x.ThanhPhanId == id);
        }
    }
}
