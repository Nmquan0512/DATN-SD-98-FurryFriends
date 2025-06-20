using FurryFriends.API.Data;
using FurryFriends.API.Models;
using FurryFriends.Web.Services.IServices;
using Microsoft.EntityFrameworkCore;


namespace FurryFriends.API.Services
{
	public class KichCoService : IKichCoService
	{
		private readonly AppDbContext _context;

		public KichCoService(AppDbContext context)
		{
			_context = context;
		}

		public async Task<IEnumerable<KichCo>> GetAllAsync() => await _context.KichCos.ToListAsync();

		public async Task<KichCo?> GetByIdAsync(Guid id) => await _context.KichCos.FindAsync(id);

		public async Task<KichCo> CreateAsync(KichCo model)
		{
			model.KichCoId = Guid.NewGuid();
			_context.KichCos.Add(model);
			await _context.SaveChangesAsync();
			return model;
		}

		public async Task<bool> UpdateAsync(Guid id, KichCo model)
		{
			var item = await _context.KichCos.FindAsync(id);
			if (item == null) return false;
			item.TenKichCo = model.TenKichCo;
			item.MoTa = model.MoTa;
			item.TrangThai = model.TrangThai;
			await _context.SaveChangesAsync();
			return true;
		}

		public async Task<bool> DeleteAsync(Guid id)
		{
			var item = await _context.KichCos.FindAsync(id);
			if (item == null) return false;
			_context.KichCos.Remove(item);
			await _context.SaveChangesAsync();
			return true;
		}
	}
}