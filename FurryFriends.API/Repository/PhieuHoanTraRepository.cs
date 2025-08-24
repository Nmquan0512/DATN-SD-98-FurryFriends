using FurryFriends.API.Data;
using FurryFriends.API.Models;
using FurryFriends.API.Repository.IRepository;
using Microsoft.EntityFrameworkCore;

namespace FurryFriends.API.Repository
{
	public class PhieuHoanTraRepository:IPhieuHoanTraRepository
	{
		private readonly AppDbContext _context; // đổi tên nếu DbContext của bạn khác

		public PhieuHoanTraRepository(AppDbContext context)
		{
			_context = context;
		}

		public async Task<PhieuHoanTra?> GetByIdAsync(Guid id, bool includeHoaDonChiTiet = false)
		{
			IQueryable<PhieuHoanTra> q = _context.PhieuHoanTras.AsQueryable();

			if (includeHoaDonChiTiet)
			{
				q = q.Include(p => p.HoaDonChiTiet);
			}

			return await q.AsNoTracking().FirstOrDefaultAsync(p => p.PhieuHoanTraId == id);
		}

		public async Task<IReadOnlyList<PhieuHoanTra>> GetByHoaDonIdAsync(Guid hoaDonId)
		{
			return await _context.PhieuHoanTras
				.Include(p => p.HoaDonChiTiet)
				.Where(p => p.HoaDonChiTiet != null && p.HoaDonChiTiet.HoaDonId == hoaDonId)
				.OrderByDescending(p => p.NgayHoanTra)
				.AsNoTracking()
				.ToListAsync();
		}


		public async Task<IReadOnlyList<PhieuHoanTra>> GetByHoaDonChiTietIdAsync(Guid hoaDonChiTietId)
		{
			return await _context.PhieuHoanTras
				.Where(p => p.HoaDonChiTietId == hoaDonChiTietId)
				.OrderByDescending(p => p.NgayHoanTra)
				.AsNoTracking()
				.ToListAsync();
		}

		public async Task<int> GetTongSoLuongHoanByHdctAsync(Guid hoaDonChiTietId)
		{
			// tổng tất cả phiếu (kể cả trạng thái). Nếu bạn chỉ muốn tính những phiếu "đã duyệt/đã hoàn",
			// hãy thêm điều kiện p.TrangThai == 1 || p.TrangThai == 3
			return await _context.PhieuHoanTras
				.Where(p => p.HoaDonChiTietId == hoaDonChiTietId)
				.SumAsync(p => (int?)p.SoLuongHoan) ?? 0;
		}

		public async Task<PhieuHoanTra> AddAsync(PhieuHoanTra entity)
		{
			if (entity.PhieuHoanTraId == Guid.Empty)
				entity.PhieuHoanTraId = Guid.NewGuid();

			await _context.PhieuHoanTras.AddAsync(entity);
			await _context.SaveChangesAsync();
			return entity;
		}

		public async Task UpdateAsync(PhieuHoanTra entity)
		{
			_context.PhieuHoanTras.Update(entity);
			await _context.SaveChangesAsync();
		}

		public async Task UpdateTrangThaiAsync(Guid id, int trangThai)
		{
			var phieu = await _context.PhieuHoanTras.FirstOrDefaultAsync(p => p.PhieuHoanTraId == id);
			if (phieu == null) throw new KeyNotFoundException("Không tìm thấy phiếu hoàn trả.");

			phieu.TrangThai = trangThai;
			await _context.SaveChangesAsync();
		}

		public async Task DeleteAsync(Guid id)
		{
			var phieu = await _context.PhieuHoanTras.FirstOrDefaultAsync(p => p.PhieuHoanTraId == id);
			if (phieu == null) return;

			_context.PhieuHoanTras.Remove(phieu);
			await _context.SaveChangesAsync();
		}

		public async Task<bool> ExistsAsync(Guid id)
		{
			return await _context.PhieuHoanTras.AnyAsync(p => p.PhieuHoanTraId == id);
		}
	}
}
