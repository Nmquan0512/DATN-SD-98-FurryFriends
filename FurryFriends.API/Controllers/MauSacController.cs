using FurryFriends.API.Data;
using FurryFriends.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FurryFriends.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class MauSacController : ControllerBase
	{
		private readonly AppDbContext _context;

		public MauSacController(AppDbContext context)
		{
			_context = context;
		}

		[HttpGet]
		public async Task<IActionResult> GetAll() =>
			Ok(await _context.MauSacs.ToListAsync());

		[HttpGet("{id}")]
		public async Task<IActionResult> GetById(Guid id)
		{
			var item = await _context.MauSacs.FindAsync(id);
			return item == null ? NotFound() : Ok(item);
		}

		[HttpPost]
		public async Task<IActionResult> Create([FromBody] MauSac model)
		{
			model.MauSacId = Guid.NewGuid();
			_context.MauSacs.Add(model);
			await _context.SaveChangesAsync();
			return CreatedAtAction(nameof(GetById), new { id = model.MauSacId }, model);
		}

		[HttpPut("{id}")]
		public async Task<IActionResult> Update(Guid id, [FromBody] MauSac model)
		{
			var entity = await _context.MauSacs.FindAsync(id);
			if (entity == null) return NotFound();

			entity.TenMau = model.TenMau;
			entity.MoTa = model.MoTa;
			entity.TrangThai = model.TrangThai;
			await _context.SaveChangesAsync();

			return NoContent();
		}

		[HttpDelete("{id}")]
		public async Task<IActionResult> Delete(Guid id)
		{
			var entity = await _context.MauSacs.FindAsync(id);
			if (entity == null) return NotFound();

			_context.MauSacs.Remove(entity);
			await _context.SaveChangesAsync();
			return NoContent();
		}
	}
}
