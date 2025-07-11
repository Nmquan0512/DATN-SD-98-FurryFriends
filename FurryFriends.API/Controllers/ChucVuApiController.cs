using FurryFriends.API.Models;
using FurryFriends.API.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;

namespace FurryFriends.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChucVuApiController : ControllerBase
    {
        private readonly IChucVuRepository _chucVuRepository;

        public ChucVuApiController(IChucVuRepository chucVuRepository)
        {
            _chucVuRepository = chucVuRepository;
        }
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var chucVus = await _chucVuRepository.GetAllAsync();
                return Ok(chucVus);
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
                var chucVu = await _chucVuRepository.GetByIdAsync(id);
                if (chucVu == null)
                {
                    return NotFound($"Chức vụ với ChucVuId {id} không tồn tại.");
                }
                return Ok(chucVu);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ChucVu chucVu)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                await _chucVuRepository.AddAsync(chucVu);
                return CreatedAtAction(nameof(GetById), new { id = chucVu.ChucVuId }, chucVu);
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
        public async Task<IActionResult> Update(Guid id, [FromBody] ChucVu chucVu)
        {
            if (id != chucVu.ChucVuId)
            {
                return BadRequest("ChucVuId không khớp.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                await _chucVuRepository.UpdateAsync(chucVu);
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
                await _chucVuRepository.DeleteAsync(id);
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
        public async Task<IActionResult> SearchByTenChucVu([FromQuery] string tenChucVu)
        {
            try
            {
                var chucVus = await _chucVuRepository.FindByTenChucVuAsync(tenChucVu);
                return Ok(chucVus);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}