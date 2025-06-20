using FurryFriends.API.Data;
using FurryFriends.API.Models;
using Microsoft.AspNetCore.Mvc;

using Microsoft.EntityFrameworkCore;

namespace FurryFriends.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class AnhController : ControllerBase
	{
		
		private readonly AppDbContext _context;
	
		public AnhController(AppDbContext context )
		{
			
			_context = context;
		}
		

		[HttpGet]
		public async Task<IActionResult> GetAll() =>
			Ok(await _context.Anhs.ToListAsync());

		[HttpGet("{id}")]
		public async Task<IActionResult> GetById(Guid id)
		{
			var item = await _context.Anhs.FindAsync(id);
			return item == null ? NotFound() : Ok(item);
		}

		[HttpPost]
		public async Task<IActionResult> Create([FromBody] Anh model)
		{
			model.AnhId = Guid.NewGuid();
			_context.Anhs.Add(model);
			await _context.SaveChangesAsync();
			return CreatedAtAction(nameof(GetById), new { id = model.AnhId }, model);
		}

		[HttpPut("{id}")]
		public async Task<IActionResult> Update(Guid id, [FromBody] Anh model)
		{
			var entity = await _context.Anhs.FindAsync(id);
			if (entity == null) return NotFound();

			entity.TenAnh = model.TenAnh;
			entity.DuongDan = model.DuongDan;
			entity.TrangThai = model.TrangThai;
			await _context.SaveChangesAsync();

			return NoContent();
		}

		[HttpDelete("{id}")]
		public async Task<IActionResult> Delete(Guid id)
		{
			var entity = await _context.Anhs.FindAsync(id);
			if (entity == null) return NotFound();

			_context.Anhs.Remove(entity);
			await _context.SaveChangesAsync();
			return NoContent();
		}

		[HttpGet("view/{fileName}")]
		public IActionResult ViewImage(string fileName)
		{
			var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
			var filePath = Path.Combine(uploadsFolder, fileName);

			if (!System.IO.File.Exists(filePath))
				return NotFound("Ảnh không tồn tại");

			var mimeType = "image/" + Path.GetExtension(filePath).Trim('.');
			return PhysicalFile(filePath, mimeType);
		}

	}
}
