using FurryFriends.API.Data;
using FurryFriends.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FurryFriends.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class SanPhamChiTietController : ControllerBase
	{
		private readonly AppDbContext _context;

		public SanPhamChiTietController(AppDbContext context)
		{
			_context = context;
		}

		// ✅ GET: lấy tất cả chi tiết sản phẩm
		[HttpGet]
		public async Task<IActionResult> GetAll()
		{
			var result = await _context.SanPhamChiTiets
				.Include(x => x.SanPham)
				.Include(x => x.KichCo)
				.Include(x => x.MauSac)
				.Include(x => x.Anh)
				.ToListAsync();

			return Ok(result);
		}

		// ✅ GET: lấy 1 chi tiết sản phẩm theo id
		[HttpGet("{id}")]
		public async Task<IActionResult> GetById(Guid id)
		{
			var item = await _context.SanPhamChiTiets
				.Include(x => x.SanPham)
				.Include(x => x.KichCo)
				.Include(x => x.MauSac)
				.Include(x => x.Anh)
				.FirstOrDefaultAsync(x => x.SanPhamChiTietId == id);

			return item == null ? NotFound() : Ok(item);
		}

		// ✅ GET: Chi tiết theo SanPhamId
		[HttpGet("by-sanpham/{sanPhamId}")]
		public async Task<IActionResult> GetBySanPhamId(Guid sanPhamId)
		{
			var result = await _context.SanPhamChiTiets
				.Where(x => x.SanPhamId == sanPhamId)
				.Include(x => x.KichCo)
				.Include(x => x.MauSac)
				.Include(x => x.Anh)
				.ToListAsync();

			return Ok(result);
		}

		// ✅ GET: Lọc + Phân trang + Tìm kiếm
		[HttpGet("paging")]
		public async Task<IActionResult> GetPaging(
			Guid? sanPhamId = null,
			string? search = null,
			int page = 1,
			int pageSize = 10)
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
				query = query.Where(x =>
					x.MoTa.Contains(search) ||
					x.SanPham.TenSanPham.Contains(search));

			var total = await query.CountAsync();
			var result = await query
				.OrderByDescending(x => x.NgayTao)
				.Skip((page - 1) * pageSize)
				.Take(pageSize)
				.ToListAsync();

			return Ok(new
			{
				TotalItems = total,
				Page = page,
				PageSize = pageSize,
				Items = result
			});
		}

		// ✅ POST: tạo mới chi tiết sản phẩm
		[HttpPost]
		public async Task<IActionResult> Create([FromBody] SanPhamChiTiet model)
		{
			if (!ModelState.IsValid) return BadRequest(ModelState);

			model.SanPhamChiTietId = Guid.NewGuid();
			model.NgayTao = DateTime.Now;
			_context.SanPhamChiTiets.Add(model);
			await _context.SaveChangesAsync();

			return CreatedAtAction(nameof(GetById), new { id = model.SanPhamChiTietId }, model);
		}

		// ✅ PUT: cập nhật chi tiết sản phẩm
		[HttpPut("{id}")]
		public async Task<IActionResult> Update(Guid id, [FromBody] SanPhamChiTiet model)
		{
			var item = await _context.SanPhamChiTiets.FindAsync(id);
			if (item == null) return NotFound();

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

			return NoContent();
		}

		// ✅ DELETE: xóa chi tiết sản phẩm
		[HttpDelete("{id}")]
		public async Task<IActionResult> Delete(Guid id)
		{
			var item = await _context.SanPhamChiTiets.FindAsync(id);
			if (item == null) return NotFound();

			_context.SanPhamChiTiets.Remove(item);
			await _context.SaveChangesAsync();

			return NoContent();
		}

		// ✅ PATCH: cập nhật ảnh
		[HttpPatch("{id}/update-anh")]
		public async Task<IActionResult> UpdateAnh(Guid id, [FromBody] Guid anhId)
		{
			var item = await _context.SanPhamChiTiets.FindAsync(id);
			if (item == null) return NotFound();

			item.AnhId = anhId;
			item.NgaySua = DateTime.Now;

			_context.SanPhamChiTiets.Update(item);
			await _context.SaveChangesAsync();

			return NoContent();
		}

		// ✅ PATCH: cập nhật màu
		[HttpPatch("{id}/update-mau")]
		public async Task<IActionResult> UpdateMau(Guid id, [FromBody] Guid mauId)
		{
			var item = await _context.SanPhamChiTiets.FindAsync(id);
			if (item == null) return NotFound();

			item.MauSacId = mauId;
			item.NgaySua = DateTime.Now;

			_context.SanPhamChiTiets.Update(item);
			await _context.SaveChangesAsync();

			return NoContent();
		}

		// ✅ PATCH: cập nhật kích cỡ
		[HttpPatch("{id}/update-kichco")]
		public async Task<IActionResult> UpdateKichCo(Guid id, [FromBody] Guid kichCoId)
		{
			var item = await _context.SanPhamChiTiets.FindAsync(id);
			if (item == null) return NotFound();

			item.KichCoId = kichCoId;
			item.NgaySua = DateTime.Now;

			_context.SanPhamChiTiets.Update(item);
			await _context.SaveChangesAsync();

			return NoContent();
		}

		// ✅ PATCH: cập nhật số lượng và giá
		[HttpPatch("{id}/update-sl-gia")]
		public async Task<IActionResult> UpdateSoLuongGia(Guid id, [FromBody] SanPhamChiTiet update)
		{
			var item = await _context.SanPhamChiTiets.FindAsync(id);
			if (item == null) return NotFound();

			item.SoLuong = update.SoLuong;
			item.Gia = update.Gia;
			item.NgaySua = DateTime.Now;

			_context.SanPhamChiTiets.Update(item);
			await _context.SaveChangesAsync();

			return NoContent();
		}
	
	}
}
