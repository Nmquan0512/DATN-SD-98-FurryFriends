using FurryFriends.API.Models;
using FurryFriends.API.Services.IServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FurryFriends.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PhieuHoanTraController : ControllerBase
    {
		private readonly IPhieuHoanTraService _service;

		public PhieuHoanTraController(IPhieuHoanTraService service)
		{
			_service = service;
		}

		// Lấy phiếu hoàn trả theo Id
		[HttpGet("{id}")]
		public async Task<IActionResult> GetById(Guid id, bool includeHoaDonChiTiet = false)
		{
			var result = await _service.GetByIdAsync(id, includeHoaDonChiTiet);
			if (result == null)
				return NotFound();

			return Ok(result);
		}

		// Lấy tất cả phiếu hoàn trả theo Hóa đơn Id
		[HttpGet("GetByHoaDon/{hoaDonId}")]
		public async Task<IActionResult> GetByHoaDonId(Guid hoaDonId)
		{
			var result = await _service.GetByHoaDonIdAsync(hoaDonId);
			return Ok(result);
		}

		// Lấy tất cả phiếu hoàn trả theo Hóa đơn chi tiết Id
		[HttpGet("GetByHoaDonChiTiet/{hoaDonChiTietId}")]
		public async Task<IActionResult> GetByHoaDonChiTietId(Guid hoaDonChiTietId)
		{
			var result = await _service.GetByHoaDonChiTietIdAsync(hoaDonChiTietId);
			return Ok(result);
		}

		// Lấy tổng số lượng hoàn trả theo chi tiết hóa đơn
		[HttpGet("GetTongSoLuongHoan/{hoaDonChiTietId}")]
		public async Task<IActionResult> GetTongSoLuongHoan(Guid hoaDonChiTietId)
		{
			var total = await _service.GetTongSoLuongHoanByHdctAsync(hoaDonChiTietId);
			return Ok(total);
		}

		// Tạo phiếu hoàn trả
		[HttpPost]
		public async Task<IActionResult> Create([FromBody] PhieuHoanTra model)
		{
			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			var created = await _service.AddAsync(model);
			return CreatedAtAction(nameof(GetById), new { id = created.PhieuHoanTraId }, created);
		}

		// Cập nhật phiếu hoàn trả
		[HttpPut("{id}")]
		public async Task<IActionResult> Update(Guid id, [FromBody] PhieuHoanTra model)
		{
			if (id != model.PhieuHoanTraId)
				return BadRequest("Id không khớp");

			await _service.UpdateAsync(model);
			return NoContent();
		}

		// Cập nhật trạng thái phiếu hoàn trả
		[HttpPatch("{id}/TrangThai")]
		public async Task<IActionResult> UpdateTrangThai(Guid id, [FromQuery] int trangThai)
		{
			if (!await _service.ExistsAsync(id))
				return NotFound();

			await _service.UpdateTrangThaiAsync(id, trangThai);
			return NoContent();
		}

		// Xóa phiếu hoàn trả
		[HttpDelete("{id}")]
		public async Task<IActionResult> Delete(Guid id)
		{
			if (!await _service.ExistsAsync(id))
				return NotFound();

			await _service.DeleteAsync(id);
			return NoContent();
		}
	}
}
