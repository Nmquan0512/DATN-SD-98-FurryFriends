using FurryFriends.API.Data;
using FurryFriends.API.Models;
using FurryFriends.API.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace FurryFriends.API.Repositories
{
	public class SanPhamChatLieuRepository : ISanPhamChatLieuRepository
	{
		private readonly AppDbContext _context;
		private readonly DbSet<SanPhamChatLieu> _dbSet;

		public SanPhamChatLieuRepository(AppDbContext context)
		{
			_context = context;
			_dbSet = _context.Set<SanPhamChatLieu>();
		}

		public async Task<IEnumerable<SanPhamChatLieu>> GetAllAsync()
		{
			return await _dbSet
				.Include(sp => sp.ChatLieu)
				.Include(sp => sp.SanPham)
				.ToListAsync();
		}

		public async Task<SanPhamChatLieu?> GetByIdAsync(Guid id)
		{
			return await _dbSet
				.Include(sp => sp.ChatLieu)
				.Include(sp => sp.SanPham)
				.FirstOrDefaultAsync(x => x.SanPhamChatLieuId == id);
		}

		public async Task<IEnumerable<SanPhamChatLieu>> FindAsync(Expression<Func<SanPhamChatLieu, bool>> predicate)
		{
			return await _dbSet
				.Include(sp => sp.ChatLieu)
				.Include(sp => sp.SanPham)
				.Where(predicate)
				.ToListAsync();
		}

		public async Task AddAsync(SanPhamChatLieu entity)
		{
			await _dbSet.AddAsync(entity);
		}

		public void Update(SanPhamChatLieu entity)
		{
			_dbSet.Update(entity);
		}

		public void Delete(SanPhamChatLieu entity)
		{
			_dbSet.Remove(entity);
		}

		public async Task SaveAsync()
		{
			await _context.SaveChangesAsync();
		}

		public async Task<IEnumerable<SanPhamChatLieu>> GetBySanPhamIdAsync(Guid sanPhamId)
		{
			return await _dbSet
				.Include(sp => sp.ChatLieu)
				.Where(x => x.SanPhamId == sanPhamId)
				.ToListAsync();
		}

		public async Task<IEnumerable<SanPhamChatLieu>> GetByChatLieuIdAsync(Guid chatLieuId)
		{
			return await _dbSet
				.Include(sp => sp.SanPham)
				.Where(x => x.ChatLieuId == chatLieuId)
				.ToListAsync();
		}

		public async Task DeleteBySanPhamIdAsync(Guid sanPhamId)
		{
			var entities = await _dbSet.Where(x => x.SanPhamId == sanPhamId).ToListAsync();
			_dbSet.RemoveRange(entities);
		}

		public async Task DeleteByChatLieuIdAsync(Guid chatLieuId)
		{
			var entities = await _dbSet.Where(x => x.ChatLieuId == chatLieuId).ToListAsync();
			_dbSet.RemoveRange(entities);
		}
	}
}
