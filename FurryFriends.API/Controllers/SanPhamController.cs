using FurryFriends.API.Data;
using FurryFriends.API.Models;
using FurryFriends.API.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FurryFriends.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class SanPhamController : ControllerBase
	{
		private readonly AppDbContext _context;

		public SanPhamController(AppDbContext context)
		{
			_context = context;
		}

		// ✅ GET ViewModel: tìm kiếm + phân trang
		[HttpGet]
		public async Task<IActionResult> GetSanPhamView(
			string? search = null, int page = 1, int pageSize = 10)
		{
			var query = _context.SanPhams
				.Include(sp => sp.SanPhamChiTiets)
				.AsQueryable();

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

			return Ok(new
			{
				TotalCount = totalCount,
				Page = page,
				PageSize = pageSize,
				Data = result
			});
		}

		// ✅ GET BY ID (nếu cần dùng riêng 1 sản phẩm)
		[HttpGet("{id}")]
		public async Task<IActionResult> GetById(Guid id)
		{
			var sp = await _context.SanPhams
				.Include(sp => sp.SanPhamChiTiets)
				.FirstOrDefaultAsync(sp => sp.SanPhamId == id);

			if (sp == null) return NotFound();

			var vm = new SanPhamViewModel
			{
				STT = 1,
				SanPhamId = sp.SanPhamId,
				TenSanPham = sp.TenSanPham,
				SoLuong = sp.SanPhamChiTiets.Sum(ct => ct.SoLuong),
				Gia = sp.SanPhamChiTiets.Min(ct => (decimal?)ct.Gia) ?? 0
			};

			return Ok(vm);
		}

		// ✅ CREATE
		[HttpPost]
		public async Task<IActionResult> Create([FromBody] SanPham model)
		{
			if (!ModelState.IsValid) return BadRequest(ModelState);

			model.SanPhamId = Guid.NewGuid();
			_context.SanPhams.Add(model);
			await _context.SaveChangesAsync();

			return CreatedAtAction(nameof(GetById), new { id = model.SanPhamId }, model);
		}

		// ✅ UPDATE
		[HttpPut("{id}")]
		public async Task<IActionResult> Update(Guid id, [FromBody] SanPham model)
		{
			if (!ModelState.IsValid) return BadRequest(ModelState);

			var sp = await _context.SanPhams.FindAsync(id);
			if (sp == null) return NotFound();

			sp.TenSanPham = model.TenSanPham;
			sp.TaiKhoanId = model.TaiKhoanId;
			sp.ThuongHieuId = model.ThuongHieuId;
			sp.TrangThai = model.TrangThai;

			_context.SanPhams.Update(sp);
			await _context.SaveChangesAsync();

			return NoContent();
		}

		// ✅ DELETE
		[HttpDelete("{id}")]
		public async Task<IActionResult> Delete(Guid id)
		{
			var sp = await _context.SanPhams.FindAsync(id);
			if (sp == null) return NotFound();

			_context.SanPhams.Remove(sp);
			await _context.SaveChangesAsync();

			return NoContent();
		}
	}
}
