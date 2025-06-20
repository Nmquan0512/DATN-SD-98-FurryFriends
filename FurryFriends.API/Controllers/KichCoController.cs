using FurryFriends.API.Data;
using FurryFriends.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FurryFriends.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class KichCoController : ControllerBase
	{
		private readonly AppDbContext _context;

		public KichCoController(AppDbContext context)
		{
			_context = context;
		}

		[HttpGet]
		public async Task<IActionResult> GetAll() =>
			Ok(await _context.KichCos.ToListAsync());

		[HttpGet("{id}")]
		public async Task<IActionResult> GetById(Guid id)
		{
			var item = await _context.KichCos.FindAsync(id);
			return item == null ? NotFound() : Ok(item);
		}

		[HttpPost]
		public async Task<IActionResult> Create([FromBody] KichCo model)
		{
			model.KichCoId = Guid.NewGuid();
			_context.KichCos.Add(model);
			await _context.SaveChangesAsync();
			return CreatedAtAction(nameof(GetById), new { id = model.KichCoId }, model);
		}

		[HttpPut("{id}")]
		public async Task<IActionResult> Update(Guid id, [FromBody] KichCo model)
		{
			var entity = await _context.KichCos.FindAsync(id);
			if (entity == null) return NotFound();

			entity.TenKichCo = model.TenKichCo;
			entity.MoTa = model.MoTa;
			entity.TrangThai = model.TrangThai;
			await _context.SaveChangesAsync();

			return NoContent();
		}

		[HttpDelete("{id}")]
		public async Task<IActionResult> Delete(Guid id)
		{
			var entity = await _context.KichCos.FindAsync(id);
			if (entity == null) return NotFound();

			_context.KichCos.Remove(entity);
			await _context.SaveChangesAsync();
			return NoContent();
		}
	}
}
