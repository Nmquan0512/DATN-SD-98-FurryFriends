using FurryFriends.API.Data;
using FurryFriends.API.Models;
using FurryFriends.API.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FurryFriends.API.Repository
{
    public class ThongBaoRepository : IThongBaoRepository
    {
        private readonly AppDbContext _context;
        public ThongBaoRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<ThongBao>> GetAllAsync()
        {
            return await _context.ThongBaos.OrderByDescending(tb => tb.NgayTao).ToListAsync();
        }
        public async Task<ThongBao> GetByIdAsync(Guid id)
        {
            return await _context.ThongBaos.FindAsync(id);
        }
        public async Task AddAsync(ThongBao thongBao)
        {
            thongBao.ThongBaoId = Guid.NewGuid();
            thongBao.NgayTao = DateTime.Now;
            _context.ThongBaos.Add(thongBao);
            await _context.SaveChangesAsync();
        }
        public async Task UpdateAsync(ThongBao thongBao)
        {
            _context.ThongBaos.Update(thongBao);
            await _context.SaveChangesAsync();
        }
        public async Task DeleteAsync(Guid id)
        {
            var tb = await _context.ThongBaos.FindAsync(id);
            if (tb != null)
            {
                _context.ThongBaos.Remove(tb);
                await _context.SaveChangesAsync();
            }
        }
        public async Task MarkAsReadAsync(Guid id)
        {
            var tb = await _context.ThongBaos.FindAsync(id);
            if (tb != null && !tb.DaDoc)
            {
                tb.DaDoc = true;
                await _context.SaveChangesAsync();
            }
        }
    }
} 