using FurryFriends.API.Data;
using FurryFriends.API.Models;
using FurryFriends.API.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FurryFriends.API.Repository
{
    public class KichCoRepository : IKichCoRepository
    {
        private readonly AppDbContext _context;

        public KichCoRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<KichCo>> GetAllAsync()
        {
            return await _context.KichCos.ToListAsync();
        }

        public async Task<KichCo> GetByIdAsync(Guid id)
        {
            return await _context.KichCos.FindAsync(id);
        }

        public async Task AddAsync(KichCo entity)
        {
            await _context.KichCos.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(KichCo entity)
        {
            _context.KichCos.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var entity = await _context.KichCos.FindAsync(id);
            if (entity != null)
            {
                _context.KichCos.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExistsAsync(Guid id)
        {
            return await _context.KichCos.AnyAsync(e => e.KichCoId == id);
        }
    }
}
