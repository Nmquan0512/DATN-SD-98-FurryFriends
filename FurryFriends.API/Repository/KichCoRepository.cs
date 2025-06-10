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
	public class KichCoRepository : IKichCoRepository
	{
		private readonly AppDbContext _context;
		private readonly DbSet<KichCo> _dbSet;

		public KichCoRepository(AppDbContext context)
		{
			_context = context;
			_dbSet = _context.Set<KichCo>();
		}

		public async Task<IEnumerable<KichCo>> GetAllAsync()
		{
			return await _dbSet.ToListAsync();
		}

		public async Task<KichCo?> GetByIdAsync(Guid id)
		{
			return await _dbSet.FirstOrDefaultAsync(x => x.KichCoId == id);
		}

		public async Task<IEnumerable<KichCo>> FindAsync(Expression<Func<KichCo, bool>> predicate)
		{
			return await _dbSet.Where(predicate).ToListAsync();
		}

		public async Task AddAsync(KichCo entity)
		{
			await _dbSet.AddAsync(entity);
		}

		public void Update(KichCo entity)
		{
			_dbSet.Update(entity);
		}

		public void Delete(KichCo entity)
		{
			_dbSet.Remove(entity);
		}

		public async Task SaveAsync()
		{
			await _context.SaveChangesAsync();
		}
	}
}
