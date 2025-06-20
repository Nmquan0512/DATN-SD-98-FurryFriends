using FurryFriends.API.Data;
using FurryFriends.API.Services;
using Microsoft.EntityFrameworkCore;

namespace FurryFriends.Web.Services
{
	public class SanPhamChiTietService
	{
	}
	namespace FurryFriends.API.Services
	{
		public class SanPhamChiTietService : ISanPhamChiTietService
		{
			private readonly AppDbContext _context;

			public SanPhamChiTietService(AppDbContext context)
			{
				_context = context;
			}

			public async Task<IEnumerable<SanPhamChiTiet>> GetAllAsync()
			{
				return await _context.SanPhamChiTiets
					.Include(x => x.SanPham)
					.Include(x => x.KichCo)
					.Include(x => x.MauSac)
					.Include(x => x.Anh)
					.ToListAsync();
			}

			public async Task<SanPhamChiTiet?> GetByIdAsync(Guid id)
			{
				return await _context.SanPhamChiTiets
					.Include(x => x.SanPham)
					.Include(x => x.KichCo)
					.Include(x => x.MauSac)
					.Include(x => x.Anh)
					.FirstOrDefaultAsync(x => x.SanPhamChiTietId == id);
			}

			public async Task<IEnumerable<SanPhamChiTiet>> GetBySanPhamIdAsync(Guid sanPhamId)
			{
				return await _context.SanPhamChiTiets
					.Where(x => x.SanPhamId == sanPhamId)
					.Include(x => x.KichCo)
					.Include(x => x.MauSac)
					.Include(x => x.Anh)
					.ToListAsync();
			}

			public async Task<object> GetPagingAsync(Guid? sanPhamId, string? search, int page, int pageSize)
			{
				var query = _context.SanPhamChiTiets
					.Include(x => x.SanPham)
					.Include(x => x.KichCo)
					.Include(x => x.MauSac)
					.Include(x => x.Anh)
					.AsQueryable();

				if (sanPhamId.HasValue)
					query = query.Where(x => x.SanPhamId == sanPhamId);

				if (!string.IsNullOrWhiteSpace(search))
					query = query.Where(x => x.MoTa.Contains(search) || x.SanPham.TenSanPham.Contains(search));

				var total = await query.CountAsync();
				var items = await query.OrderByDescending(x => x.NgayTao).Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

				return new { TotalItems = total, Page = page, PageSize = pageSize, Items = items };
			}

			public async Task<SanPhamChiTiet> CreateAsync(SanPhamChiTiet model)
			{
				model.SanPhamChiTietId = Guid.NewGuid();
				model.NgayTao = DateTime.Now;
				_context.SanPhamChiTiets.Add(model);
				await _context.SaveChangesAsync();
				return model;
			}

			public async Task<bool> UpdateAsync(Guid id, SanPhamChiTiet model)
			{
				var item = await _context.SanPhamChiTiets.FindAsync(id);
				if (item == null) return false;

				item.SanPhamId = model.SanPhamId;
				item.KichCoId = model.KichCoId;
				item.MauSacId = model.MauSacId;
				item.AnhId = model.AnhId;
				item.SoLuong = model.SoLuong;
				item.Gia = model.Gia;
				item.MoTa = model.MoTa;
				item.TrangThai = model.TrangThai;
				item.NgaySua = DateTime.Now;

				_context.SanPhamChiTiets.Update(item);
				await _context.SaveChangesAsync();
				return true;
			}

			public async Task<bool> DeleteAsync(Guid id)
			{
				var item = await _context.SanPhamChiTiets.FindAsync(id);
				if (item == null) return false;

				_context.SanPhamChiTiets.Remove(item);
				await _context.SaveChangesAsync();
				return true;
			}

			public async Task<bool> UpdateAnhAsync(Guid id, Guid anhId)
			{
				var item = await _context.SanPhamChiTiets.FindAsync(id);
				if (item == null) return false;
				item.AnhId = anhId;
				item.NgaySua = DateTime.Now;
				await _context.SaveChangesAsync();
				return true;
			}

			public async Task<bool> UpdateMauAsync(Guid id, Guid mauId)
			{
				var item = await _context.SanPhamChiTiets.FindAsync(id);
				if (item == null) return false;
				item.MauSacId = mauId;
				item.NgaySua = DateTime.Now;
				await _context.SaveChangesAsync();
				return true;
			}

			public async Task<bool> UpdateKichCoAsync(Guid id, Guid kichCoId)
			{
				var item = await _context.SanPhamChiTiets.FindAsync(id);
				if (item == null) return false;
				item.KichCoId = kichCoId;
				item.NgaySua = DateTime.Now;
				await _context.SaveChangesAsync();
				return true;
			}

			public async Task<bool> UpdateSoLuongGiaAsync(Guid id, SanPhamChiTiet update)
			{
				var item = await _context.SanPhamChiTiets.FindAsync(id);
				if (item == null) return false;
				item.SoLuong = update.SoLuong;
				item.Gia = update.Gia;
				item.NgaySua = DateTime.Now;
				await _context.SaveChangesAsync();
				return true;
			}
		}
	}

}