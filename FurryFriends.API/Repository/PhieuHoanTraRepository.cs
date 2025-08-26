using FurryFriends.API.Data;
using FurryFriends.API.Models;
using FurryFriends.API.Repository.IRepository;
using Microsoft.EntityFrameworkCore;

namespace FurryFriends.API.Repository
{
	public class PhieuHoanTraRepository:IPhieuHoanTraRepository
	{
		private readonly AppDbContext _context;

		public PhieuHoanTraRepository(AppDbContext context)
		{
			_context = context;
		}

		public async Task<IEnumerable<PhieuHoanTra>> GetAllAsync()
		{
			return await _context.PhieuHoanTras
				.Include(p => p.HoaDonChiTiet)
				.ThenInclude(ct => ct.HoaDon)               
				.Include(p => p.HoaDonChiTiet)
				.ThenInclude(ct => ct.PhieuHoanTras)        
				.ToListAsync();
		}

		public async Task<PhieuHoanTra> GetByIdAsync(Guid id)
		{
			return await _context.PhieuHoanTras
			.Include(p => p.HoaDonChiTiet)
			.ThenInclude(ct => ct.HoaDon)
			.Include(p => p.HoaDonChiTiet)
			.ThenInclude(ct => ct.PhieuHoanTras)       
			.FirstOrDefaultAsync(p => p.PhieuHoanTraId == id);
		}

		public async Task<IEnumerable<PhieuHoanTra>> GetByKhachHangAsync(Guid khachHangId)
		{
			return await _context.PhieuHoanTras
			.Include(p => p.HoaDonChiTiet)
			.ThenInclude(ct => ct.HoaDon)
			.Include(p => p.HoaDonChiTiet)
			.ThenInclude(ct => ct.PhieuHoanTras)       
			.Where(p => p.HoaDonChiTiet.HoaDon.KhachHangId == khachHangId)
			.ToListAsync();
		}

		public async Task AddAsync(PhieuHoanTra entity)
		{
			_context.PhieuHoanTras.Add(entity);
			await _context.SaveChangesAsync();
		}

		public async Task UpdateAsync(PhieuHoanTra entity)
		{
			_context.PhieuHoanTras.Update(entity);
			await _context.SaveChangesAsync();
		}

		public async Task DeleteAsync(Guid id)
		{
			var entity = await _context.PhieuHoanTras.FindAsync(id);
			if (entity != null)
			{
				_context.PhieuHoanTras.Remove(entity);
				await _context.SaveChangesAsync();
			}
		}

		// ✅ Lấy đầy đủ chi tiết hóa đơn + sản phẩm + khách hàng
		public async Task<HoaDonChiTiet?> GetHoaDonChiTietWithRelationsAsync(Guid hoaDonChiTietId)
		{
			return await _context.HoaDonChiTiets
				.Include(ct => ct.HoaDon)
					.ThenInclude(h => h.KhachHang)
				.Include(ct => ct.SanPhamChiTiet)
				.FirstOrDefaultAsync(ct => ct.HoaDonChiTietId == hoaDonChiTietId);
		}

		// ✅ Tính tổng số lượng đã hoàn trước đó
		public async Task<int> GetTongSoLuongDaHoanAsync(Guid hoaDonChiTietId)
		{
			return await _context.PhieuHoanTras
				.Where(p => p.HoaDonChiTietId == hoaDonChiTietId && p.TrangThai != 2) // loại bỏ yêu cầu bị từ chối
				.SumAsync(p => p.SoLuongHoan);
		}
	}
}
