using FurryFriends.API.Data;
using FurryFriends.API.Models;
using FurryFriends.API.Repository.IRepository;
using Microsoft.EntityFrameworkCore;

namespace FurryFriends.API.Repository
{
	
		public class KhachHangRepository : IKhachHangRepository
		{
			private readonly AppDbContext _context;

			public KhachHangRepository(AppDbContext context)
			{
				_context = context;
			}

			public async Task<IEnumerable<KhachHang>> GetAllAsync()
			{
				return await _context.KhachHangs
									 .Include(k => k.DiaChiKhachHangs) // đúng tên
									 .ToListAsync();
			}

			public async Task<KhachHang> GetByIdAsync(Guid id)
			{
				return await _context.KhachHangs
									 .Include(k => k.DiaChiKhachHangs)
									 .FirstOrDefaultAsync(k => k.KhachHangId == id);
			}

			public async Task AddAsync(KhachHang khachHang)
			{
				await _context.KhachHangs.AddAsync(khachHang);
				await _context.SaveChangesAsync();
			}

			public async Task UpdateAsync(KhachHang khachHang)
			{
				_context.KhachHangs.Update(khachHang);
				await _context.SaveChangesAsync();
			}

			public async Task DeleteAsync(Guid id)
			{
				var khachHang = await _context.KhachHangs.FindAsync(id);
				if (khachHang != null)
				{
					_context.KhachHangs.Remove(khachHang);
					await _context.SaveChangesAsync();
				}
			}
		}
}

