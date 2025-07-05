using FurryFriends.API.Data;
using FurryFriends.API.Models;
using FurryFriends.API.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace FurryFriends.API.Repositories
{
	public class MauSacRepository : IMauSacRepository
	{
		private readonly AppDbContext _context;
		private readonly DbSet<MauSac> _dbSet;

		public MauSacRepository(AppDbContext context)
		{
			_context = context;
			_dbSet = _context.Set<MauSac>();
		}

		public async Task<IEnumerable<MauSac>> GetAllAsync()
		{
			return await _dbSet.ToListAsync();
		}

		public async Task<MauSac?> GetByIdAsync(Guid id)
		{
			return await _dbSet.FirstOrDefaultAsync(x => x.MauSacId == id);
		}

		public async Task<IEnumerable<MauSac>> FindAsync(Expression<Func<MauSac, bool>> predicate)
		{
			return await _dbSet.Where(predicate).ToListAsync();
		}

		public async Task AddAsync(MauSac entity)
		{
			await _dbSet.AddAsync(entity);
		}

		public void Update(MauSac entity)
		{
			_dbSet.Update(entity);
		}

		public void Delete(MauSac entity)
		{
			_dbSet.Remove(entity);
		}

		public async Task SaveAsync()
		{
			await _context.SaveChangesAsync();
		}
	}
}
