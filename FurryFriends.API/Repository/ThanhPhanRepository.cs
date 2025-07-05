using FurryFriends.API.Data;
using FurryFriends.API.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace FurryFriends.API.Repositories
{
	public class ThanhPhanRepository : IThanhPhanRepository
	{
		private readonly AppDbContext _context;
		private readonly DbSet<ThanhPhan> _dbSet;

		public ThanhPhanRepository(AppDbContext context)
		{
			_context = context;
			_dbSet = _context.Set<ThanhPhan>();
		}

		public async Task<IEnumerable<ThanhPhan>> GetAllAsync()
		{
			return await _dbSet.ToListAsync();
		}

		public async Task<ThanhPhan?> GetByIdAsync(Guid id)
		{
			return await _dbSet.FindAsync(id);
		}

		public async Task<IEnumerable<ThanhPhan>> FindAsync(Expression<Func<ThanhPhan, bool>> predicate)
		{
			return await _dbSet.Where(predicate).ToListAsync();
		}

		public async Task AddAsync(ThanhPhan entity)
		{
			await _dbSet.AddAsync(entity);
		}

		public void Update(ThanhPhan entity)
		{
			_dbSet.Update(entity);
		}

		public void Delete(ThanhPhan entity)
		{
			_dbSet.Remove(entity);
		}

		public async Task SaveAsync()
		{
			await _context.SaveChangesAsync();
		}

		public async Task<IEnumerable<ThanhPhan>> GetActiveAsync()
		{
			return await _dbSet.Where(x => x.TrangThai).ToListAsync();
		}
	}
}
