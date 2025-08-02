using FurryFriends.API.Data;
using FurryFriends.API.Models;
using FurryFriends.API.Repository.IRepository;
using Microsoft.EntityFrameworkCore;

namespace FurryFriends.API.Repository
{
    public class HinhThucThanhToanRepository : IHinhThucThanhToanRepository
    {
        private readonly AppDbContext _context;

        public HinhThucThanhToanRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<HinhThucThanhToan>> GetAllAsync()
        {
            return await _context.HinhThucThanhToans.ToListAsync();
        }
    }

}
