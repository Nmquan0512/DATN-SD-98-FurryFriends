using FurryFriends.API.Data;
using FurryFriends.API.Models;
using FurryFriends.API.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace FurryFriends.API.Services
{
	public class SanPhamService : ISanPhamService
	{
		private readonly AppDbContext _context;

		public SanPhamService(AppDbContext context)
		{
			_context = context;
		}

		public async Task<(List<SanPhamViewModel> Data, int TotalCount)> GetSanPhamViewAsync(string? search, int page, int pageSize)
		{
			var query = _context.SanPhams.Include(sp => sp.SanPhamChiTiets).AsQueryable();

			if (!string.IsNullOrWhiteSpace(search))
			{
				query = query.Where(sp => sp.TenSanPham.Contains(search));
			}

			var totalCount = await query.CountAsync();

			var data = await query
				.OrderBy(sp => sp.TenSanPham)
				.Skip((page - 1) * pageSize)
				.Take(pageSize)
				.ToListAsync();

			var result = data.Select((sp, index) => new SanPhamViewModel
			{
				STT = (page - 1) * pageSize + index + 1,
				SanPhamId = sp.SanPhamId,
				TenSanPham = sp.TenSanPham,
				SoLuong = sp.SanPhamChiTiets.Sum(ct => ct.SoLuong),
				Gia = sp.SanPhamChiTiets.Min(ct => (decimal?)ct.Gia) ?? 0
			}).ToList();

			return (result, totalCount);
		}

		public async Task<SanPhamViewModel?> GetByIdAsync(Guid id)
		{
			var sp = await _context.SanPhams
				.Include(sp => sp.SanPhamChiTiets)
				.FirstOrDefaultAsync(sp => sp.SanPhamId == id);

			if (sp == null) return null;

			return new SanPhamViewModel
			{
				STT = 1,
				SanPhamId = sp.SanPhamId,
				TenSanPham = sp.TenSanPham,
				SoLuong = sp.SanPhamChiTiets.Sum(ct => ct.SoLuong),
				Gia = sp.SanPhamChiTiets.Min(ct => (decimal?)ct.Gia) ?? 0
			};
		}

		public async Task<SanPham> CreateAsync(SanPham model)
		{
			model.SanPhamId = Guid.NewGuid();
			_context.SanPhams.Add(model);
			await _context.SaveChangesAsync();
			return model;
		}

		public async Task<bool> UpdateAsync(Guid id, SanPham model)
		{
			var sp = await _context.SanPhams.FindAsync(id);
			if (sp == null) return false;

			sp.TenSanPham = model.TenSanPham;
			sp.TaiKhoanId = model.TaiKhoanId;
			sp.ThuongHieuId = model.ThuongHieuId;
			sp.TrangThai = model.TrangThai;

			_context.SanPhams.Update(sp);
			await _context.SaveChangesAsync();
			return true;
		}

		public async Task<bool> DeleteAsync(Guid id)
		{
			var sp = await _context.SanPhams.FindAsync(id);
			if (sp == null) return false;

			_context.SanPhams.Remove(sp);
			await _context.SaveChangesAsync();
			return true;
		}
	}
}
