using FurryFriends.API.Data;
using FurryFriends.API.Models;
using FurryFriends.API.Services;
using Microsoft.EntityFrameworkCore;

namespace FurryFriends.Web.Services
{

	namespace FurryFriends.API.Services
	{
		public class AnhService : IAnhService
		{
			private readonly AppDbContext _context;

			public AnhService(AppDbContext context)
			{
				_context = context;
			}

			public async Task<IEnumerable<Anh>> GetAllAsync() => await _context.Anhs.ToListAsync();

			public async Task<Anh?> GetByIdAsync(Guid id) => await _context.Anhs.FindAsync(id);

			public async Task<Anh> CreateAsync(Anh model)
			{
				model.AnhId = Guid.NewGuid();
				_context.Anhs.Add(model);
				await _context.SaveChangesAsync();
				return model;
			}

			public async Task<bool> UpdateAsync(Guid id, Anh model)
			{
				var item = await _context.Anhs.FindAsync(id);
				if (item == null) return false;
				item.TenAnh = model.TenAnh;
				item.DuongDan = model.DuongDan;
				item.TrangThai = model.TrangThai;
				await _context.SaveChangesAsync();
				return true;
			}

			public async Task<bool> DeleteAsync(Guid id)
			{
				var item = await _context.Anhs.FindAsync(id);
				if (item == null) return false;
				_context.Anhs.Remove(item);
				await _context.SaveChangesAsync();
				return true;
			}
		}
	}

}
