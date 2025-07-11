using FurryFriends.API.Data;
using FurryFriends.API.Models;
using FurryFriends.API.Repositories;
using FurryFriends.API.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FurryFriends.API.Repository
{
    public class ThuongHieuRepository : IThuongHieuRepository
    {
        private readonly AppDbContext _context;

        public ThuongHieuRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ThuongHieu>> GetAllAsync()
        {
            return await _context.ThuongHieus.ToListAsync();
        }

        public async Task<ThuongHieu> GetByIdAsync(Guid id)
        {
            return await _context.ThuongHieus.FindAsync(id);
        }

        public async Task AddAsync(ThuongHieu entity)
        {
            await _context.ThuongHieus.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(ThuongHieu entity)
        {
            _context.ThuongHieus.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var entity = await _context.ThuongHieus.FindAsync(id);
            if (entity != null)
            {
                _context.ThuongHieus.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExistsAsync(Guid id)
        {
            return await _context.ThuongHieus.AnyAsync(e => e.ThuongHieuId == id);
        }
    }
}