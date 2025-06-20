using FurryFriends.API.Models;
using FurryFriends.API.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;

namespace FurryFriends.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NhanVienApiController : ControllerBase
    {
        private readonly INhanVienRepository _nhanVienRepository;

        public NhanVienApiController(INhanVienRepository nhanVienRepository)
        {
            _nhanVienRepository = nhanVienRepository;
        }

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

        [HttpGet("{nhanVienId}")]
        public async Task<IActionResult> GetById(Guid nhanVienId)
        {
            try
            {
                var nhanVien = await _nhanVienRepository.GetByIdAsync(nhanVienId);
                if (nhanVien == null)
                {
                    return NotFound($"Nhân viên với NhanVienId {nhanVienId} không tồn tại.");
                }
                return Ok(nhanVien);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

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
                return CreatedAtAction(nameof(GetById), new { nhanVienId = nhanVien.NhanVienId }, nhanVien);
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

        [HttpPut("{nhanVienId}")]
        public async Task<IActionResult> Update(Guid nhanVienId, [FromBody] NhanVien nhanVien)
        {
            if (nhanVienId != nhanVien.NhanVienId)
            {
                return BadRequest("NhanVienId không khớp.");
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

        [HttpDelete("{nhanVienId}")]
        public async Task<IActionResult> Delete(Guid nhanVienId)
        {
            try
            {
                await _nhanVienRepository.DeleteAsync(nhanVienId);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

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