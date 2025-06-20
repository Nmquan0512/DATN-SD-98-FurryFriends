using FurryFriends.API.Data;
using FurryFriends.API.Models;
using FurryFriends.Web.Services.IServices;
using Microsoft.EntityFrameworkCore;

namespace FurryFriends.API.Services
{
	public class MauSacService : IMauSacService
	{
		private readonly AppDbContext _context;

		public MauSacService(AppDbContext context)
		{
			_context = context;
		}

		public async Task<IEnumerable<MauSac>> GetAllAsync() => await _context.MauSacs.ToListAsync();

		public async Task<MauSac?> GetByIdAsync(Guid id) => await _context.MauSacs.FindAsync(id);

		public async Task<MauSac> CreateAsync(MauSac model)
		{
			model.MauSacId = Guid.NewGuid();
			_context.MauSacs.Add(model);
			await _context.SaveChangesAsync();
			return model;
		}

		public async Task<bool> UpdateAsync(Guid id, MauSac model)
		{
			var item = await _context.MauSacs.FindAsync(id);
			if (item == null) return false;
			item.TenMau = model.TenMau;
			item.MoTa = model.MoTa;
			item.TrangThai = model.TrangThai;
			await _context.SaveChangesAsync();
			return true;
		}

		public async Task<bool> DeleteAsync(Guid id)
		{
			var item = await _context.MauSacs.FindAsync(id);
			if (item == null) return false;
			_context.MauSacs.Remove(item);
			await _context.SaveChangesAsync();
			return true;
		}
	}
}