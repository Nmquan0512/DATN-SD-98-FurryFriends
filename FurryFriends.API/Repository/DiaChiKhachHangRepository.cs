using FurryFriends.API.Data;
using FurryFriends.API.Models;
using FurryFriends.API.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
namespace FurryFriends.API.Models
{
	public class DiaChiKhachHangRepository : IDiaChiKhachHangRepository
	{
		private readonly AppDbContext _context;

		public DiaChiKhachHangRepository(AppDbContext context)
		{
			_context = context;
		}

		public async Task<IEnumerable<DiaChiKhachHang>> GetAllAsync()
		{
			return await _context.DiaChiKhachHangs
				.Include(dc => dc.KhachHangs)
				.ToListAsync();
		}

		public async Task<DiaChiKhachHang> GetByIdAsync(Guid id)
		{
			return await _context.DiaChiKhachHangs
				.Include(dc => dc.KhachHangs)
				.FirstOrDefaultAsync(dc => dc.DiaChiId == id);
		}

		public async Task<IEnumerable<DiaChiKhachHang>> GetByKhachHangIdAsync(Guid khachHangId)
		{
			return await _context.DiaChiKhachHangs
				.Where(dc => dc.KhachHangId == khachHangId)
				.ToListAsync();
		}

		public async Task AddAsync(DiaChiKhachHang diaChi)
		{
			await _context.DiaChiKhachHangs.AddAsync(diaChi);
			await _context.SaveChangesAsync();
		}

		public async Task UpdateAsync(DiaChiKhachHang diaChi)
		{
			_context.DiaChiKhachHangs.Update(diaChi);
			await _context.SaveChangesAsync();
		}

		public async Task DeleteAsync(Guid id)
		{
			var diaChi = await _context.DiaChiKhachHangs.FindAsync(id);
			if (diaChi != null)
			{
				_context.DiaChiKhachHangs.Remove(diaChi);
				await _context.SaveChangesAsync();
			}
		}
	}
}