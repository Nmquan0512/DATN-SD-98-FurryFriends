using FurryFriends.API.Models;
using FurryFriends.API.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;

namespace FurryFriends.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class TaiKhoanApiController : Controller
    {
		private readonly ITaiKhoanRepository _taiKhoanRepository;

		public TaiKhoanApiController(ITaiKhoanRepository taiKhoanRepository)
		{
			_taiKhoanRepository = taiKhoanRepository;
		}

		[HttpGet]
		public async Task<IActionResult> GetAll()
		{
			try
			{
				var taiKhoans = await _taiKhoanRepository.GetAllAsync();
				return Ok(taiKhoans);
			}
			catch (Exception ex)
			{
				return StatusCode(500, $"Internal server error: {ex.Message}");
			}
		}

		[HttpGet("{id}")]
		public async Task<IActionResult> GetById(Guid id)
		{
			try
			{
				var taiKhoan = await _taiKhoanRepository.GetByIdAsync(id);
				if (taiKhoan == null)
				{
					return NotFound($"Tài khoản với TaiKhoanId {id} không tồn tại.");
				}
				return Ok(taiKhoan);
			}
			catch (Exception ex)
			{
				return StatusCode(500, $"Internal server error: {ex.Message}");
			}
		}

		[HttpPost]
		public async Task<IActionResult> Create([FromBody] TaiKhoan taiKhoan)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			try
			{
				await _taiKhoanRepository.AddAsync(taiKhoan);
				return CreatedAtAction(nameof(GetById), new { id = taiKhoan.TaiKhoanId }, taiKhoan);
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

		[HttpPut("{id}")]
		public async Task<IActionResult> Update(Guid id, [FromBody] TaiKhoan taiKhoan)
		{
			if (id != taiKhoan.TaiKhoanId)
			{
				return BadRequest("TaiKhoanId không khớp.");
			}

			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			try
			{
				await _taiKhoanRepository.UpdateAsync(taiKhoan);
				return NoContent();
			}
			catch (KeyNotFoundException ex)
			{
				return NotFound(ex.Message);
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

		[HttpDelete("{id}")]
		public async Task<IActionResult> Delete(Guid id)
		{
			try
			{
				await _taiKhoanRepository.DeleteAsync(id);
				return NoContent();
			}
			catch (KeyNotFoundException ex)
			{
				return NotFound(ex.Message);
			}
			catch (InvalidOperationException ex)
			{
				return BadRequest(ex.Message);
			}
			catch (Exception ex)
			{
				return StatusCode(500, $"Internal server error: {ex.Message}");
			}
		}

		[HttpGet("search")]
		public async Task<IActionResult> SearchByUserName([FromQuery] string userName)
		{
			try
			{
				var taiKhoan = await _taiKhoanRepository.FindByUserNameAsync(userName);
				if (taiKhoan == null)
				{
					return NotFound($"Tài khoản với UserName {userName} không tồn tại.");
				}
				return Ok(taiKhoan);
			}
			catch (Exception ex)
			{
				return StatusCode(500, $"Internal server error: {ex.Message}");
			}
		}
	}
}
