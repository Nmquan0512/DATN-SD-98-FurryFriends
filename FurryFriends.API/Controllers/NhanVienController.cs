using FurryFriends.API.Models;
using FurryFriends.API.Repository.IRepository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FurryFriends.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class NhanVienController : Controller
    {
		private readonly INhanVienRepository _nhanVienRepository;

		public NhanVienController(INhanVienRepository nhanVienRepository)
		{
			_nhanVienRepository = nhanVienRepository;
		}

		// GET: api/NhanVien
		[HttpGet]
		public async Task<IActionResult> GetAll()
		{
			try
			{
				var nhanViens = await _nhanVienRepository.GetAllAsync();
				return Ok(nhanViens);
			}
			catch (Exception ex)
			{
				return StatusCode(500, $"Internal server error: {ex.Message}");
			}
		}

		// GET: api/NhanVien/{taiKhoanId}
		[HttpGet("{taiKhoanId}")]
		public async Task<IActionResult> GetById(Guid taiKhoanId)
		{
			try
			{
				var nhanVien = await _nhanVienRepository.GetByIdAsync(taiKhoanId);
				if (nhanVien == null)
				{
					return NotFound($"Nhân viên với TaiKhoanId {taiKhoanId} không tồn tại.");
				}
				return Ok(nhanVien);
			}
			catch (Exception ex)
			{
				return StatusCode(500, $"Internal server error: {ex.Message}");
			}
		}

		// POST: api/NhanVien
		[HttpPost]
		public async Task<IActionResult> Create([FromBody] NhanVien nhanVien)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			try
			{
				await _nhanVienRepository.AddAsync(nhanVien);
				return CreatedAtAction(nameof(GetById), new { taiKhoanId = nhanVien.TaiKhoanId }, nhanVien);
			}
			catch (ArgumentException ex)
			{
				return BadRequest(ex.Message);
			}
			catch (Exception ex)
			{
				return StatusCode(500, $"Internal server error: {ex.Message}");
			}
		}

		// PUT: api/NhanVien/{taiKhoanId}
		[HttpPut("{taiKhoanId}")]
		public async Task<IActionResult> Update(Guid taiKhoanId, [FromBody] NhanVien nhanVien)
		{
			if (taiKhoanId != nhanVien.TaiKhoanId)
			{
				return BadRequest("TaiKhoanId không khớp.");
			}

			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			try
			{
				await _nhanVienRepository.UpdateAsync(nhanVien);
				return NoContent();
			}
			catch (ArgumentException ex)
			{
				return BadRequest(ex.Message);
			}
			catch (Exception ex)
			{
				return StatusCode(500, $"Internal server error: {ex.Message}");
			}
		}

		// DELETE: api/NhanVien/{taiKhoanId}
		[HttpDelete("{taiKhoanId}")]
		public async Task<IActionResult> Delete(Guid taiKhoanId)
		{
			try
			{
				await _nhanVienRepository.DeleteAsync(taiKhoanId);
				return NoContent();
			}
			catch (Exception ex)
			{
				return StatusCode(500, $"Internal server error: {ex.Message}");
			}
		}

		// GET: api/NhanVien/search?hoVaTen={hoVaTen}
		[HttpGet("search")]
		public async Task<IActionResult> SearchByHoVaTen([FromQuery] string hoVaTen)
		{
			try
			{
				var nhanViens = await _nhanVienRepository.FindByNameAsync(hoVaTen);
				return Ok(nhanViens);
			}
			catch (Exception ex)
			{
				return StatusCode(500, $"Internal server error: {ex.Message}");
			}
		}
	}
}
