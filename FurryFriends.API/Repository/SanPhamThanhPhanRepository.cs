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
	public class SanPhamThanhPhanRepository : ISanPhamThanhPhanRepository
	{
		private readonly AppDbContext _context;
		private readonly DbSet<SanPhamThanhPhan> _dbSet;

		public SanPhamThanhPhanRepository(AppDbContext context)
		{
			_context = context;
			_dbSet = _context.Set<SanPhamThanhPhan>();
		}

		public async Task<IEnumerable<SanPhamThanhPhan>> GetAllAsync()
		{
			return await _dbSet
				.Include(sp => sp.ThanhPhan)
				.Include(sp => sp.SanPham)
				.ToListAsync();
		}

		public async Task<SanPhamThanhPhan?> GetByIdAsync(Guid id)
		{
			return await _dbSet
				.Include(sp => sp.ThanhPhan)
				.Include(sp => sp.SanPham)
				.FirstOrDefaultAsync(x => x.SanPhamThanhPhanId == id);
		}

		public async Task<IEnumerable<SanPhamThanhPhan>> FindAsync(Expression<Func<SanPhamThanhPhan, bool>> predicate)
		{
			return await _dbSet
				.Include(sp => sp.ThanhPhan)
				.Include(sp => sp.SanPham)
				.Where(predicate)
				.ToListAsync();
		}

		public async Task AddAsync(SanPhamThanhPhan entity)
		{
			await _dbSet.AddAsync(entity);
		}

		public void Update(SanPhamThanhPhan entity)
		{
			_dbSet.Update(entity);
		}

		public void Delete(SanPhamThanhPhan entity)
		{
			_dbSet.Remove(entity);
		}

		public async Task SaveAsync()
		{
			await _context.SaveChangesAsync();
		}

		public async Task<IEnumerable<SanPhamThanhPhan>> GetBySanPhamIdAsync(Guid sanPhamId)
		{
			return await _dbSet
				.Include(sp => sp.ThanhPhan)
				.Where(x => x.SanPhamId == sanPhamId)
				.ToListAsync();
		}

		public async Task<IEnumerable<SanPhamThanhPhan>> GetByThanhPhanIdAsync(Guid thanhPhanId)
		{
			return await _dbSet
				.Include(sp => sp.SanPham)
				.Where(x => x.ThanhPhanId == thanhPhanId)
				.ToListAsync();
		}

		public async Task DeleteBySanPhamIdAsync(Guid sanPhamId)
		{
			var entities = await _dbSet.Where(x => x.SanPhamId == sanPhamId).ToListAsync();
			_dbSet.RemoveRange(entities);
		}

		public async Task DeleteByThanhPhanIdAsync(Guid thanhPhanId)
		{
			var entities = await _dbSet.Where(x => x.ThanhPhanId == thanhPhanId).ToListAsync();
			_dbSet.RemoveRange(entities);
		}
	}
}
