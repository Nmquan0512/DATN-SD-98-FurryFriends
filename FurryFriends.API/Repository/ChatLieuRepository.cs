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
	public class ChatLieuRepository : IChatLieuRepository
	{
		private readonly AppDbContext _context;
		private readonly DbSet<ChatLieu> _dbSet;

		public ChatLieuRepository(AppDbContext context)
		{
			_context = context;
			_dbSet = _context.Set<ChatLieu>();
		}

		public async Task<IEnumerable<ChatLieu>> GetAllAsync()
		{
			return await _dbSet.ToListAsync();
		}

		public async Task<ChatLieu?> GetByIdAsync(Guid id)
		{
			return await _dbSet.FirstOrDefaultAsync(x => x.ChatLieuId == id);
		}

		public async Task<IEnumerable<ChatLieu>> FindAsync(Expression<Func<ChatLieu, bool>> predicate)
		{
			return await _dbSet.Where(predicate).ToListAsync();
		}

		public async Task AddAsync(ChatLieu entity)
		{
			await _dbSet.AddAsync(entity);
		}

		public void Update(ChatLieu entity)
		{
			_dbSet.Update(entity);
		}

		public void Delete(ChatLieu entity)
		{
			_dbSet.Remove(entity);
		}

		public async Task SaveAsync()
		{
			await _context.SaveChangesAsync();
		}
	}
}
