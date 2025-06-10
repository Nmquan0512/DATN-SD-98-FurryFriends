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
	public class ThuongHieuRepository : IThuongHieuRepository
	{
		private readonly AppDbContext _context;
		private readonly DbSet<ThuongHieu> _dbSet;

		public ThuongHieuRepository(AppDbContext context)
		{
			_context = context;
			_dbSet = _context.Set<ThuongHieu>();
		}

		public async Task<IEnumerable<ThuongHieu>> GetAllAsync()
		{
			return await _dbSet.ToListAsync();
		}

		public async Task<ThuongHieu?> GetByIdAsync(Guid id)
		{
			return await _dbSet.FindAsync(id);
		}

		public async Task<IEnumerable<ThuongHieu>> FindAsync(Expression<Func<ThuongHieu, bool>> predicate)
		{
			return await _dbSet.Where(predicate).ToListAsync();
		}

		public async Task AddAsync(ThuongHieu entity)
		{
			await _dbSet.AddAsync(entity);
		}

		public void Update(ThuongHieu entity)
		{
			_dbSet.Update(entity);
		}

		public void Delete(ThuongHieu entity)
		{
			_dbSet.Remove(entity);
		}

		public async Task SaveAsync()
		{
			await _context.SaveChangesAsync();
		}

		public async Task<IEnumerable<ThuongHieu>> GetActiveBrandsAsync()
		{
			return await _dbSet.Where(x => x.TrangThai).ToListAsync();
		}
	}
}
