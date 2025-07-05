using FurryFriends.API.Data;
using FurryFriends.API.Models;
using FurryFriends.API.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FurryFriends.API.Repositories
{
    public class AnhRepository : IAnhRepository
    {
        private readonly AppDbContext _context;

        public AnhRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Anh>> GetAllAsync()
        {
            return await _context.Anhs.ToListAsync();
        }

        public async Task<Anh> GetByIdAsync(Guid id)
        {
            return await _context.Anhs.FindAsync(id);
        }

        public async Task AddAsync(Anh entity)
        {
            await _context.Anhs.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Anh entity)
        {
            _context.Anhs.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var entity = await _context.Anhs.FindAsync(id);
            if (entity != null)
            {
                _context.Anhs.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExistsAsync(Guid id)
        {
            return await _context.Anhs.AnyAsync(a => a.AnhId == id);
        }
    }
}