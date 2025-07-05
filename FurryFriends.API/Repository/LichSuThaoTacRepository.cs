using FurryFriends.API.Data;
using FurryFriends.API.Models;
using FurryFriends.API.Repository.IRepository;
using Microsoft.EntityFrameworkCore;

namespace FurryFriends.API.Repository
{
    public class LichSuThaoTacRepository : ILichSuThaoTacRepository
    {
        private readonly AppDbContext _context;
        public LichSuThaoTacRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(LichSuThaoTac log)
        {
            await _context.LichSuThaoTacs.AddAsync(log);
            await _context.SaveChangesAsync();
        }

        public async Task<List<LichSuThaoTac>> GetRecentAsync(int take)
        {
            try
            {
                return await _context.LichSuThaoTacs
                    .OrderByDescending(x => x.ThoiGian)
                    .Take(take)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi truy vấn DB: " + ex.Message);
                throw;
            }
        }
    }
}
