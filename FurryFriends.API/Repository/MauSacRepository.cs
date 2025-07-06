using FurryFriends.API.Data;
using FurryFriends.API.Models;
using FurryFriends.API.Repository.IRepository;
using Microsoft.EntityFrameworkCore;

namespace FurryFriends.API.Repository
{
    public class MauSacRepository : IMauSacRepository
    {
        private readonly AppDbContext _context;

        public MauSacRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<MauSac>> GetAllAsync() =>
            await _context.MauSacs.ToListAsync();

        public async Task<MauSac> GetByIdAsync(Guid id) =>
            await _context.MauSacs.FindAsync(id);

        public async Task AddAsync(MauSac entity)
        {
            await _context.MauSacs.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(MauSac entity)
        {
            _context.MauSacs.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var entity = await _context.MauSacs.FindAsync(id);
            if (entity != null)
            {
                _context.MauSacs.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExistsAsync(Guid id) =>
            await _context.MauSacs.AnyAsync(x => x.MauSacId == id);
    }
}